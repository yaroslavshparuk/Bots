﻿using System.Text;
using Bot.Abstractions.Exceptions;
using Bot.Money.Enums;
using Bot.Money.Exceptions;
using Bot.Money.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.UpdateRequest;

namespace Bot.Money.Repositories
{
    public class GoogleSpreadSheetsBudgetRepository : IBudgetRepository
    {
        private const string _summarySheetName = "Summary";
        private const string _transactionsSheetName = "Transactions";
        private readonly IUserDataRepository _userDataRepository;
        private readonly GoogleSpreadSheetsExportUrl _exportUrl;

        public GoogleSpreadSheetsBudgetRepository(IUserDataRepository userDataRepository, GoogleSpreadSheetsExportUrl exportUrl)
        {
            _userDataRepository = userDataRepository;
            _exportUrl = exportUrl;
        }

        public async Task CreateRecord(FinanceOperationMessage financeOperationMessage)
        {
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(await _userDataRepository.GetClientSecret(financeOperationMessage.UserId)).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(
                                        GetValueRange(
                                            financeOperationMessage.BuildTranferObject()), 
                                            await _userDataRepository.GetUserSheet(financeOperationMessage.UserId),
                                            financeOperationMessage.TransactionRange());
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                await appendRequest.ExecuteAsync();
            }
        }

        public async Task<Stream> DownloadArchive(long userId)
        {
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(await _userDataRepository.GetClientSecret(userId)).CreateScoped(SheetsService.Scope.Spreadsheets),
            }))
            {
                var userSheet = await _userDataRepository.GetUserSheet(userId);
                byte[] pdfBytes = null;
                byte[] xlsxBytes = null;

                using (var responsePdf = await sheetsService.HttpClient.GetAsync(_exportUrl.BuildWith(userSheet, FileType.Pdf)))
                {
                    if (!responsePdf.IsSuccessStatusCode)
                    {
                        throw new DownloadException();
                    }

                    pdfBytes = await responsePdf.Content.ReadAsByteArrayAsync();
                }

                using (var responseXlsx = await sheetsService.HttpClient.GetAsync(_exportUrl.BuildWith(userSheet, FileType.Xlsx)))
                {
                    if (!responseXlsx.IsSuccessStatusCode)
                    {
                        throw new DownloadException();
                    }

                    xlsxBytes = await responseXlsx.Content.ReadAsByteArrayAsync();
                }

                var outputStream = new MemoryStream();

                using (var zipStream = new ZipOutputStream(outputStream))
                {
                    zipStream.PutNextEntry(new ZipEntry($"Budget.pdf"));

                    var buffer = new byte[4096];

                    using (var pdfStream = new MemoryStream(pdfBytes))
                        StreamUtils.Copy(pdfStream, zipStream, buffer);
                    zipStream.CloseEntry();

                    zipStream.PutNextEntry(new ZipEntry($"Budget.xlsx"));

                    using (var xlsxStream = new MemoryStream(xlsxBytes))
                        StreamUtils.Copy(xlsxStream, zipStream, buffer);
                    zipStream.CloseEntry();

                    zipStream.IsStreamOwner = false;
                }

                outputStream.Position = 0;
                return outputStream;
            }
        }

        public async Task<IEnumerable<string>> GetCategories(long userId, string category)
        {
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(await _userDataRepository.GetClientSecret(userId)).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var range = new StringBuilder(_summarySheetName);
                if (category == "Витрата")
                {
                    range.Append("!B23:B");
                }
                else if (category == "Дохід")
                {
                    range.Append("!H23:H");
                }
                else
                {
                    throw new UserChoiceException("Такої категорії немає у списку ⛔️");
                }
                var categories = sheetsService.Spreadsheets.Values.Get(await _userDataRepository.GetUserSheet(userId), range.ToString());
                return (await categories.ExecuteAsync()).Values.Select(x => x.FirstOrDefault().ToString());
            }
        }

        public async Task ResetMonth(long userId)
        {
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(await _userDataRepository.GetClientSecret(userId)).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var resetMonthValueRange = GetValueRange(new List<object>() { DateTime.Now.ToString("MMMM yyyy") });
                var resetMonthRequest = sheetsService.Spreadsheets.Values.Update(
                                        resetMonthValueRange, await _userDataRepository.GetUserSheet(userId), $"{_summarySheetName}!B3:E4");
                resetMonthRequest.ValueInputOption = ValueInputOptionEnum.USERENTERED;
                await resetMonthRequest.ExecuteAsync();

                var getEndBalanceRequest = sheetsService.Spreadsheets.Values.Get(await _userDataRepository.GetUserSheet(userId), $"{_summarySheetName}!C9");
                var endBalance = (await getEndBalanceRequest.ExecuteAsync()).Values.FirstOrDefault().FirstOrDefault().ToString().Replace("UAH", "");

                var changeStartingBalanceValueRange = GetValueRange(new List<object>() { endBalance });
                var changeStartingBalanceRequest = sheetsService.Spreadsheets.Values.Update(
                                                   changeStartingBalanceValueRange, await _userDataRepository.GetUserSheet(userId), $"{_summarySheetName}!L3");
                changeStartingBalanceRequest.ValueInputOption = ValueInputOptionEnum.USERENTERED;
                await changeStartingBalanceRequest.ExecuteAsync();

                var deleteExpensesRequest = sheetsService.Spreadsheets.Values.Clear(null, await _userDataRepository.GetUserSheet(userId), $"{_transactionsSheetName}!B5:E");
                await deleteExpensesRequest.ExecuteAsync();

                var deleteIncomesRequest = sheetsService.Spreadsheets.Values.Clear(null, await _userDataRepository.GetUserSheet(userId), $"{_transactionsSheetName}!G5:J");
                await deleteIncomesRequest.ExecuteAsync();
            }
        }

        private ValueRange GetValueRange(IList<object> objectList)
        {
            return new ValueRange() { Values = new List<IList<object>> { objectList } };
        }
    }
}