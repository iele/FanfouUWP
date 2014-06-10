using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    [DataContract]
    public class Photo : Item
    {
        [DataMember(Name = "imageurl", IsRequired = true)]
        public string imageurl { get; set; }
        [DataMember(Name = "thumburl", IsRequired = true)]
        public string thumburl { get; set; }
        [DataMember(Name = "largeurl", IsRequired = true)]
        public string largeurl { get; set; }
    }

}
