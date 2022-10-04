using Bot.Core.Abstractions;
using Telegram.Bot;

namespace Bot.Money.Handlers
{
    public class HelpCommand : IMoneyBotInputHandler
    {
        private const string helpResponse = "Всі питання до @shparuk";

        public bool IsSuitable(UserRequest request)
        {
            return request.Message.Text is "/help";
        }

        public async Task Handle(UserRequest request) 
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }
            await request.Client.SendTextMessageAsync(request.Message.ChatId, helpResponse);
        }
    }
}
