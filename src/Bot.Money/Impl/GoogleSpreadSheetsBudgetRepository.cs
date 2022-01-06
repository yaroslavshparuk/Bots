using System.Configuration;
using System.Text;
using Bot.Core;
using Bot.Core.Enums;
using Bot.Core.Exceptions;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.UpdateRequest;

namespace Bot.Money.Impl
{
    public class GoogleSpreadSheetsBudgetRepository : IBudgetRepository
    {
        private const string SUMMARY_SHEET = "Summary";
        private const string TRANSACTIONS_SHEET = "Transactions";
        private readonly IUserDataRepository _userDataRepository;

        public GoogleSpreadSheetsBudgetRepository(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public string CreateAndGetResult(FinanceOperationMessage message)
        {
            var clientSecret = _userDataRepository.GetClientSecret(message.UserId);
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new NotFoundUserException();
            }

            FinanceOperation operation = null;
            IList<object> objectList = null;
            var result = string.Empty;
            var range = new StringBuilder(TRANSACTIONS_SHEET);

            if (message.IsExpense())
            {
                operation = message.ToExpense();
                objectList = (operation as Expense).GetTranferObject();
                result = "Expense was added";
                range.Append("!B:E");
            }

            else if (message.IsIncome())
            {
                operation = message.ToIncome();
                objectList = (operation as Income).GetTranferObject();
                result = "Income was added";
                range.Append("!G:J");
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(GetValueRange(objectList), _userDataRepository.GetUserSheet(operation.UserId), range.ToString());
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
            }

            return result;
        }

        public async Task<Stream> DownloadArchive(long userId)
        {
            var clientSecret = _userDataRepository.GetClientSecret(userId);
            var userSheet = _userDataRepository.GetUserSheet(userId);

            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(userSheet))
            {
                throw new NotFoundUserException();
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets),
            }))
            {
                byte[] pdfBytes = null;
                byte[] xlsxBytes = null;

                var urlBuiler = new ExportUrlBuilder(ConfigurationManager.AppSettings["export_url"]);

                using (var responsePdf = await sheetsService.HttpClient.GetAsync(urlBuiler.Build(userSheet, ExportFileType.PDF)))
                {
                    if (!responsePdf.IsSuccessStatusCode)
                    {
                        throw new DownloadException();
                    }

                    pdfBytes = await responsePdf.Content.ReadAsByteArrayAsync();
                }

                using (var responseXlsx = await sheetsService.HttpClient.GetAsync(urlBuiler.Build(userSheet, ExportFileType.XLSX)))
                {
                    if(!responseXlsx.IsSuccessStatusCode)
                    {
                        throw new DownloadException();
                    }

                    xlsxBytes = await responseXlsx.Content.ReadAsByteArrayAsync();
                }

                var outputStream = new MemoryStream();

                using (var zipStream = new ZipOutputStream(outputStream))
                {
                    zipStream.PutNextEntry(new ZipEntry($"budget.pdf"));

                    var buffer = new byte[4096];

                    using (var pdfStream = new MemoryStream(pdfBytes))
                        StreamUtils.Copy(pdfStream, zipStream, buffer);
                    zipStream.CloseEntry();

                    zipStream.PutNextEntry(new ZipEntry($"budget.xlsx"));

                    using (var xlsxStream = new MemoryStream(xlsxBytes))
                        StreamUtils.Copy(xlsxStream, zipStream, buffer);
                    zipStream.CloseEntry();

                    zipStream.IsStreamOwner = false;
                }

                outputStream.Position = 0;
                return outputStream;
            }
        }

        public async Task ResetMonth(long userId)
        {
            var clientSecret = _userDataRepository.GetClientSecret(userId);
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new NotFoundUserException();
            }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var resetMonthValueRange = GetValueRange(new List<object>() { DateTime.Now.ToString("MMMM") + " Monthly Budget" });
                var resetMonthRequest = sheetsService.Spreadsheets.Values.Update(
                                        resetMonthValueRange, _userDataRepository.GetUserSheet(userId), $"{SUMMARY_SHEET}!B2:E3");
                resetMonthRequest.ValueInputOption = ValueInputOptionEnum.USERENTERED;
                await resetMonthRequest.ExecuteAsync();

                var getEndBalanceRequest = sheetsService.Spreadsheets.Values.Get(_userDataRepository.GetUserSheet(userId), $"{SUMMARY_SHEET}!E11");
                var endBalance = (await getEndBalanceRequest.ExecuteAsync()).Values.FirstOrDefault().FirstOrDefault().ToString().Replace("UAH", "");

                var changeStartingBalanceValueRange = GetValueRange(new List<object>() { endBalance });
                var changeStartingBalanceRequest = sheetsService.Spreadsheets.Values.Update(
                                                   changeStartingBalanceValueRange, _userDataRepository.GetUserSheet(userId), $"{SUMMARY_SHEET}!L2");
                changeStartingBalanceRequest.ValueInputOption = ValueInputOptionEnum.USERENTERED;
                await changeStartingBalanceRequest.ExecuteAsync();

                var deleteExpensesRequest = sheetsService.Spreadsheets.Values.Clear(null, _userDataRepository.GetUserSheet(userId), $"{TRANSACTIONS_SHEET}!B5:E");
                await deleteExpensesRequest.ExecuteAsync();

                var deleteIncomesRequest = sheetsService.Spreadsheets.Values.Clear(null, _userDataRepository.GetUserSheet(userId), $"{TRANSACTIONS_SHEET}!G5:J");
                await deleteIncomesRequest.ExecuteAsync();
            }
        }

        private ValueRange GetValueRange(IList<object> objectList)
        {
            return new ValueRange() { Values = new List<IList<object>> { objectList } };
        }
    }
}