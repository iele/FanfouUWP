using System;
using System.Runtime.Serialization;

namespace FanfouUWP.FanfouAPI.Items
{
    [DataContract]
    public class Status : Item
    {
        [DataMember(Name = "is_refresh", IsRequired = false)]
        public bool is_refresh { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string created_at { get; set; }

        [DataMember(Name = "id", IsRequired = false)]
        public string id { get; set; }

        [DataMember(Name = "rawid", IsRequired = false)]
        public UInt64 rawid { get; set; }

        [DataMember(Name = "text", IsRequired = false)]
        public string text { get; set; }

        [DataMember(Name = "source", IsRequired = false)]
        public string source { get; set; }

        [DataMember(Name = "truncated", IsRequired = false)]
        public bool truncated { get; set; }

        [DataMember(Name = "in_reply_to_status_id", IsRequired = false)]
        public string in_reply_to_status_id { get; set; }

        [DataMember(Name = "in_reply_to_user_id", IsRequired = false)]
        public string in_reply_to_user_id { get; set; }

        [DataMember(Name = "in_reply_to_screen_name", IsRequired = false)]
        public string in_reply_to_screen_name { get; set; }

        [DataMember(Name = "repost_status_id", IsRequired = false)]
        public string repost_status_id { get; set; }

        [DataMember(Name = "repost_status", IsRequired = false)]
        public Status repost_status { get; set; }

        [DataMember(Name = "repost_user_id", IsRequired = false)]
        public string repost_user_id { get; set; }

        [DataMember(Name = "repost_screen_name", IsRequired = false)]
        public string repost_screen_name { get; set; }

        [DataMember(Name = "favorited", IsRequired = false)]
        public bool favorited { get; set; }

        [DataMember(Name = "location", IsRequired = false)]
        public string location { get; set; }

        [DataMember(Name = "user", IsRequired = false)]
        public User user { get; set; }

        [DataMember(Name = "photo", IsRequired = false)]
        public Photo photo { get; set; }
    }
}