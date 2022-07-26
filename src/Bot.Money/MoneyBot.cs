using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Google;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Bot.Core.Exceptions;

namespace Bot.Money
{
    public class MoneyBot : IBot
    {
        private readonly IEnumerable<IMoneyBotInputHandler> _commands;
        private readonly IChatSessionService _chatSessionService;
        private TelegramBotClient _botClient = new(ConfigurationManager.AppSettings["money_bot_token"]);

        public MoneyBot(IEnumerable<IMoneyBotInputHandler> commands, IChatSessionService chatSessionService)
        {
            _commands = commands;
            _chatSessionService = chatSessionService;
        }

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();
            _logger.Info("Money bot was started");
        }

        public void Stop()
        {
            _botClient.OnMessage -= OnMessage;
            _botClient.StopReceiving();
            _logger.Info("Money bot was stoped");
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                await new Dispatcher(_commands, _chatSessionService).Dispatch(e.Message, _botClient); // remove _chatSessionService, create separate dispatcher for every bot
                _logger.Debug($"Proccessed message from: User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
            }
            catch (NotFoundCommandException ex)
            {
                _logger.Debug($"Message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message);
            }
            catch(UserChoiceException ex)
            {
                _logger.Debug($"Message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message);
            }
            catch (NotFoundUserException ex)
            {
                _logger.Warn(ex.Message + $"\nMessage: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, "I don't know you, if you want to use me - contact @shparuk please");
            }
            catch (GoogleApiException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(e.Message.Chat, "Seems you provided a wrong spread sheet");
            }
            catch (DownloadException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
    }
}
