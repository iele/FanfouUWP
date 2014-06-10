using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    [DataContract]
    public class Error : Item
    {
        [DataMember(Name = "request", IsRequired = false)]
        public string request { get; set; }
        [DataMember(Name = "error", IsRequired = false)]
        public string error { get; set; }
    }
}
