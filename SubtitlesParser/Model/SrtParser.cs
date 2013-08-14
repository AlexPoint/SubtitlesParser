using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitlesParser.Model
{
    /// <summary>
    /// Parser for the .srt subtitles files
    /// 
    /// A .srt file looks like:
    /// 1
    /// 00:00:10,500 --> 00:00:13,000
    /// Elephant's Dream
    ///
    /// 2
    /// 00:00:15,000 --> 00:00:18,000
    /// At the left we can see...[12]
    /// </summary>
    internal class SrtParser: ISubtitlesParser
    {
        
        // Properties -----------------------------------------------------------------------

        private readonly string[] _delimiters = new string[] { "-->" , "- >", "->" };
        

        // Constructors --------------------------------------------------------------------

        public SrtParser(){}


        // Methods -------------------------------------------------------------------------

        public List<SubtitleItem> ParseStream(Stream srtStream, Encoding encoding)
        {
            // test if stream if readable and seekable (just a check, should be good)
            if (!srtStream.CanRead || !srtStream.CanSeek)
            {
                var message = string.Format("Stream must be seekable and readable in a subtitles parser. " +
                                   "Operation interrupted; isSeekable: {0} - isReadable: {1}",
                                   srtStream.CanSeek, srtStream.CanSeek);
                throw new ArgumentException(message);
            }

            // seek the beginning of the stream
            srtStream.Position = 0;

            var reader = new StreamReader(srtStream, encoding, true);

            var items = new List<SubtitleItem>();
            var srtSubParts = GetSrtSubTitleParts(reader);
            foreach (var srtSubPart in srtSubParts)
            {
                var lines = srtSubPart.Split(new string[]{Environment.NewLine}, StringSplitOptions.None).Select(s => s.Trim()).ToList();
                if (lines.Count < 3)
                {
                    Console.WriteLine("Srt part {0} could not be parsed -> we skip it.", srtSubPart);
                    continue;
                }

                // lines[0] is the index -> useless
                // timecode part
                var timeCodeLine = lines[1];
                var timeCodeParts = timeCodeLine.Split(_delimiters, StringSplitOptions.None);
                if (timeCodeParts.Length != 2)
                {
                    var msg = string.Format("Timecode line {0} in {1} could not be parsed " +
                                            "to retrieve item start and end timecodes", timeCodeLine, srtSubPart);
                    throw new IndexOutOfRangeException(msg);
                }
                var start = ParseSrtTimecode(timeCodeParts[0]);
                if (start == -1)
                {
                    var msg = string.Format("Failed to parse timecode {0} in {1}. Timecode is in the wrong format.", timeCodeParts[0], srtSubPart);
                    throw new FormatException(msg);
                }
                var end = ParseSrtTimecode(timeCodeParts[1]);
                if (end == -1)
                {
                    var msg = string.Format("Failed to parse timecode {0} in {1}. Timecode is in the wrong format.", timeCodeParts[1], srtSubPart);
                    throw new FormatException(msg);
                }

                // take only the non-empty lines
                var textLines = lines.Skip(2).Where(l => !string.IsNullOrEmpty(l)).ToList();
                
                items.Add(new SubtitleItem
                    {
                        StartTime = start,
                        EndTime = end,
                        Lines = textLines
                    });
            }
            return items;
        }
        
        /// <summary>
        /// Enumerates the subtitle parts in a srt file based on the standard line break observed between them. 
        /// A srt subtitle part is in the form:
        /// 
        /// 1
        /// 00:00:20,000 --> 00:00:24,400
        /// Altocumulus clouds occur between six thousand
        /// 
        /// </summary>
        /// <param name="reader">The textreader associated with the srt file</param>
        /// <returns>An IEnumerable(string) object containing all the subtitle parts</returns>
        private IEnumerable<string> GetSrtSubTitleParts(TextReader reader)
        {
            string line;
            var sb = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    yield return sb.ToString().TrimEnd();
                    sb = new StringBuilder();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        /// <summary>
        /// Takes an SRT timecode as a string and parses it into a double (in seconds). A SRT timecode reads as follows: 
        /// 00:00:20,000
        /// </summary>
        /// <param name="s">The timecode to parse</param>
        /// <returns>The parsed timecode as a TimeSpan instance. If the parsing was unsuccessful, -1 is returned (subtitles should never show)</returns>
        private int ParseSrtTimecode(string s)
        {
            TimeSpan result;
            var match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+,[0-9]+");
            if (match != null)
            {
                s = match.Value;
                if (TimeSpan.TryParse(s.Replace(',', '.'), out result))
                {
                    var nbOfMs = (int)result.TotalMilliseconds;
                    return nbOfMs;
                }
            }
            return -1;
        }

    }
}