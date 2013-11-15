using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new SubtitlesParser.Model.SubtitlesParser();

            var allFiles = BrowseTestSubtitlesFiles();
            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                using (var fileStream = File.OpenRead(file))
                {
                    try
                    {
                        var items = parser.ParseStream(fileStream);
                        Console.WriteLine("Parsing of file {0}: SUCCESS", fileName);
                        Console.WriteLine();
                        Console.WriteLine(string.Join(Environment.NewLine, items.Take(5)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parsing of file {0}: FAILURE\n{1}", fileName, ex);
                    }
                }
                Console.WriteLine("----------------------");
            }

            Console.ReadLine();
        }

        private static string[] BrowseTestSubtitlesFiles()
        {
            const string subFilesDirectory = @"Content\TestFiles";
            var currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var completePath = Path.Combine(currentPath, "..", "..", subFilesDirectory);

            var allFiles = Directory.GetFiles(completePath);
            return allFiles;
        }
    }
}
