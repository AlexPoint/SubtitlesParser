using System.IO;

namespace SubtitlesParser.Tests
{
    public static class TestFiles
    {
        public static string[] ValidFiles { get; }
        public static string[] InvalidFiles { get; }

        static TestFiles()
        {
            ValidFiles = GetFiles(@"Content\ValidFiles");
            InvalidFiles = GetFiles(@"Content\InvalidFiles");
        }

        private static string[] GetFiles(string relativePath) =>
            Directory.GetFiles(Path.GetFullPath(relativePath));

    }
}
