namespace Bot.Core.Abstractions
{
    public interface IChatSessionService
    {
        void Save(ChatSession session);

        ChatSession Upload(long chatId);
    }
}
