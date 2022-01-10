using Bot.Core.Exceptions;
using Bot.Money.Interfaces;
using Bot.Money.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Money.Commands
{
    public class StopCommand : IMoneyCommand
    {
        private const string NAME = "/stop";
        private readonly IUserDataRepository _userDataRepository;

        public StopCommand(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        public bool CanExecute(Message message)
        {
            return message.Text == NAME;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (_userDataRepository.IsOwner(message.Chat.Id))
            {
                var response = string.Empty;

                if (botClient.IsReceiving)
                {
                    botClient.StopReceiving();
                    response = "Bot was stopped";
                }
                else
                {
                    response = "Bot is already stopped";
                }

                await botClient.SendTextMessageAsync(message.Chat, response);
            }
            else
            {
                throw new NotFoundCommandException();
            }
        }
    }
}
