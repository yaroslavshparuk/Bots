using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Core.Extensions;
using Bot.Youtube.Handlers;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Youtube
{
    public class YoutubeBot : IBot
    {
        private readonly IEnumerable<IYoutubeBotInputHandler> _commands;
        private readonly IChatSessionService _chatSessionService;
        private TelegramBotClient _botClient = new (ConfigurationManager.AppSettings["youtube_bot_token"]);
        public YoutubeBot(IEnumerable<IYoutubeBotInputHandler> commands, IChatSessionService chatSessionService)
        {
            _commands = commands;
            _chatSessionService = chatSessionService;
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
                var session = _chatSessionService.Upload(e.Message.Chat.Id);
                await new Dispatcher(_commands, _chatSessionService).Dispatch(e.Message, _botClient);

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
