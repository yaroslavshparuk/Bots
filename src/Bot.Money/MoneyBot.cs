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
            try
            {
                await new Dispatcher(_handlers, _chatSessionService, _botClient).Dispatch(update.Message);
                _logger.Debug($"Proccessed update.Message from: User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
            }
            catch (NotFoundCommandException ex)
            {
                _logger.Debug($"update.Message: '{update.Message.Text}' User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(update.Message.Chat, ex.Message);
            }
            catch (UserChoiceException ex)
            {
                _logger.Debug($"update.Message: '{update.Message.Text}' User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(update.Message.Chat, ex.Message);
            }
            catch (NotFoundUserException)
            {
                _logger.Warn(update.Message + $"\nupdate.Message: '{update.Message.Text}' User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(update.Message.Chat, "I don't know you, if you want to use me - contact @shparuk please");
            }
            catch (GoogleApiException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(update.Message.Chat, "Seems you provided a wrong spread sheet");
            }
            catch (DownloadException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(update.Message.Chat, ex.Message);
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
