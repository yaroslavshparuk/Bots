using Bot.Core.Exceptions;
using Telegram.Bot;
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
            var session = _chatSessionService.GetOrCreate(message.ChatId);
            
            if (message.Text is "Cancel")
            {
                await _client.DeleteMessageAsync(message.ChatId, session.LastReplyId);
                return;
            }

            var request = new UserRequest(session, message, _client);

            foreach (var inputHandler in _inputHandlers)
            {
                if (inputHandler.IsSuitable(request))
                {
                    try
                    {
                        await inputHandler.Handle(request);
                        return;
                    }
                    finally
                    {
                        _chatSessionService.Save(message.ChatId, session);
                    }
                }
            }

            throw new NotFoundCommandException();
        }
    }
}
