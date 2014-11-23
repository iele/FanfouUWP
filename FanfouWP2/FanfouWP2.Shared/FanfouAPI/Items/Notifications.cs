using System.Runtime.Serialization;

namespace FanfouWP2.FanfouAPI.Items
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