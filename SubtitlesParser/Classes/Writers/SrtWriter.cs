using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SubtitlesParser.Classes.Writers
{
    /// <summary>
    /// A writer for the SubRip subtitles format.
    /// See https://en.wikipedia.org/wiki/SubRip for complete explanations.
    /// Example output:
    /// 1
    /// 00:18:03,87 --> 00:18:04,23
    /// Oh?
    /// 
    /// 2
    /// 00:18:05,19 --> 00:18:05,90
    /// What was that?
    /// </summary>
    public class SrtWriter : ISubtitlesWriter
    {
        /// <summary>
        /// Converts a subtitle item into the lines for an SRT subtitle entry
        /// </summary>
        /// <param name="subtitleItem">The SubtitleItem to convert</param>
        /// <param name="subtitleEntryNumber">The subtitle number for the entry (increments sequentially from 1)</param>
        /// <returns>A list of strings to write as an SRT subtitle entry</returns>
        private IEnumerable<string> SubtitleItemToSubtitleEntry(SubtitleItem subtitleItem, int subtitleEntryNumber)
        {
            // take the start and end timestamps and format it as a timecode line
            string formatTimecodeLine()
            {
                TimeSpan start = TimeSpan.FromMilliseconds(subtitleItem.StartTime);
                TimeSpan end = TimeSpan.FromMilliseconds(subtitleItem.EndTime);
                return $"{start:hh\\:mm\\:ss\\,ff} --> {end:hh\\:mm\\:ss\\,ff}";
            }

            List<string> lines = new List<string>(); 
            lines.Add(subtitleEntryNumber.ToString());
            lines.Add(formatTimecodeLine());
            lines.AddRange(subtitleItem.Lines);

            return lines;
        }

        /// <summary>
        /// Write a list of subtitle items to a stream in the SubRip (SRT) format synchronously 
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="subtitleItems">The subtitle items to write</param>
        public void WriteStream(Stream stream, IEnumerable<SubtitleItem> subtitleItems)
        {
            using TextWriter writer = new StreamWriter(stream);

            List<SubtitleItem> items = subtitleItems.ToList(); // avoid multiple enumeration since we're using a for instead of foreach
            for (int i = 0; i < items.Count; i++)
            {
                SubtitleItem subtitleItem = items[i];
                IEnumerable<string> lines = SubtitleItemToSubtitleEntry(subtitleItem, i + 1); // add one because subtitle entry numbers start at 1 instead of 0
                foreach (string line in lines)
                    writer.WriteLine(line);

                writer.WriteLine(); // empty line between subtitle entries
            }
        }

        /// <summary>
        /// Write a list of subtitle items to a stream in the SubRip (SRT) format asynchronously 
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="subtitleItems">The subtitle items to write</param>
        public async Task WriteStreamAsync(Stream stream, IEnumerable<SubtitleItem> subtitleItems)
        {
            await using TextWriter writer = new StreamWriter(stream);

            List<SubtitleItem> items = subtitleItems.ToList(); // avoid multiple enumeration since we're using a for instead of foreach
            for (int i = 0; i < items.Count; i++)
            {
                SubtitleItem subtitleItem = items[i];
                IEnumerable<string> lines = SubtitleItemToSubtitleEntry(subtitleItem, i + 1); // add one because subtitle entry numbers start at 1 instead of 0
                foreach (string line in lines)
                    await writer.WriteLineAsync(line);

                await writer.WriteLineAsync(); // empty line between subtitle entries
            }
        }
    }
}
