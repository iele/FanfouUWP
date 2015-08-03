using System.Runtime.Serialization;

namespace FanfouUWP.FanfouAPI.Items
{
    [DataContract]
    public class DirectMessage : Item
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }

        [DataMember(Name = "text", IsRequired = false)]
        public string text { get; set; }

        [DataMember(Name = "sender_id", IsRequired = false)]
        public string sender_id { get; set; }

        [DataMember(Name = "recipient_id", IsRequired = false)]
        public string recipient_id { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string created_at { get; set; }

        [DataMember(Name = "sender_screen_name", IsRequired = false)]
        public string sender_screen_name { get; set; }

        [DataMember(Name = "recipient_screen_name", IsRequired = false)]
        public string recipient_screen_name { get; set; }

        [DataMember(Name = "sender", IsRequired = false)]
        public User sender { get; set; }

        [DataMember(Name = "recipient", IsRequired = false)]
        public User recipient { get; set; }

        [DataMember(Name = "in_reply_to", IsRequired = false)]
        public DirectMessage in_reply_to { get; set; }
    }

    [DataContract]
    public class DirectMessageItem : Item
    {
        [DataMember(Name = "dm", IsRequired = false)]
        public DirectMessage dm { get; set; }

        [DataMember(Name = "otherid", IsRequired = false)]
        public string otherid { get; set; }

        [DataMember(Name = "msg_num", IsRequired = false)]
        public int msg_num { get; set; }

        [DataMember(Name = "new_conv", IsRequired = false)]
        public bool new_conv { get; set; }
    }
}