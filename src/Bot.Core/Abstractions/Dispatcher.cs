using Bot.Core.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Core.Abstractions
{
    public class Dispatcher
    {
        private readonly IEnumerable<IBotInputHandler> _inputHandlers;
        private readonly IChatSessionService _chatSessionService;
        private readonly ITelegramBotClient _client;

        public Dispatcher(IEnumerable<IBotInputHandler> inputHandlers, IChatSessionService chatSessionService, ITelegramBotClient client)
        {
            _inputHandlers = inputHandlers;
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
                return;
            }

            var request = new UserRequest(session, message, _client);

            foreach (var inputHandler in _inputHandlers)
            {
                if (inputHandler.IsSuitable(request))
                {
                    await inputHandler.Handle(request);
                    _chatSessionService.Save(chatId, session);
                    return;
                }
            }


            throw new NotFoundCommandException();
        }
    }
}
