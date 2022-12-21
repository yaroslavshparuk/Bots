using Telegram.Bot;

namespace Bot.Abstractions.Models
{
    public class UserRequest
    {
        public UserRequest(ChatSession session, Message message, ITelegramBotClient client)
        {
            Session = session;
            Message = message;
            Client = client;
        }

        public ChatSession Session { get; }

        public Message Message { get; }

        public ITelegramBotClient Client { get; }
    }
}