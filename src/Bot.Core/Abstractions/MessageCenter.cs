using Bot.Core.Exceptions;
using Telegram.Bot;

namespace Bot.Core.Abstractions
{
    public class UserInputCenter
    {
        private readonly IEnumerable<IBotInput> _inputHandlers;
        private readonly IChatSessionService _chatSessionService;
        private readonly ITelegramBotClient _client;

        public UserInputCenter(IEnumerable<IBotInput> inputHandlers, IChatSessionService chatSessionService, ITelegramBotClient client)
        {
            _inputHandlers = inputHandlers;
            _chatSessionService = chatSessionService;
            _client = client;
        }

        public async Task ProcessFor(Message message)
        {
            var session = _chatSessionService.TakeOrCreate(message.ChatId);

            if (message.Text is "Відмінити")
            {
                await _client.DeleteMessageAsync(message.ChatId, session.LastReplyId);
                return;
            }

            var request = new UserRequest(session, message, _client);

            foreach (var inputHandler in _inputHandlers)
            {
                if (inputHandler.IsExecutable(request))
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
