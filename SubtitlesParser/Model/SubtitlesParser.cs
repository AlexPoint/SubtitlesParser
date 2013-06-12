using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using log4net;

namespace SubtitlesParser.Model
{
    public class SubtitlesParser : ISubtitlesParser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (SubtitlesParser));

        // Properties -----------------------------------------------------------------------
        
        private readonly Dictionary<SubtitlesFormat, ISubtitlesParser> _subFormatToParser = new Dictionary<SubtitlesFormat, ISubtitlesParser>
            {
                {SubtitlesFormat.SubRipFormat, new SrtParser()},
                {SubtitlesFormat.SubViewerFormat, new SubParser()}
            };


        // Constructors --------------------------------------------------------------------

        public SubtitlesParser(){}


        // Methods -------------------------------------------------------------------------

        /// <summary>
        /// Gets the most likely format of the subtitle file based on its filename.
        /// Most likely because .sub are sometimes srt files for example.
        /// </summary>
        /// <param name="file">The subtitles file</param>
        /// <returns>The most likely subtitles format</returns>
        /*public SubtitlesFormat GetMostLikelyFormat(HttpPostedFileBase file)
        {
            var fileName = file.FileName;
            var extension = Path.GetExtension(fileName);

            if (extension == ".srt")
            {
                return SubtitlesFormat.Srt;
            }
            else if (extension == ".sub")
            {
                return SubtitlesFormat.Sub;
            }
            else
            {
                // default is srt
                return SubtitlesFormat.Srt;
            }
        }*/

        /// <summary>
        /// Parses a subtitles file stream
        /// </summary>
        /// <param name="stream">The subtitles file stream</param>
        /// <returns>The corresponding list of SubtitleItems</returns>
        public List<SubtitleItem> ParseStream(Stream stream)
        {
            // we default encoding to UTF-8
            return ParseStream(stream, Encoding.UTF8);
        }

        /// <summary>
        /// Parses a subtitle file stream.
        /// We try all the parsers registered in the _subFormatToParser dictionary
        /// </summary>
        /// <param name="stream">The subtitle file stream</param>
        /// <param name="encoding">The stream encoding</param>
        /// <returns>The corresponding list of SubtitleItem, null if parsing failed</returns>
        public List<SubtitleItem> ParseStream(Stream stream, Encoding encoding)
        {
            return ParseStream(stream, encoding, SubtitlesFormat.SubRipFormat);
        }

        /// <summary>
        /// Parses a subtitle file stream.
        /// We try all the parsers registered in the _subFormatToParser dictionary
        /// </summary>
        /// <param name="stream">The subtitle file stream</param>
        /// <param name="encoding">The stream encoding</param>
        /// <param name="subFormat">The preferred subFormat to try first (if we have a clue with the subtitle file name for example)</param>
        /// <returns>The corresponding list of SubtitleItem, null if parsing failed</returns>
        public List<SubtitleItem> ParseStream(Stream stream, Encoding encoding, SubtitlesFormat subFormat)
        {
            var dictionary = _subFormatToParser
                                // start the parsing by the specified format
                                .OrderBy(dic => Math.Abs(String.Compare(dic.Key.Name, subFormat.Name, StringComparison.Ordinal)))
                                .ToDictionary(entry => entry.Key, entry => entry.Value);

            return ParseStream(stream, encoding, dictionary);
        }

        /// <summary>
        /// Parses a subtitle file stream.
        /// We try all the parsers registered in the _subFormatToParser dictionary
        /// </summary>
        /// <param name="stream">The subtitle file stream</param>
        /// <param name="encoding">The stream encoding</param>
        /// <param name="subFormatDictionary">The dictionary of the subtitles parser (ordered) to try</param>
        /// <returns>The corresponding list of SubtitleItem, null if parsing failed</returns>
        public List<SubtitleItem> ParseStream(Stream stream, Encoding encoding, Dictionary<SubtitlesFormat, ISubtitlesParser> subFormatDictionary)
        {
            // test if stream if readable
            if (!stream.CanRead)
            {
                throw new ArgumentException("Cannot parse a non-readable stream");
            }

            // copy the stream if not seekable
            var seekableStream = stream;
            if (!stream.CanSeek)
            {
                seekableStream = StreamHelpers.CopyStream(stream);
            }

            // if dictionary is null, use the default one
            subFormatDictionary = subFormatDictionary ?? _subFormatToParser;

            foreach (var subtitlesParser in subFormatDictionary)
            {
                try
                {
                    var parser = subtitlesParser.Value;
                    var items = parser.ParseStream(seekableStream, encoding);

                    return items;
                }
                catch (Exception ex)
                {
                    // log the first characters
                    LogFirstCharactersOfStream(seekableStream, ex, 500, encoding);
                }
            }

            // all the parsers failed
            Logger.ErrorFormat("All the subtitles parsers failed to parse the following stream:");
            LogFirstCharactersOfStream(stream, new ArgumentException("Wrong subtitle format"),
                                                               500, encoding);
            return null;
        }

        /*public List<SubtitleItem> ParseSubtitleFile(HttpPostedFileBase file, short languageCode)
        {
            var mostLikelyFormat = GetMostLikelyFormat(file);

            Stream copy = new MemoryStream();
            file.InputStream.CopyTo(copy);
            file.InputStream.Position = copy.Position = 0;//reset the position at 0 to use the stream aftewards
            
            return ParseStream(copy, languageCode, mostLikelyFormat);
        }*/
        

        /// <summary>
        /// Logs the first characters of a stream for debug
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="ex">The exception caught when reading the stream</param>
        /// <param name="nbOfCharactersToPrint">The number of caracters to print</param>
        /// <param name="encoding">The stream encoding</param>
        private void LogFirstCharactersOfStream(Stream stream, Exception ex, int nbOfCharactersToPrint, Encoding encoding)
        {
            // print the first 500 characters
            if (stream.CanRead)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                var reader = new StreamReader(stream, encoding, true);

                var buffer = new char[nbOfCharactersToPrint];
                reader.ReadBlock(buffer, 0, nbOfCharactersToPrint);
                Logger.DebugFormat("Parsing of subtitle stream failed: {0}\n" +
                                  "Beginning of sub stream:\n{1}", ex,
                                  string.Join("", buffer));
            }
            else
            {
                Logger.ErrorFormat("Tried to log the first {0} characters of a closed stream", nbOfCharactersToPrint);
            }
        }
    }
}