using System.Collections.ObjectModel;
using System.Runtime.Serialization;
namespace FanfouWP2.FanfouAPI.Items
{
    [DataContract]
    public class UserAuth : Item
    {
        [DataMember(Name = "oauthToken", IsRequired = false)]
        public string oauthToken { get; set; }

        [DataMember(Name = "oauthSecret", IsRequired = false)]
        public string oauthSecret { get; set; }

        [DataMember(Name = "username", IsRequired = false)]
        public string username { get; set; }

        [DataMember(Name = "password", IsRequired = false)]
        public string password { get; set; }
    }

    [DataContract]
    public class UserList : Item
    {
        [DataMember(Name = "total_number", IsRequired = false)]
        public int total_number { get; set; }

        [DataMember(Name = "users", IsRequired = false)]
        public ObservableCollection<User> users { get; set; }
    }

    [DataContract]
    public class User : Item
    {
        [DataMember(Name = "id", IsRequired = true)]
        public string id { get; set; }

        [DataMember(Name = "name", IsRequired = false)]
        public string name { get; set; }

        [DataMember(Name = "screen_name", IsRequired = false)]
        public string screen_name { get; set; }

        [DataMember(Name = "location", IsRequired = false)]
        public string location { get; set; }

        [DataMember(Name = "gender", IsRequired = false)]
        public string gender { get; set; }

        [DataMember(Name = "birthday", IsRequired = false)]
        public string birthday { get; set; }

        [DataMember(Name = "description", IsRequired = false)]
        public string description { get; set; }

        [DataMember(Name = "profile_image_url", IsRequired = false)]
        public string profile_image_url { get; set; }

        [DataMember(Name = "profile_image_url_large", IsRequired = false)]
        public string profile_image_url_large { get; set; }

        [DataMember(Name = "url", IsRequired = false)]
        public string url { get; set; }

        [DataMember(Name = "protected", IsRequired = false)]
        public bool @protected { get; set; }

        [DataMember(Name = "followers_count", IsRequired = false)]
        public int followers_count { get; set; }

        [DataMember(Name = "friends_count", IsRequired = false)]
        public int friends_count { get; set; }

        [DataMember(Name = "favourites_count", IsRequired = false)]
        public int favourites_count { get; set; }

        [DataMember(Name = "statuses_count", IsRequired = false)]
        public int statuses_count { get; set; }

        [DataMember(Name = "following", IsRequired = false)]
        public bool following { get; set; }

        [DataMember(Name = "notifications", IsRequired = false)]
        public bool notifications { get; set; }

        [DataMember(Name = "created_at", IsRequired = false)]
        public string created_at { get; set; }

        [DataMember(Name = "utc_offset", IsRequired = false)]
        public long utc_offset { get; set; }

        [DataMember(Name = "profile_background_color", IsRequired = false)]
        public string profile_background_color { get; set; }

        [DataMember(Name = "profile_text_color", IsRequired = false)]
        public string profile_text_color { get; set; }

        [DataMember(Name = "profile_link_color", IsRequired = false)]
        public string profile_link_color { get; set; }

        [DataMember(Name = "profile_sidebar_fill_color", IsRequired = false)]
        public string profile_sidebar_fill_color { get; set; }

        [DataMember(Name = "profile_sidebar_border_color", IsRequired = false)]
        public string profile_sidebar_border_color { get; set; }

        [DataMember(Name = "profile_background_image_url", IsRequired = false)]
        public string profile_background_image_url { get; set; }

        [DataMember(Name = "profile_background_tile", IsRequired = false)]
        public bool profile_background_tile { get; set; }

        [DataMember(Name = "status", IsRequired = false)]
        public Status status { get; set; }

        [DataMember(Name = "auth", IsRequired = false)]
        public UserAuth auth { get; set; }
    }
}