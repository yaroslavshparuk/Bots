using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Core.Extension;
using Bot.Youtube.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Youtube.Implementation
{
    public class YoutubeBot : IBot
    {
        private readonly IEnumerable<IYoutubeCommand> _commands;
        private ITelegramBotClient _botClient;
        public YoutubeBot(IEnumerable<IYoutubeCommand> commands)
        {
            _commands = commands;
        }

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            _botClient = new TelegramBotClient(ConfigurationManager.AppSettings["youtube_bot_token"]);
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
                await _commands.GetCommandToExecute(e.Message).Execute(e.Message, _botClient);
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
