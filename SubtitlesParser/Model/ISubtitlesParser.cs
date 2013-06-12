using System.Collections.Generic;
using System.IO;

namespace SubtitlesParser.Model
{
    /// <summary>
    /// Interface specifying the required method for a SubtitlesParser.
    /// </summary>
    public interface ISubtitlesParser
    {
        
        /// <summary>
        /// Parses a subtitles file stream in a list of SubtitleItem
        /// </summary>
        /// <param name="stream">The subtitles file stream to parse</param>
        /// <param name="languageCode">The language code of the subtitles file</param>
        /// <returns>The corresponding list of SubtitleItems</returns>
        List<SubtitleItem> ParseStream(Stream stream, short languageCode);
        
    }
}