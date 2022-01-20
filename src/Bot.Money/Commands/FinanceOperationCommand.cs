using Bot.Core.Exceptions;
using Bot.Core.Extensions;
using Bot.Money.Interfaces;
using Bot.Money.Models;
using Bot.Money.Repositories;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Commands
{
    public class FinanceOperationCommand : IMoneyCommand
    {
        private const int _keyBoardMarkUpRowSize = 2;
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, new KeyboardButton[] { "Cancel" }, }) { ResizeKeyboard = true };
        private readonly ReplyKeyboardMarkup _skipReply = new(new[] { new KeyboardButton[] { "Skip" } }) { ResizeKeyboard = true };
        private readonly UsersFinanceOperationCommandHistory _operationCommandHistory = new UsersFinanceOperationCommandHistory();
        private readonly IBudgetRepository _budgetRepository;

        public FinanceOperationCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            return Regex.IsMatch(message.Text, @"[\d]{1,9}([.,][\d]{1,6})?") || _operationCommandHistory.HasUnfinishedCommand(message.Chat.Id);
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (message.Text is "Cancel")
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
                _operationCommandHistory.TryFlush(message.Chat.Id);
                return;
            }
            if (!_operationCommandHistory.HasUnfinishedCommand(message.Chat.Id))
            {
                _operationCommandHistory.StartNewHistory(message);
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);
            }
            else
            {
                switch (_operationCommandHistory.TakeUserHistoryLength(message.Chat.Id))
                {
                    case 1:
                        var categories = await _budgetRepository.GetFinanceOperationCategories(message.Chat.Id, message.Text);
                        var categoriesKeyboardMarkUp = new ReplyKeyboardMarkup(categories.Select(x => new KeyboardButton(x)).Split(_keyBoardMarkUpRowSize));
                        _operationCommandHistory.Push(message);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "What category is it?", replyMarkup: categoriesKeyboardMarkUp);
                        break;
                    case 2:
                        _operationCommandHistory.Push(message);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Send me a description of it (optional) ", replyMarkup: _skipReply);
                        break;
                    case 3:
                        _operationCommandHistory.Push(message);
                        var finOperMessage = _operationCommandHistory.Complete(message.Chat.Id);
                        _budgetRepository.CreateRecord(finOperMessage);
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Added", replyMarkup: new ReplyKeyboardRemove());
                        break;
                    default:
                        _operationCommandHistory.TryFlush(message.Chat.Id);
                        throw new NotFoundCommandException();
                }
            }
        }
    }
}
