using SubtitlesParser.Classes.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SubtitlesParser.Tests
{
    public class InvalidFiles_should
    {
        SubParser parser = new SubParser();

        [Fact]
        public void Throw_on_parse()
        {
            foreach (var filePath in TestFiles.InvalidFiles)
            {
                var fileName = Path.GetFileName(filePath);
                using (var fileStream = File.OpenRead(filePath))
                {
                    var mostLikelyFormat = parser.GetMostLikelyFormat(fileName);
                    Assert.Throws<FormatException>(() => parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat));
                }
            }
        }
    }
}
