namespace Bot.Core.Abstractions
{
    public interface IBotInput
    {
        public bool IsExecutable(UserRequest request);

        public Task Handle(UserRequest request);
    }
}