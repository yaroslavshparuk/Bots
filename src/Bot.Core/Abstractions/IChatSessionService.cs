namespace Bot.Core.Abstractions
{
    public interface IChatSessionService
    {
        void Save(long id, ChatSession session);

        ChatSession GetOrCreate(long id);
    }
}
