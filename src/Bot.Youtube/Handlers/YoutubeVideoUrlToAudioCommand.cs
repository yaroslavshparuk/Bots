using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Telegram.Bot.Types.InputFiles;
using VideoLibrary;

namespace Bot.Youtube.Handlers
{
    public class YoutubeVideoUrlToAudioCommand : IYoutubeBotInputHandler
    {
        private static Uri _uriResult;
        public bool CanHandle(UserRequest request)
        {
            return Uri.TryCreate(request.Message.Text, UriKind.Absolute, out _uriResult) &&
                                (_uriResult.Scheme == Uri.UriSchemeHttp || _uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public async Task Handle(UserRequest request)
        {
            if (!CanHandle(request)) { throw new ArgumentException(); }

            var video = YouTube.Default.GetVideo(request.Message.Text);
            using (var memoryStream = new MemoryStream(video.GetBytes()))
            {
                if (memoryStream.Capacity > 5e+7) { throw new MaxUploadSizeExceededException("File should not exceed 50 MB"); }
                await request.Client.SendAudioAsync(request.Message.Chat, new InputOnlineFile(memoryStream, video.FullName.Replace(".mp4", "")));
            }
        }
    }
}