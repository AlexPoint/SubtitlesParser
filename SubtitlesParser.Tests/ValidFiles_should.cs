using SubtitlesParser.Classes.Parsers;
using System.IO;
using System.Text;
using Xunit;

namespace SubtitlesParser.Tests
{
    public class ValidFiles_should
    {
        SubParser parser = new SubParser();

        [Fact]
        public void Parse_successfully()
        {
            foreach(var filePath in TestFiles.ValidFiles)
            {
                var fileName = Path.GetFileName(filePath);
                using (var fileStream = File.OpenRead(filePath))
                {
                    var mostLikelyFormat = parser.GetMostLikelyFormat(fileName);
                    _ = parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat);
                }
            }
        }
    }
}
