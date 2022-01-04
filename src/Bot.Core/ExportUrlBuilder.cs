using Bot.Core.Enums;
using System.Text;

namespace Bot.Core
{
    public class ExportUrlBuilder
    {
        private readonly string _url;

        public ExportUrlBuilder(string url)
        {
            _url = url;
        }

        public string Build(string userSheet, ExportFileType fileType)
        {
            var urlBuilder = new StringBuilder(_url);
            urlBuilder.Append($"{userSheet}/export?format=");
            switch (fileType)
            {
                case ExportFileType.PDF:
                    urlBuilder.Append("pdf");
                    break;
                case ExportFileType.XLSX:
                    urlBuilder.Append("xlsx");
                    break;
            }
            urlBuilder.Append($"&id={userSheet}");
            return urlBuilder.ToString();
        }
    }
}
