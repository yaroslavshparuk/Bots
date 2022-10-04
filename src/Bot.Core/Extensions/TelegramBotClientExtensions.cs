using Telegram.Bot;

namespace Bot.Core.Extensions
{
    public static class TelegramBotClientExtensions
    {
        public static async Task SendAndDeleteTextMessageAfterDelay(this TelegramBotClient client, long chatId, string message, TimeSpan delay)
        {
            var reply = await client.SendTextMessageAsync(chatId, message);
            await Task.Delay(delay);
            await client.DeleteMessageAsync(chatId, reply.MessageId);
        }
    }
}
