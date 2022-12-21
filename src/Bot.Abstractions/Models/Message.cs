namespace Bot.Abstractions.Models
{
    public class Message
    {
        public Message(long chatId, string userName, string text)
        {
            ChatId = chatId;
            UserName = userName;
            Text = text;
        }

        public long ChatId { get; }

        public string UserName { get; }

        public string Text { get; }
    }
}