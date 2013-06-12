using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitlesParser.Model
{
    public class SubtitleItem
    {

        //Properties------------------------------------------------------------------
        
        //StartTime and EndTime times are in milliseconds
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string Text { get; set; }
        

        //Constructors-----------------------------------------------------------------

        /// <summary>
        /// The empty constructor
        /// </summary>
        public SubtitleItem(){ }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="start">The begin time of the subtitle item in milliseconds</param>
        /// <param name="end">The end time of the subtitle item in milliseconds</param>
        /// <param name="text">The text of the subtitle item</param>
        public SubtitleItem(int start, int end, string text)
        {
            StartTime = start;
            EndTime = end;
            Text = text;
        }


        // Methods --------------------------------------------------------------------------

        /*/// <summary>
        /// Finds the "closest" (in terms of timecodes) item in the collection of items
        /// submitted as parameters.
        /// </summary>
        /// <param name="items">The list of items in which to find the closest of the current SubtitleItem</param>
        /// <returns>The index as int</returns>
        public int FindClosestIndex(List<SubtitleItem> items)
        {
            var minItem = items.OrderBy(i => Math.Pow(i.StartTime - this.StartTime, 2) + Math.Pow(i.EndTime - this.EndTime, 2))
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
        }*/

        
        public override string ToString()
        {
            var startTs = new TimeSpan(0, 0, 0, 0, StartTime);
            var endTs = new TimeSpan(0, 0, 0, 0, EndTime);

            var res = string.Format("{0} --> {1}: {2}", startTs.ToString("G"), endTs.ToString("G"), Text);
            return res;
        }

    }
}