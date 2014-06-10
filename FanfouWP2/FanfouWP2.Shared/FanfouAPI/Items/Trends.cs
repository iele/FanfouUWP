using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{

    [DataContract]
    public class Trends : Item
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string name { get; set; }
        [DataMember(Name = "query", IsRequired = false)]
        public string query { get; set; }
        [DataMember(Name = "url", IsRequired = false)]
        public string url { get; set; }
    }
    [DataContract]
    public class TrendsList : Item
    {
        [DataMember(Name = "as_of", IsRequired = true)]
        public string as_of { get; set; }
        [DataMember(Name = "trends", IsRequired = true)]
        public ObservableCollection<Trends> trends { get; set; }
    }

}
