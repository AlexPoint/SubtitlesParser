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
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();

            var allFiles = BrowseTestSubtitlesFiles();
            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                using (var fileStream = File.OpenRead(file))
                {
                    try
                    {
                        var mostLikelyFormat = parser.GetMostLikelyFormat(fileName);
                        var items = parser.ParseStream(fileStream, Encoding.UTF8, mostLikelyFormat);
                        if (items.Any())
                        {
                            Console.WriteLine("Parsing of file {0}: SUCCESS ({1} items)", fileName, items.Count);
                            foreach (var item in items)
                            {
                                Console.WriteLine(item);
                            }
                            /*var duplicates =
                                items.GroupBy(it => new {it.StartTime, it.EndTime}).Where(grp => grp.Count() > 1);
                            Console.WriteLine("{0} duplicate items", duplicates.Count());*/
                            Console.WriteLine("----------------");
                        }
                        else
                        {
                            throw new ArgumentException("Not items found!");
                        }
                        
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
