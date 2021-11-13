using Bot.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Services.Commands
{
    public class TurnOffCommand : ICommand
    {
        private const string NAME = "fuck off";

        public bool CanExecute(Message message)
        {
            return message.Text == NAME ? true : false;
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            if (message.Text == "fuck off")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Turning down...", ParseMode.Default, null, false, false, 0);
                botClient.StopReceiving();
                Console.WriteLine($"{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}: {message.Chat.Id} @{message.Chat.Username} turned bot off");
            }
        }
    }
}
