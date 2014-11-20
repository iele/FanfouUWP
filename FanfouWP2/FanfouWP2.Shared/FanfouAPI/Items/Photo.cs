using System.Runtime.Serialization;

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