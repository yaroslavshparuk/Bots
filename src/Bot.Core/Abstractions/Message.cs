namespace Bot.Core.Abstractions
{
    public class Message
    {
        public Message(long chatId, string userName, string text)
        {
            ChatId = chatId;
            UserName = userName;
            Text = text;
        }

        public long ChatId { get; private set; }

        public string UserName { get; private set; }

        public string Text { get; private set; }
    }
}
