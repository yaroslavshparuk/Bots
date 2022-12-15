using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Youtube.Exceptions;
using Bot.Youtube.Handlers;
using log4net;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Message = Bot.Core.Abstractions.Message;

namespace Bot.Youtube
{
    public class YoutubeBot : IBot
    {
        private readonly IEnumerable<IYoutubeBotInput> _commands;
        private readonly IChatSessionService _chatSessionService;
        private TelegramBotClient _botClient = new(ConfigurationManager.AppSettings["youtube_bot_token"]);
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public YoutubeBot(IEnumerable<IYoutubeBotInput> commands, IChatSessionService chatSessionService)
        {
            _commands = commands;
            _chatSessionService = chatSessionService;
        }


        public void Start()
        {
            _botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync);
            _logger.Info("Youtube bot was started");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
        {
            try
            {
                await new UserInputCenter(_commands, _chatSessionService, _botClient).ProcessFor(new Message(update.Message.Chat.Id, update.Message.Chat.Username, update.Message.Text));
                _logger.Debug($"Proccessed message from: User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
            }
            catch (NotFoundCommandException)
            {
                _logger.Debug($"Message: '{update.Message.Text}' User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(update.Message.Chat, "Seemds you send me incorrect URL");
            }
            catch (MaxUploadSizeExceededException ex)
            {
                _logger.Debug($"Message: '{update.Message.Text}' User Id: {update.Message.Chat.Id} UserName: @{update.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(update.Message.Chat, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken = default)
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
