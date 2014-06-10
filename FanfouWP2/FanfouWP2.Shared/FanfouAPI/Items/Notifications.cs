using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    [DataContract]
    public class Notifications : Item
    {
        [DataMember(Name = "mentions", IsRequired = false)]
        public int mentions { get; set; }
        [DataMember(Name = "direct_messages", IsRequired = false)]
        public int direct_messages { get; set; }
        [DataMember(Name = "friend_requests", IsRequired = false)]
        public int friend_requests { get; set; }
    }
}
