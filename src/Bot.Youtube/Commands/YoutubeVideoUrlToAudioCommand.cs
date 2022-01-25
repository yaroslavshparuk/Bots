using Bot.Core.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using VideoLibrary;

namespace Bot.Youtube.Commands
{
    public class YoutubeVideoUrlToAudioCommand : IYoutubeBotCommand
    {
        private static Uri _uriResult;
        public bool CanExecute(Message message)
        {
            return Uri.TryCreate(message.Text, UriKind.Absolute, out _uriResult) &&
                                (_uriResult.Scheme == Uri.UriSchemeHttp || _uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public async Task Execute(Message message, ITelegramBotClient botClient)
        {
            var video = YouTube.Default.GetVideo(message.Text);
            using (var memoryStream = new MemoryStream(video.GetBytes()))
            {
                if (memoryStream.Capacity > 5e+7) { throw new MaxUploadSizeExceededException("File should not exceed 50 MB"); }
                await botClient.SendAudioAsync(message.Chat, new InputOnlineFile(memoryStream, video.FullName.Replace(".mp4", "")));
            }
        }
    }
}
