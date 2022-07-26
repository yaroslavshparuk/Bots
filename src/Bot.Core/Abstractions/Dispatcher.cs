using Bot.Core.Exceptions;

namespace Bot.Core.Abstractions
{
    public class Dispatcher
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public Dispatcher(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        public async Task Dispatch(UserRequest request)
        {
            foreach (var command in _commands)
            {
                if (command.CanExecute(request))
                {
                    await command.Execute(request);
                }
            }

            throw new NotFoundCommandException();
        }
    }
}
