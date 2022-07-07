using Bot.Core.Enums;
namespace Bot.Core.Abstractions
{
    public interface IExportUrl
    {
        string BuildWith(string userSheet, FileType fileType);
    }
}
