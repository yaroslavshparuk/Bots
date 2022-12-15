using Bot.Money.Enums;
using Bot.Money.Models;
using Xunit;

namespace Bot.Money.Tests.Models
{
    public class GoogleSpreadSheetsExportUrlTest
    {
        [Fact]
        public void BuildWithTest()
        {
            var exportUrl = new GoogleSpreadSheetsExportUrl();
            Assert.Throws<ArgumentNullException>(() => exportUrl.BuildWith(null, FileType.None));

            var expectedPdf = "https://docs.google.com/spreadsheets/d/123asd/export?format=pdf&id=123asd";
            var actual = exportUrl.BuildWith("123asd", FileType.Pdf);
            Assert.Equal(expectedPdf, actual);

            var expectedXlsx = "https://docs.google.com/spreadsheets/d/asd123/export?format=xlsx&id=asd123";
            actual = exportUrl.BuildWith("asd123", FileType.Xlsx);
            Assert.Equal(expectedXlsx, actual);
        }
    }
}
