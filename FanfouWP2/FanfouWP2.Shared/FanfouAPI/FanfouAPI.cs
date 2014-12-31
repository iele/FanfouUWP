using System;
using System.Collections.Generic;
using Windows.Storage;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.Utils;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace FanfouWP2.FanfouAPI
{
    public class FanfouAPI
    {
        private static FanfouAPI instance;
        public User currentUser;

        public string oauthSecret;
        public string oauthToken;

        public string password;
        public string username;

        public static FanfouAPI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FanfouAPI();
                }
                return instance;
            }
            set { instance = value; }
        }

        public void setUser(User user)
        {
            oauthToken = user.auth.oauthToken;
            oauthSecret = user.auth.oauthSecret;
            username = user.auth.username;
            password = user.auth.password;

            this.currentUser = user;
        }

        private RestClient GetClient()
        {
            if (oauthToken != null && oauthSecret != null)
            {
                return new RestClient(FanfouConsts.API_URL, FanfouConsts.CONSUMER_KEY, FanfouConsts.CONSUMER_SECRET,
                    oauthToken, oauthSecret);
            }
            return null;
        }

        #region account

        public void AccountNotification()
        {
        }

        public void AccountUpdateProfile(string url = "", string location = "", string description = "",
            string name = "", string email = "")
        {
        }

        public async Task Login(string username, string password)
        {
            this.username = username;
            this.password = password;

            var client = new RestClient(FanfouConsts.BASE_URL, FanfouConsts.CONSUMER_KEY,
                FanfouConsts.CONSUMER_SECRET);
            await client.Login(FanfouConsts.ACCESS_TOKEN, username, password);

            oauthToken = client.token;
            oauthSecret = client.tokenSecret;
        }

        public void Logout()
        {
            oauthToken = "";
            oauthSecret = "";
            username = "";
            password = "";
            currentUser = null;

            SettingStorage.Instance.currentUser = null;
        }

        public async Task<User> VerifyCredentials()
        {
            User result = await GetClient().GetRequestObject<User>(FanfouConsts.VERIFY_CREDENTIALS);
            currentUser = result;
            var ua = new UserAuth();
            ua.oauthToken = oauthToken;
            ua.oauthSecret = oauthSecret;
            ua.username = username;
            ua.password = password;
            result.auth = ua;

            SettingStorage.Instance.currentUser = result;

            return result;
        }

        #endregion

        #region status

        public async Task<Status> StatusUpdate(string status, string in_reply_to_status_id = "", string in_reply_to_user_id = "", string repost_status_id = "", string location = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("status", status);
            if (in_reply_to_status_id != "")
                parameters.Add("in_reply_to_status_id", in_reply_to_status_id);
            if (in_reply_to_user_id != "")
                parameters.Add("in_reply_to_user_id", in_reply_to_user_id);
            if (repost_status_id != "")
                parameters.Add("repost_status_id", repost_status_id);
            if (location != "")
                parameters.Add("location", location);

            Status result = await client.PostRequestObject<Status>(FanfouConsts.STATUS_UPDATE, parameters);
            return result;
        }


        public async Task<Status> StatusDestroy(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);

            Status result = await client.PostRequestObject<Status>(FanfouConsts.STATUS_DESTROY, parameters);
            return result;
        }

        public async Task<List<Status>> StatusContextTimeline(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUSES_CONTEXT_TIMELINE, parameters);
            return result;
        }

        public async Task<List<Status>> StatusUserTimeline(string id, int count, int page = 1, string since_id = "",
            string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_USER_TIMELINE, parameters);
            return result;
        }

        public async Task<List<Status>> StatusHomeTimeline(int count, int page = 1, string id = "", string since_id = "",
            string max_id = "")
        {

            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);
            if (id != "")
                parameters.Add("id", id);
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_HOME_TIMELINE, parameters);
            return result;
        }

        public async Task<List<Status>> StatusPublicTimeline(int count = 20, int page = 1, string since_id = "", string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_PUBLIC_TIMELINE, parameters);
            return result;
        }

        public async Task<List<Status>> StatusMentionTimeline(int count, int page = 1, string since_id = "", string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_MENTION_TIMELINE, parameters);
            return result;
        }

        #endregion

        #region favorite

        public async Task<List<Status>> FavoritesId(string id, int count, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            if (page > 0)
                parameters.Add("page", page);
            parameters.Add("count", count.ToString());
            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.FAVORITES_ID, parameters);
            return result;
        }

        public async Task<Status> FavoritesCreate(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            Status result =
                await
                    client.PostRequestObject<Status>(FanfouConsts.FAVORITES_CREATE_ID + ":" + id + ".json",
                        parameters);
            return result;
        }

        public async Task<Status> FavoritesDestroy(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            Status result =
                await client.PostRequestObject<Status>(FanfouConsts.FAVORITES_DESTROY_ID + id + ".json", parameters);
            return result;
        }

        #endregion

        #region search

        public async Task<List<Status>> SearchTimeline(string q, int count = 60, string since_id = "", string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("q", q);
            parameters.Add("count", count.ToString());
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);
            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.SEARCH_PUBLIC_TIMELINE, parameters);
            return result;
        }

        public async Task<List<Status>> SearchUserTimeline(string q, string id, int count = 60, string since_id = "",
            string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("q", q);
            parameters.Add("id", id);
            parameters.Add("count", count.ToString());
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);
            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.SEARCH_USER_TIMELINE, parameters);
            return result;
        }

        public async Task<UserList> SearchUser(string q, int count = 60, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("q", q);
            parameters.Add("count", count.ToString());
            parameters.Add("page", page.ToString());
            UserList result = await client.GetRequestObject<UserList>(FanfouConsts.SEARCH_USER, parameters);
            return result;
        }

        public async Task<TrendsList> TrendsList()
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            TrendsList result = await client.GetRequestObject<TrendsList>(FanfouConsts.TRENDS_LIST, parameters);
            return result;
        }

        public async Task<List<string>> TaggedList(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
        
            var ds = new DataContractJsonSerializer(typeof(List<string>));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await client.GetRequest(FanfouConsts.USERS_TAG_LIST, parameters))))
            {
                var obj = ds.ReadObject(ms) as List<string>;
                return obj;
            } 
        }

        public async Task<List<User>> Tagged(string tag, int count = 100, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("tag", tag);
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page.ToString());
            List<User> result = await client.GetRequestObjectCollection<User>(FanfouConsts.USERS_TAGGED, parameters);
            return result;
        }

        public void SavedSearchList()
        {
        }

        #endregion

        #region user

        public async Task<List<User>> UsersShow(string id)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            List<User> result = await client.GetRequestObjectCollection<User>(FanfouConsts.USERS_SHOW, parameters);
            return result;
        }

        public async Task<List<User>> UsersFollowers(string id, int count = 60, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            if (page > 0)
                parameters.Add("page", page);
            parameters.Add("count", count.ToString());
            List<User> result =
                await client.GetRequestObjectCollection<User>(FanfouConsts.USERS_FOLLOWERS, parameters);
            return result;
        }

        public async Task<List<User>> UsersFriends(string id, int count = 60, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            if (page > 0)
                parameters.Add("page", page);
            parameters.Add("count", count.ToString());
            List<User> result =
                await client.GetRequestObjectCollection<User>(FanfouConsts.USERS_FRIENDS, parameters);
            return result;
        }

        #endregion

        #region friendship

        public void FriendshipCreate(string id)
        {
        }

        public void FriendshipDestroy(string id)
        {
        }

        public void FriendshipRequests(int page = 1)
        {
        }

        public void FriendshipAccept(string id)
        {
        }

        public void FriendshipDeny(string id)
        {
        }

        public void FriendshipExists(string user_a, string user_b)
        {
        }

        #endregion

        #region photo

        public async Task<List<Status>> PhotosUserTimeline(string id, int count, int page = 1, string since_id = "",
            string max_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);
            if (since_id != "")
                parameters.Add("since_id", since_id);
            if (max_id != "")
                parameters.Add("max_id", max_id);

            List<Status> result =
                await client.GetRequestObjectCollection<Status>(FanfouConsts.PHOTOS_USER_TIMELINE, parameters);
            return result;
        }

        public async Task<Status> PhotoUpload(string status, StorageFile photo, string location = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("status", status);
            if (location != "")
                parameters.Add("location", location);

            Status result =
                await client.PostRequestWithFile<Status>(FanfouConsts.PHOTOS_UPLOAD, parameters, "photo", photo);
            return result;
        }

        #endregion

        #region direct

        public async Task<List<DirectMessageItem>> DirectMessagesConversationList(int page = 1, int count = 20)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);

            List<DirectMessageItem> result =
                await
                    client.GetRequestObjectCollection<DirectMessageItem>(
                        FanfouConsts.DIRECT_MESSAGES_CONVERSATION_LIST, parameters);
            return result;
        }

        public async Task<List<DirectMessage>> DirectMessagesConversation(string id, int count = 60, int page = 1)
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("id", id);
            parameters.Add("count", count.ToString());
            if (page > 0)
                parameters.Add("page", page);

            List<DirectMessage> result =
                await
                    client.GetRequestObjectCollection<DirectMessage>(FanfouConsts.DIRECT_MESSAGES_CONVERSATION,
                        parameters);
            return result;
        }

        public async Task<DirectMessage> DirectMessagesNew(string user, string text, string in_reply_to_id = "")
        {
            RestClient client = GetClient();
            var parameters = new Parameters();
            parameters.Add("user", user);
            parameters.Add("text", text);
            if (in_reply_to_id != "")
                parameters.Add("in_reply_to_id", in_reply_to_id);

            DirectMessage result =
                await client.PostRequestObject<DirectMessage>(FanfouConsts.DIRECT_MESSAGES_NEW, parameters);
            return result;
        }

        #endregion
    }
}