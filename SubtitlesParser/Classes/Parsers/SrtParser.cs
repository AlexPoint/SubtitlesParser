using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitlesParser.Classes.Parsers
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
    public class SrtParser: ISubtitlesParser
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
                var lines = srtSubPart.Split(new string[]{Environment.NewLine}, StringSplitOptions.None).Select(s => s.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();

                var item = new SubtitleItem();
                foreach (var line in lines)
                {
                    if (item.StartTime == 0 && item.EndTime == 0)
                    {
                        // we look for the timecodes first
                        int startTc;
                        int endTc;
                        var success = TryParseTimecodeLine(line, out startTc, out endTc);
                        if (success)
                        {
                            item.StartTime = startTc;
                            item.EndTime = endTc;
                        }
                    }
                    else
                    {
                        // we found the timecode, now we get the text
                        item.Lines.Add(line);
                    }
                }

                if (item.StartTime != 0 || item.EndTime != 0)
                {
                    // parsing succeeded
                    items.Add(item);
                }
                else
                {
                    // parsing failed -> it's the wrong format
                    var msg = string.Format("Failed to parse srt part: {0}. We stop the process", string.Join(Environment.NewLine, lines));
                    throw new FormatException(msg);
                }
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

        private bool TryParseTimecodeLine(string line, out int startTc, out int endTc)
        {
            var parts = line.Split(_delimiters, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                // this is not a timecode line
                startTc = -1;
                endTc = -1;
                return false;
            }
            else
            {
                startTc = ParseSrtTimecode(parts[0]);
                endTc = ParseSrtTimecode(parts[1]);
                return true;
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
            if (match.Success)
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