using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class Photo:Item
    {
        public string imageurl { get; set; }
        public string thumburl { get; set; }
        public string largeurl { get; set; }
    }

}
