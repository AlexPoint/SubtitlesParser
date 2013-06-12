using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitlesParser.Model
{
    public class SubtitleItem
    {

        //Properties------------------------------------------------------------------

        /*[ScriptIgnore] // The JavaScriptSerializer ignores this field.
        public int SubtitleItemId { get; set; }*/

        public double Start { get; set; }
        public double End { get; set; }
        public string Text { get; set; }
        public short Language { get; set; }


        //Constructors-----------------------------------------------------------------

        /// <summary>
        /// The empty constructor
        /// </summary>
        public SubtitleItem(){ }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="subtitleItemId">The id of the SubtitleItem instance</param>
        /// <param name="start">The begin time of the SubtitleItem instance</param>
        /// <param name="end">The end time of the SubtitleItem instance</param>
        /// <param name="text">The text of the SubtitleItem instance</param>
        /// <param name="language">The lanaguage of the SubtitleItem instance</param>
        public SubtitleItem(/*int subtitleItemId, */double start, double end, string text, short language)
        {
            /*SubtitleItemId = subtitleItemId;*/
            Start = start;
            End = end;
            Text = text;
            Language = language;
        }

        /// <summary>
        /// A constructor that doesn't specify the subtitleItem's language
        /// </summary>
        /// <param name="subtitleItemId">The id of the SubtitleItem instance</param>
        /// <param name="start">The begin time of the SubtitleItem instance</param>
        /// <param name="end">The end time of the SubtitleItem instance</param>
        /// <param name="text">The text of the SubtitleItem instance</param>
        public SubtitleItem(int subtitleItemId, double start, double end, string text)
        {
            /*SubtitleItemId = subtitleItemId;*/
            Start = start;
            End = end;
            Text = text;
        }

        /// <summary>
        /// Finds the "closest" (in terms of timecodes) item in the collection of items
        /// submitted as parameters.
        /// </summary>
        /// <param name="items">The list of items in which to find the closest of the current SubtitleItem</param>
        /// <returns>The index as int</returns>
        public int FindClosestIndex(List<SubtitleItem> items)
        {
            var minItem = items.OrderBy(i => Math.Pow(i.Start - this.Start, 2) + Math.Pow(i.End - this.End, 2))
                .FirstOrDefault();
            if (minItem == null)
            {
                // happens only in the collection items is empty
                return 0;
            }
            else
            {
                return items.IndexOf(minItem);
            }
        }

        // Temporary (for debug) -----------------------------------------------------------------------

        public override string ToString()
        {
            string res = "";
            res += Start + " --> " + End + " (" + (End - Start) + ")<br/>";
            res += Text;
            return res;
        }

    }
}