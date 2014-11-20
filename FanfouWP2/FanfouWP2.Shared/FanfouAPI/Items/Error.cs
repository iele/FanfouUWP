using System.Runtime.Serialization;

namespace FanfouWP2.FanfouAPI
{
    [DataContract]
    public class Error : Item
    {
        [DataMember(Name = "request", IsRequired = false)]
        public string request { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public string error { get; set; }
    }
}