using Bot.Domain.Models;
using Bot.Repositories.Interfaces;
using Bot.Repository.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
namespace Bot.Repositories.Impl
{
    public class GoogleSpreadSheetsBudgetRepository : IBudgetRepository
    {
        private const string SHEET_NAME = "Transactions";
        private readonly IUserDataRepository _userDataRepository;

        public GoogleSpreadSheetsBudgetRepository(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public void Create(Expense expense)
        {
            var range = $"{SHEET_NAME}!B:E";
            var valueRange = new ValueRange();

            var objectList = expense.GetTranferObject();
            valueRange.Values = new List<IList<object>> { objectList };
            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = "Monthly Budget",
                HttpClientInitializer = GoogleCredential.FromJson(_userDataRepository.GetClientSecret(expense.ClientSecretId))
                                                       .CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, _userDataRepository.GetUserSheet(expense.UserSheetId), range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
            }
        }

        public void Create(Income income)
        {

            var range = $"{SHEET_NAME}!G:J";
            var valueRange = new ValueRange();

            var objectList = income.GetTranferObject();
            valueRange.Values = new List<IList<object>> { objectList };

            using (var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = "Monthly Budget",
                HttpClientInitializer = GoogleCredential.FromJson(_userDataRepository.GetClientSecret(income.ClientSecretId))
                                                       .CreateScoped(SheetsService.Scope.Spreadsheets)
            }))
            {
                var appendRequest = sheetsService.Spreadsheets.Values.Append(valueRange, _userDataRepository.GetUserSheet(income.UserSheetId), range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                appendRequest.Execute();
            }
        }
    }
}
