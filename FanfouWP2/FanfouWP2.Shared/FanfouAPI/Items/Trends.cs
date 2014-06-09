using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{

    public class Trends : Item
    {
        public string name { get; set; }
        public string query { get; set; }
        public string url { get; set; }
    }
    public class TrendsList : Item
    {
        public string as_of { get; set; }
        public ObservableCollection<Trends> trends { get; set; }
    }

}
