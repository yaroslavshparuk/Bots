namespace Bot.Core.Abstractions
{
    public interface IBotInputHandler
    {
        public bool CanHandle(UserRequest request);

        public Task Handle(UserRequest request);
    }
}
