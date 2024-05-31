using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesParser.Classes.Parsers
{
    /// <summary>
    /// Interface specifying the required method for a SubParser.
    /// </summary>
    public interface ISubtitlesParser
    {
        /// <summary>
        /// Parses a subtitles file stream in a list of SubtitleItem
        /// </summary>
        /// <param name="stream">The subtitles file stream to parse</param>
        /// <param name="encoding">The stream encoding (if known)</param>
        /// <returns>The corresponding list of SubtitleItems</returns>
        List<SubtitleItem> ParseStream(Stream stream, Encoding encoding);

        // /// <summary>
        // /// Parses a subtitles file stream in a list of SubtitleItem
        // /// </summary>
        // /// <param name="stream">The subtitles file stream to parse</param>
        // /// <param name="encoding">The stream encoding (if known)</param>
        // /// <returns>The corresponding list of SubtitleItems</returns>
        // Task<List<SubtitleItem>> ParseStreamAsync(Stream stream, Encoding encoding);
    }
}