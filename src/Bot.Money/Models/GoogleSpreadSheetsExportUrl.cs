using Bot.Money.Enums;
using System.Text;

namespace Bot.Money.Models
{
    public class GoogleSpreadSheetsExportUrl
    {
        private const string _url = "https://docs.google.com/spreadsheets/d/";

        public string BuildWith(string userSheet, FileType fileType)
        {
            if (userSheet == null || fileType == FileType.None) throw new ArgumentNullException();

            var builder = new StringBuilder(_url);
            builder.Append($"{userSheet}/export?format=");
            switch (fileType)
            {
                case FileType.Pdf:
                    builder.Append("pdf");
                    break;
                case FileType.Xlsx:
                    builder.Append("xlsx");
                    break;
            }
            builder.Append($"&id={userSheet}");
            return builder.ToString();
        }
    }
}
