using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class UserList : Item
    {
        public int total_number { get; set; }
        public ObservableCollection<User> users { get; set; }
    }
    public class User : Item
    {

        public string id { get; set; }

        public string name { get; set; }

        public string screen_name { get; set; }

        public string location { get; set; }

        public string gender { get; set; }

        public string birthday { get; set; }

        public string description { get; set; }

        public string profile_image_url { get; set; }

        public string profile_image_url_large { get; set; }

        public string url { get; set; }

        public bool @protected { get; set; }

        public int followers_count { get; set; }

        public int friends_count { get; set; }

        public int favourites_count { get; set; }

        public int statuses_count { get; set; }

        public bool following { get; set; }

        public bool notifications { get; set; }

        public string created_at { get; set; }

        public long utc_offset { get; set; }

        public string profile_background_color { get; set; }

        public string profile_text_color { get; set; }

        public string profile_link_color { get; set; }

        public string profile_sidebar_fill_color { get; set; }

        public string profile_sidebar_border_color { get; set; }

        public string profile_background_image_url { get; set; }

        public bool profile_background_tile { get; set; }

        public Status status { get; set; }

        public string oauthToken { get; set; }
        public string oauthSecret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}