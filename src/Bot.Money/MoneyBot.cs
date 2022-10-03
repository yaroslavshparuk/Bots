using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Google;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Bot.Core.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Message = Bot.Core.Abstractions.Message;

namespace Bot.Money
{
    public class MoneyBot : IBot
    {
        private readonly IEnumerable<IMoneyBotInputHandler> _handlers;
        private readonly IChatSessionService _chatSessionService;
        private TelegramBotClient _botClient = new(ConfigurationManager.AppSettings["money_bot_token"]);
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MoneyBot(IEnumerable<IMoneyBotInputHandler> handlers, IChatSessionService chatSessionService)
        {
            _handlers = handlers;
            _chatSessionService = chatSessionService;
        }


        public void Start()
        {
            _botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync);
            _logger.Info("Money bot was started");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
        {
            Message message = null;
            try
            {
                if (update.Type == UpdateType.CallbackQuery)
                {
                    message = new Message(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Username, update.CallbackQuery.Data);
                }
                else if (update.Type == UpdateType.Message)
                {
                    message = new Message(update.Message.Chat.Id, update.Message.Chat.Username, update.Message.Text);
                    await _botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
                }
                await new Dispatcher(_handlers, _chatSessionService, _botClient).Dispatch(message);
                _logger.Debug($"Proccessed Message from: User Id: {message.ChatId} UserName: @{message.UserName}");
            }
            catch (NotFoundCommandException ex)
            {
                _logger.Debug($"Message: '{message.Text}' User Id: {message.ChatId} UserName: @{message.UserName}");
                await _botClient.SendTextMessageAsync(message.ChatId, ex.Message);
            }
            catch (UserChoiceException ex)
            {
                _logger.Debug($"Message: '{message.Text}' User Id: {message.ChatId} UserName: @{message.UserName}");
                await _botClient.SendTextMessageAsync(message.ChatId, ex.Message);
            }
            catch (NotFoundUserException)
            {
                _logger.Warn(message.Text + $"\nMessage: '{message.Text}' User Id: {message.ChatId} UserName: @{message.UserName}");
                await _botClient.SendTextMessageAsync(message.ChatId, "I don't know you, if you want to use me - contact @shparuk please");
            }
            catch (GoogleApiException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(message.ChatId, "Seems you provided a wrong spread sheet");
            }
            catch (DownloadException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(message.ChatId, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken = default)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
