using Bot.Core.Enums;
using System.Text;

namespace Bot.Money.Models
{
    public class ExportUrl
    {
        private readonly string _url;

        public ExportUrl(string url)
        {
            _url = url;
        }

        public string BuildWith(string userSheet, FileType fileType)
        {
            var builder = new StringBuilder(_url);
            builder.Append($"{userSheet}/export?format=");
            switch (fileType)
            {
                case FileType.PDF:
                    builder.Append("pdf");
                    break;
                case FileType.XLSX:
                    builder.Append("xlsx");
                    break;
            }
            builder.Append($"&id={userSheet}");
            return builder.ToString();
        }
    }
}
