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
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { "Cancel" }, }) { ResizeKeyboard = true };
        private readonly ReplyKeyboardMarkup _skipReply = new(new[] { new KeyboardButton[] { "Skip" } }) { ResizeKeyboard = true };
        private readonly IUserCommandHistory _userCommandHistory;
        private readonly IBudgetRepository _budgetRepository;

        public FinanceOperationCommand(IUserCommandHistory userCommandHistory, IBudgetRepository budgetRepository)
        {
            _userCommandHistory = userCommandHistory;
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            return Regex.IsMatch(message.Text, @"[\d]{1,9}([.,][\d]{1,6})?") || _userCommandHistory.HasHistory(message.Chat.Id);
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (!CanExecute(message)) { throw new NotFoundCommandException(); }
            if (message.Text is "Cancel")
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
                _userCommandHistory.Clear(message.Chat.Id);
                return;
            }
            if (!_userCommandHistory.HasHistory(message.Chat.Id))
            {
                _userCommandHistory.StartNewHistory(message);
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);
            }
            else
            {
                switch (_userCommandHistory.HistoryLength(message.Chat.Id))
                {
                    case 1:
                        var categories = await _budgetRepository.GetFinanceOperationCategories(message.Chat.Id, message.Text);
                        var categoriesKeyboardMarkUp = new ReplyKeyboardMarkup(categories.Select(x => new KeyboardButton(x)).Split(_keyBoardMarkUpRowSize));
                        _userCommandHistory.Add(message);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "What category is it?", replyMarkup: categoriesKeyboardMarkUp);
                        break;
                    case 2:
                        _userCommandHistory.Add(message);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Send me a description of it (optional) ", replyMarkup: _skipReply);
                        break;
                    case 3:
                        _userCommandHistory.Add(message);
                        var financeOperationMessage = new FinanceOperationMessage(message.Chat.Id, _userCommandHistory.GetHistory(message.Chat.Id));
                        _budgetRepository.CreateRecord(financeOperationMessage);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Added", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    default:
                        _userCommandHistory.Clear(message.Chat.Id);
                        throw new NotFoundCommandException();
                }
            }
        }
    }
}
