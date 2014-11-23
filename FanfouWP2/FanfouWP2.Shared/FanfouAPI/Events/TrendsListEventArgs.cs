using System;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.FanfouAPI.Events
{
    public class TrendsListEventArgs : EventArgs
    {
        public TrendsListEventArgs()
        {
        }

        public TrendsListEventArgs(TrendsList trendsList)
        {
            this.trendsList = trendsList;
        }

        public TrendsList trendsList { get; set; }
    }
}