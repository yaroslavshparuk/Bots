using Bot.Core.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Core.Abstractions
{
    public abstract class BotDispatcher
    {
        private readonly IEnumerable<IBotInputHandler> _commands;
        private readonly IChatSessionService _chatSessionService;
        private readonly ITelegramBotClient _client;

        protected BotDispatcher(IEnumerable<IBotInputHandler> commands, IChatSessionService chatSessionService, ITelegramBotClient client)
        {
            _commands = commands;
            _chatSessionService = chatSessionService;
            _client = client;
        }

        public async Task Dispatch(Message message)
        {
            var chatId = message.Chat.Id;
            var session = _chatSessionService.Upload(chatId);

            if (message.Text is "Cancel")
            {
                await _client.SendTextMessageAsync(chatId: chatId, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
            }

            var request = new UserRequest(session, message, _client);

            foreach (var command in _commands)
            {
                if (command.IsSuitable(request))
                {
                    await command.Handle(request);
                    _chatSessionService.Save(chatId, session);
                    return;
                }
            }

            throw new NotFoundCommandException();
        }
    }
}
