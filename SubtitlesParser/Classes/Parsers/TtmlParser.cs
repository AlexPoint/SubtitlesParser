using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SubtitlesParser.Classes.Parsers
{
    class TtmlParser:ISubtitlesParser
    {
        public List<SubtitleItem> ParseStream(Stream xmlStream, Encoding encoding)
        {
            // rewind the stream
            xmlStream.Position = 0;
            var items = new List<SubtitleItem>();

            // parse xml stream
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream);
            
            if (xmlDoc.DocumentElement != null)
            {
                var nodeList = xmlDoc.DocumentElement.SelectNodes("//p");

                if (nodeList != null)
                {
                    for (var i = 0; i < nodeList.Count; i++)
                    {
                        var node = nodeList[i];
                        try
                        {
                            var beginString = node.Attributes["begin"].Value.Replace("t", "");
                            int startTicks = int.Parse(beginString);
                            var endString = node.Attributes["dur"].Value.Replace("t", "");
                            int endTicks = int.Parse(endString);
                            var text = node.InnerText;

                            items.Add(new SubtitleItem()
                            {
                                StartTime = (int)(startTicks / 10000),
                                EndTime = (int)(endTicks / 10000),
                                Lines = new List<string>() { text }
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception raised when parsing xml node {0}: {1}", node, ex);
                        }
                    }  
                }
            }

            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid TTML format, or represents empty subtitles");
            }
        }
    }
}
