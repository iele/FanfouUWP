using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class DirectMessage : Item
    {
        public string id { get; set; }
        public string text { get; set; }
        public string sender_id { get; set; }
        public string recipient_id { get; set; }
        public string created_at { get; set; }
        public string sender_screen_name { get; set; }
        public string recipient_screen_name { get; set; }
        public User sender { get; set; }
        public User recipient { get; set; }

        public DirectMessage in_reply_to { get; set; }
    }

    public class DirectMessageItem : Item
    {
        public DirectMessage dm { get; set; }
        public string otherid { get; set; }
        public int msg_num { get; set; }
        public bool new_conv { get; set; }
    }
}
