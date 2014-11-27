using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SubtitlesParser.Classes.Parsers
{
    public class TtmlParser:ISubtitlesParser
    {
        public List<SubtitleItem> ParseStream(Stream xmlStream, Encoding encoding)
        {
            // rewind the stream
            xmlStream.Position = 0;
            var items = new List<SubtitleItem>();

            // parse xml stream
            var xElement = XElement.Load(xmlStream);
            XNamespace tt = xElement.GetNamespaceOfPrefix("tt");

            if (xElement != null)
            {
                var nodeList = xElement.Descendants(tt + "p").ToList();

                if (nodeList != null)
                {
                    for (var i = 0; i < nodeList.Count; i++)
                    {
                        var node = nodeList[i];
                        try
                        {
                            var reader = node.CreateReader();
                            reader.MoveToContent();
                            var beginString = node.Attribute("begin").Value.Replace("t", "");
                            long startTicks = long.Parse(beginString);
                            var endString = node.Attribute("end").Value.Replace("t", "");
                            long endTicks = long.Parse(endString);
                            var text = reader.ReadInnerXml().Replace("<tt:", "<").Replace("</tt:", "</").Replace(string.Format(@" xmlns:tt=""{0}""", tt), "");

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
