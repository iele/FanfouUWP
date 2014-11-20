using System.Runtime.Serialization;

namespace FanfouWP2.FanfouAPI
{
    [DataContract]
    public class Search : Item
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }

        [DataMember(Name = "query", IsRequired = false)]
        public string query { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        public string name { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string created_at { get; set; }
    }
}