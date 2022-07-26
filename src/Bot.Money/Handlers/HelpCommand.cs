using Bot.Core.Abstractions;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Handlers
{
    public class HelpCommand : IMoneyBotInputHandler
    {
        private const string helpResponse = "It's beta version. To get any info - write @shparuk";

        public bool IsSuitable(UserRequest request)
        {
            return request.Message.Text is "/help";
        }

        public async Task Handle(UserRequest request) 
        {
            if (!IsSuitable(request)) { throw new ArgumentException(); }
            await request.Client.SendTextMessageAsync(request.Message.Chat, helpResponse, ParseMode.Default, false, false, 0);
        }
    }
}
