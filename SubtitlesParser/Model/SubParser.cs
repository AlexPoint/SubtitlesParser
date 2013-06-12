using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using log4net;

namespace SubtitlesParser.Model
{
    /// <summary>
    /// Parser for .sub subtitles files
    /// 
    /// A .sub file looks like this:
    /// {1}{1}29.970
    /// {0}{180}PIRATES OF THE CARIBBEAN|English subtitlez by tHe.b0dY
    /// {509}{629}Drink up me 'earties yo ho!
    /// {635}{755}We kidnap and ravage and don't give a hoot.
    /// 
    /// We need the video frame rate to extract .sub files -> careful when using it
    /// </summary>
    public class SubParser : ISubtitlesParser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SubParser));

        // Properties -----------------------------------------------------------------------

        private static SubParser _instance;
        public static SubParser Instance
        {
            get { return _instance ?? (_instance = new SubParser()); }
        }

        private const short DefaultFrameRate = 25;


        // Constructors --------------------------------------------------------------------

        private SubParser(){}


        // Methods -------------------------------------------------------------------------

        public List<SubtitleItem> ParseStream(Stream subStream, short languageCode)
        {
            // test if stream if readable and seekable (just a check, should be good)
            if (!subStream.CanRead || !subStream.CanSeek)
            {
                Logger.ErrorFormat("Stream must be seekable and readable in specific parser. " +
                                   "Operation interrupted; isSeekable: {0} - isReadable: {1}", 
                                   subStream.CanSeek, subStream.CanSeek);
                return null;
            }

            // seek the beginning of the stream
            subStream.Position = 0;

            var items = new List<SubtitleItem>();

            //TODO implement encoding preference
            //var encoding = Language.GetPreferredEncoding(languageCode);
            var reader = new StreamReader(subStream, Encoding.UTF8, true);

            var line = reader.ReadLine();
            if (line != null)
            {
                float frameRate;
                // try to extract the framerate from the first line
                var firstItem = ParseLine(line, DefaultFrameRate);
                var success = TryExtractFrameRate(firstItem.Text, out frameRate);
                if (!success)
                {
                    Logger.WarnFormat("Couldn't extract frame rate of sub file with first line {0}. " +
                                      "We use the default frame rate: {1}", line, DefaultFrameRate);
                    frameRate = DefaultFrameRate;
                    
                    // treat it as a regular line
                    firstItem.Language = languageCode;
                    items.Add(firstItem);
                }

                // parse other lines
                line = reader.ReadLine();
                while (line != null)
                {
                    var item = ParseLine(line, frameRate);
                    item.Language = languageCode;

                    items.Add(item);
                    line = reader.ReadLine();
                }
            }

            return items;
        }

        /// <summary>
        /// Parses one line of the .sub file
        /// 
        /// ex:
        /// {0}{180}PIRATES OF THE CARIBBEAN|English subtitlez by tHe.b0dY
        /// </summary>
        /// <param name="line">The .sub file line</param>
        /// <param name="frameRate">The frame rate with which the .sub file was created</param>
        /// <returns>The corresponding SubtitleItem</returns>
        private SubtitleItem ParseLine(string line, float frameRate)
        {
            const string regex = @"{(\d+)}{(\d+)}({.*})?(.*)";
            var match = Regex.Match(line, regex);
            if (match.Success && match.Groups.Count > 2)
            {
                var startFrame = match.Groups[1].Value;
                var start = double.Parse(startFrame) / frameRate;
                var endTime = match.Groups[2].Value;
                var end = double.Parse(endTime) / frameRate;
                var text = match.Groups[match.Groups.Count - 1].Value;
                var item = new SubtitleItem
                    {
                        Text = text,
                        Start = start,
                        End = end
                    };

                return item;
            }
            else
            {
                var message = string.Format("The subtitle file line {0} is " +
                                            "not in the .sub format. We stop the process.", line);
                Logger.Debug(message);
                throw new InvalidDataException(message);
            }
        }

        /// <summary>
        /// Tries to extract the frame rate from a subtitle file line.
        /// 
        /// Supported formats are:
        /// - {x}{y}25
        /// - {x}{y}{...}23.976
        /// </summary>
        /// <param name="text">The subtitle file line</param>
        /// <param name="frameRate">The frame rate if we can parse it</param>
        /// <returns>True if the parsing was successful, false otherwise</returns>
        private bool TryExtractFrameRate(string text, out float frameRate)
        {
            var success = float.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out frameRate);
            return success;
        }

    }
}