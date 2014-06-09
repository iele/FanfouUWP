using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class TrendsListEventArgs : EventArgs
    {
        public TrendsList trendsList { get; set; }

        public TrendsListEventArgs() { }
        public TrendsListEventArgs(TrendsList trendsList)
        {
            this.trendsList = trendsList;        
        }
    }
}
