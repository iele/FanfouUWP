using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{

    public class Search : Item
    {
        public string id { get; set; }
        public string query { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
    }
}
