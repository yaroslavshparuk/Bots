using Bot.Core.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Core.Abstractions
{
    public class Dispatcher
    {
        private readonly IEnumerable<IBotInputHandler> _commands;
        private readonly IChatSessionService _chatSessionService;

        public Dispatcher(IEnumerable<IBotInputHandler> commands, IChatSessionService chatSessionService)
        {
            _commands = commands;
            _chatSessionService = chatSessionService;
        }

        public async Task Dispatch(Message message, ITelegramBotClient client)
        {
            var chatId = message.Chat.Id;

            if (message.Text is "Cancel")
            {
                await client.SendTextMessageAsync(chatId: chatId, text: "Canceled", replyMarkup: new ReplyKeyboardRemove());
            }

            var session = _chatSessionService.Upload(chatId);
            var request = new UserRequest(session, message, client);

            foreach (var command in _commands)
            {
                if (command.CanHandle(request))
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
