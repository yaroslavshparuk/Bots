using Bot.Money.Enums;
using Bot.Money.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bot.Tests.MoneyBot
{
    public class FinanceOperationTests
    {
        [Fact]
        public void ExpenseGetTranferObjectTest()
        {
            var expense = new Expense(0, new DateTime(2021, 1, 1, 1, 1, 1, 1), 100, "Test GetTranferObject()", ExpenseCategory.Other);
            Assert.Equal(new List<object>() { "01/01/2021 1:01 AM", "100", "Test GetTranferObject()", "Other" }, expense.GetTranferObject());
        }

        [Fact]
        public void IncomeGetTranferObjectTest()
        {
            var income = new Income(0, new DateTime(2021, 1, 1, 1, 1, 1, 1), 100, "Test GetTranferObject()", IncomeCategory.Other);
            Assert.Equal(new List<object>() { "01/01/2021 1:01 AM", "100", "Test GetTranferObject()", "Other" }, income.GetTranferObject());
        }

        [Fact]
        public void GetClientSecretIdTest()
        {
            var income = new Income(0, new DateTime(2021, 1, 1, 1, 1, 1, 1), 100, "Test ClientSecretId", IncomeCategory.Other);
            Assert.Equal("0_secret", income.ClientSecretId);
        }

        [Fact]
        public void GetUserSheetIdTest()
        {
            var income = new Income(0, new DateTime(2021, 1, 1, 1, 1, 1, 1), 100, "Test ClientSecretId", IncomeCategory.Other);
            Assert.Equal("0_sheet", income.UserSheetId);
        }
    }
}
