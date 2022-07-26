namespace Bot.Core.Abstractions
{
    public interface IBotInputHandler
    {
        public bool IsSuitable(UserRequest request);

        public Task Handle(UserRequest request);
    }
}