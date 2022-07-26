namespace Bot.Core.Abstractions
{
    public interface IBotCommand
    {
        public bool CanExecute(UserRequest request);

        public Task Execute(UserRequest request);
    }
}
