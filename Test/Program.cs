using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubtitlesParser.Classes;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new SubtitlesParser.Classes.Parsers.SubParser();
			// Get the memory usage at start
			long initialMemory = GC.GetTotalMemory(true);
			Console.WriteLine($"Initial memory usage: {initialMemory / 1024.0 / 1024.0} MB");

			while (true) 
            {
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
								Console.WriteLine("Parsing of file {0}: SUCCESS ({1} items - {2}% corrupted)",
									fileName, items.Count, (items.Count(it => it.StartTime <= 0 || it.EndTime <= 0) * 100) / items.Count);
								/*foreach (var item in items)
								{
									Console.WriteLine(item);
								}*/
								/*var duplicates =
									items.GroupBy(it => new {it.StartTime, it.EndTime}).Where(grp => grp.Count() > 1);
								Console.WriteLine("{0} duplicate items", duplicates.Count());*/
								Console.WriteLine("----------------");
							}
							else
							{
								Console.WriteLine("Parsing of file {0}: SUCCESS (No items found!)", fileName, items.Count);
							}

						}
						catch (Exception ex)
						{
							Console.WriteLine("Parsing of file {0}: FAILURE\n{1}", fileName, ex);
						}
					}
					Console.WriteLine("----------------------");
				}
				// Get the memory usage after parsing
				Console.WriteLine($"Memory usage after parsing: {GC.GetTotalMemory(true) / 1024.0 / 1024.0} MB");
				// Force GC to run
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				Console.WriteLine($"Memory usage after GC: {GC.GetTotalMemory(true) / 1024.0 / 1024.0} MB");
				Console.ReadLine();
			}
        }

        private static string[] BrowseTestSubtitlesFiles()
        {
            const string subFilesDirectory = @"Content\TestFiles";
            var currentPath = Directory.GetCurrentDirectory();
            var completePath = Path.Combine(currentPath, subFilesDirectory);

            var allFiles = Directory.GetFiles(completePath);
            return allFiles;
        }
    }
}
