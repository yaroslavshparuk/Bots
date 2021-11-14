using Bot.Youtube.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using VideoLibrary;

namespace Bot.Youtube.Commands
{
    public class YoutubeVideoUrlToAudioCommand : IYoutubeCommand
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
                var file = new InputOnlineFile(memoryStream, video.FullName.Replace(".mp4", ""));
                await botClient.SendAudioAsync(message.Chat, file);
            }
        }
    }
}
