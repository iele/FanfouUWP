using System;

namespace FanfouWP2.FanfouAPI
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