using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Core.Extensions;
using Bot.Money.Models;
using Bot.Money.Repositories;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Commands
{
    public class FinanceOperationCommand : IMoneyBotCommand
    {
        private const int _keyBoardMarkUpRowSize = 2;
        private const string _floatNumberPattern = @"[\d]{1,9}([.,][\d]{1,6})?$";
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { "Cancel" }, }) { ResizeKeyboard = true };
        private readonly ReplyKeyboardMarkup _skipReply = new(new[] { new KeyboardButton[] { "Skip" } }) { ResizeKeyboard = true };
        private readonly ICommandSteps _commandSteps;
        private readonly IBudgetRepository _budgetRepository;

        public FinanceOperationCommand(ICommandSteps commandSteps, IBudgetRepository budgetRepository)
        {
            _commandSteps = commandSteps;
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            return Regex.IsMatch(message.Text, _floatNumberPattern) || _commandSteps.IsStarted(message.Chat.Id);
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (!CanExecute(message)) { throw new ArgumentException(); }

            if (message.Text is "Cancel")
            {
                _commandSteps.Finish(message.Chat.Id);
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            if (!_commandSteps.IsStarted(message.Chat.Id))
            {
                _commandSteps.Start(message);
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);
                return;
            }

            switch (_commandSteps.Passed(message.Chat.Id))
            {
                case 1:
                    if (message.Text is not ("Income" or "Expense")) { throw new UserChoiceException("You should choose 'Expense' or 'Income'"); }
                    var categories = await _budgetRepository.GetFinanceOperationCategories(message.Chat.Id, message.Text);
                    var categoriesKeyboardMarkUp = new ReplyKeyboardMarkup(categories.Select(x => new KeyboardButton(x)).Split(_keyBoardMarkUpRowSize));
                    _commandSteps.Pass(message);
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: "What category is it?", replyMarkup: categoriesKeyboardMarkUp);
                    break;
                case 2:
                    _commandSteps.Pass(message);
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Send me a description of it (optional) ", replyMarkup: _skipReply);
                    break;
                case 3:
                    var financeOperationMessage = new FinanceOperationMessage(message.Chat.Id, _commandSteps.PrintPassed(message.Chat.Id));
                    _budgetRepository.CreateRecord(financeOperationMessage);
                    _commandSteps.Pass(message);
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Added", replyMarkup: new ReplyKeyboardRemove());
                    _commandSteps.Finish(message.Chat.Id);
                    break;
                default:
                    _commandSteps.Finish(message.Chat.Id);
                    throw new NotFoundCommandException();
            }
        }
    }
}
