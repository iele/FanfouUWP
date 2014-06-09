using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class Notifications : Item
    {
        public int mentions { get; set; }
        public int direct_messages { get; set; }
        public int friend_requests { get; set; }
    }
}
