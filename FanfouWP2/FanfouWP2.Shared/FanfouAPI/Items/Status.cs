using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class Status : Item
    {
        public bool is_refresh { get; set; }
        public string created_at { get; set; }
        public string id { get; set; }
        public string rawid { get; set; }

        public string text { get; set; }

        public string source { get; set; }

        public bool truncated { get; set; }

        public string in_reply_to_status_id { get; set; }

        public string in_reply_to_user_id { get; set; }

        public string in_reply_to_screen_name { get; set; }

        public string repost_status_id { get; set; }

        public string repost_status { get; set; }

        public string repost_user_id { get; set; }

        public string repost_screen_name { get; set; }

        public bool favorited { get; set; }

        public string location { get; set; }

        public User user { get; set; }

        public Photo photo { get; set; }
    }
}
