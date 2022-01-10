using Bot.Core.Abstractions;
using Bot.Money.Interfaces;
using Google;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Bot.Core.Exceptions;
using Bot.Core;

namespace Bot.Money.Impl
{
    public class MoneyBot : IBot
    {
        private readonly IEnumerable<IMoneyCommand> _commands;
        private TelegramBotClient _botClient = new (ConfigurationManager.AppSettings["money_bot_token"]);
        private CommandsCollection _commandsCollection;

        public MoneyBot(IEnumerable<IMoneyCommand> commands)
        {
            _commands = commands; 
        }

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            _commandsCollection = new(_commands);

            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();

            _logger.Info("Money bot was started");
        }

        public void Stop()
        {
            _botClient.StopReceiving();
            _logger.Info("Money bot was stoped");
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                await _commandsCollection.GetAppropriateCommandOnMessage(e.Message).Execute(e.Message, _botClient);
                _logger.Debug($"Proccessed message from: User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
            }
            catch (NotFoundCommandException ex)
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
                await _botClient.SendTextMessageAsync(e.Message.Chat, "Seems you provided wrong spread sheet URL");
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
