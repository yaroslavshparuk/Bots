using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Youtube.Interfaces;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Youtube.Impl
{
    public class YoutubeBot : IBot
    {
        private readonly Core.Commands _commands;
        private TelegramBotClient _botClient = new (ConfigurationManager.AppSettings["youtube_bot_token"]);
        public YoutubeBot(IEnumerable<IYoutubeCommand> commands)
        {
            _commands = new Core.Commands(commands);
        }

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            _botClient.OnMessage += OnMessage;
            _botClient.StartReceiving();

            _logger.Info("Youtube bot was started");
        }

        public void Stop()
        {
            _botClient.StopReceiving();
            _logger.Info("Youtube bot was stoped");
        }

        private async void OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                await _commands.DetermineAndGetCommand(e.Message).Execute(e.Message, _botClient);
                _logger.Debug($"Proccessed message from: User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
            }
            catch (NotFoundCommandException)
            {
                _logger.Debug($"Message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, "Seemds you send me incorrect URL", ParseMode.Default, false, false, 0);
            }
            catch (MaxUploadSizeExceededException ex)
            {
                _logger.Debug($"Message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, false, false, 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
    }
}
