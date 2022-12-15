using Bot.Core.Abstractions;
using Telegram.Bot;

namespace Bot.Money.Handlers
{
    public class HelpCommand : IMoneyBotInput
    {
        private const string helpResponse = "Всі питання до @shparuk";

        public bool IsExecutable(UserRequest request)
        {
            return request.Message.Text is "/help";
        }

        public async Task Handle(UserRequest request) 
        {
            if (!IsExecutable(request)) { throw new ArgumentException(); }
            await request.Client.SendTextMessageAsync(request.Message.ChatId, helpResponse);
        }
    }
}
