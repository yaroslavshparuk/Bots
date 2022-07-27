using Bot.Core.Enums;
namespace Bot.Core.Abstractions
{
    public interface IExportUrl // TODO: revise the necessity of this in Abstractions
    {
        string BuildWith(string userSheet, FileType fileType);
    }
}
