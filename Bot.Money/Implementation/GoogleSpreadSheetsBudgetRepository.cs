using System.Collections.Generic;
using System.Text;
using Bot.Core.Exceptions;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Bot.Money.Implementation
{
    public class GoogleSpreadSheetsBudgetRepository : IBudgetRepository
    {
        private const string SHEET_NAME = "Transactions";
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
            var range = new StringBuilder(SHEET_NAME);

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
            var clientSecret = _userDataRepository.GetClientSecret(operation.ClientSecretId);

            if (string.IsNullOrEmpty(clientSecret)) { throw new NotFoundUserException(); }

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = "Monthly Budget",
                HttpClientInitializer = GoogleCredential.FromJson(clientSecret).CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, _userDataRepository.GetUserSheet(operation.UserSheetId), range.ToString());
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
            }

            return result;
        }
    }
}