using Bot.Money.Enums;
using Bot.Money.Interfaces;
using Bot.Money.Models;
using Bot.Money.Repositories;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Money.Commands
{
    public class FinanceOperationCommand : IMoneyCommand
    {
        private const int _keyBoardMarkUpRowSize = 2;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ConcurrentDictionary<long, ICollection<string>> _history = new();
        private readonly ReplyKeyboardMarkup _expOrIncReply = new(new[] { new KeyboardButton[] { "Expense", "Income" }, })
        {
            ResizeKeyboard = true
        };

        public FinanceOperationCommand(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public bool CanExecute(Message message)
        {
            return Regex.IsMatch(message.Text, "^[0-9]+$") || (_history.TryGetValue(message.Chat.Id, out var history) && history.Count() > 0);
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (_history.TryGetValue(message.Chat.Id, out var history))
            {
                switch (history.Count())
                {
                    case 1:
                        ReplyKeyboardMarkup categoriesKeyboardMarkUp = null;
                        var keyBoardButtons = new List<List<KeyboardButton>>();
                        if (message.Text is "Expense")
                        {
                            categoriesKeyboardMarkUp = new(Split(Enum.GetNames(typeof(ExpenseCategory)).Select(x => new KeyboardButton(x)), _keyBoardMarkUpRowSize));
                        }
                        else if (message.Text is "Income")
                        {
                            categoriesKeyboardMarkUp = new(Split(Enum.GetNames(typeof(IncomeCategory)).Select(x => new KeyboardButton(x)), _keyBoardMarkUpRowSize));
                        }

                        categoriesKeyboardMarkUp.ResizeKeyboard = true;
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "What category is it?", replyMarkup: categoriesKeyboardMarkUp);
                        break;
                    case 2:
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Send me a description of your " + history.Last().ToLower(), replyMarkup: new ReplyKeyboardRemove());
                        break;
                    default:
                        history.Add(message.Text);
                        var response = _budgetRepository.CreateAndGetResult(new FinanceOperationMessage(message.Chat.Id, history));
                        await botClient.SendTextMessageAsync(chatId: message.Chat, text: response);
                        _history.TryRemove(message.Chat.Id, out var f);
                        break;
                }
                history.Add(message.Text);
            }
            else
            {
                _history.TryAdd(message.Chat.Id, new List<string> { message.Text });
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Is expense or income?", replyMarkup: _expOrIncReply);
            }
        }

        public IEnumerable<IEnumerable<KeyboardButton>> Split(IEnumerable<KeyboardButton> array, int size)
        {
            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
