using System.Text;
using Bot.Core.Exceptions;
using Bot.Money.Enums;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Telegram.Bot.Types;

namespace Bot.Money.Impl
{
    public class GoogleSpreadSheetsBudgetRepository : IBudgetRepository
    {
        private const string TRANSACTIONS_SHEET = "Transactions";
        private readonly IUserDataRepository _userDataRepository;

        public GoogleSpreadSheetsBudgetRepository(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public string CreateAndGetResult(FinanceOperationMessage message)
        {
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

            var valueRange = new ValueRange() { Values = new List<IList<object>> { objectList } };
            var clientSecret = _userDataRepository.GetClientSecret(operation.UserId);

            if (string.IsNullOrEmpty(clientSecret)) { throw new NotFoundUserException(); }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, _userDataRepository.GetUserSheet(operation.UserId), range.ToString());
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
            }

            return result;
        }

        public async Task<Stream> DownloadArchive(Message message)
        {
            var clientSecret = _userDataRepository.GetClientSecret(message.Chat.Id);
            var userSheet = _userDataRepository.GetUserSheet(message.Chat.Id);
            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(userSheet)) { throw new NotFoundUserException(); }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                byte[] pdfBytes = null;
                byte[] xlsxBytes = null;

                using (var requestPdf = new HttpRequestMessage(HttpMethod.Get, _buildUrl(userSheet, ExportFileType.PDF)))
                using (var responsePdf = await sheetsService.HttpClient.SendAsync(requestPdf))
                    pdfBytes = await responsePdf.Content.ReadAsByteArrayAsync();

                using (var requestXlsx = new HttpRequestMessage(HttpMethod.Get, _buildUrl(userSheet, ExportFileType.XLSX)))
                using (var responseXlsx = await sheetsService.HttpClient.SendAsync(requestXlsx))
                    xlsxBytes = await responseXlsx.Content.ReadAsByteArrayAsync();

                var outputStream = new MemoryStream();

                using (var zipStream = new ZipOutputStream(outputStream))
                {
                    zipStream.PutNextEntry(new ZipEntry($"{message.Chat.Username}.pdf"));

                    using (var pdfStream = new MemoryStream(pdfBytes))
                        StreamUtils.Copy(pdfStream, zipStream, new byte[4096]);
                    zipStream.CloseEntry();

                    zipStream.PutNextEntry(new ZipEntry($"{message.Chat.Username}.xlsx"));

                    using (var xlsxStream = new MemoryStream(xlsxBytes))
                        StreamUtils.Copy(xlsxStream, zipStream, new byte[4096]);
                    zipStream.CloseEntry();

                    zipStream.IsStreamOwner = false;
                }

                outputStream.Position = 0;
                return outputStream;
            }
        }

        private string _buildUrl(string userSheet, ExportFileType fileType)
        {
            var url = new StringBuilder($"https://docs.google.com/spreadsheets/d/{userSheet}/export?format=");
            switch (fileType)
            {
                case ExportFileType.PDF:
                    url.Append("pdf");
                    break;
                case ExportFileType.XLSX:
                    url.Append("xlsx");
                    break;
            }
            url.Append($"&id={userSheet}");
            return url.ToString();
        }
    }
}