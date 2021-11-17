using System.Text;
using Bot.Core.Exceptions;
using Bot.Money.Enums;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

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

        public async Task<Stream> DownloadArchive(long userId)
        {
            var clientSecret = _userDataRepository.GetClientSecret(userId);

            if (string.IsNullOrEmpty(clientSecret)) { throw new NotFoundUserException(); }

            var url = _buildUrl(_userDataRepository.GetUserSheet(userId), ExportFileType.PDF);
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret)
                                                        .CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await sheetsService.HttpClient.SendAsync(request))
            {
                return new MemoryStream(await response.Content.ReadAsByteArrayAsync());
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