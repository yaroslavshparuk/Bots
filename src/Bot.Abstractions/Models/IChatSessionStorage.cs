namespace Bot.Abstractions.Models
{
    public interface IChatSessionStorage
    {
        void Load(long id, ChatSession session);

        ChatSession UnloadOrCreate(long id);
    }
}