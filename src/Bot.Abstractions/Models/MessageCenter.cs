using Bot.Abstractions.Exceptions;
using Telegram.Bot;

namespace Bot.Abstractions.Models
{
    public class UserInputCenter
    {
        private readonly IEnumerable<IBotInput> _inputHandlers;
        private readonly IChatSessionStorage _chatSessionService;
        private readonly ITelegramBotClient _client;

        public UserInputCenter(IEnumerable<IBotInput> inputHandlers, IChatSessionStorage chatSessionService, ITelegramBotClient client)
        {
            _inputHandlers = inputHandlers;
            _chatSessionService = chatSessionService;
            _client = client;
        }

        public async Task ProcessFor(Message message)
        {
            var session = _chatSessionService.UnloadOrCreate(message.ChatId);

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
                        _chatSessionService.Load(message.ChatId, session);
                    }
                }
            }

            throw new NotFoundCommandException();
        }
    }
}