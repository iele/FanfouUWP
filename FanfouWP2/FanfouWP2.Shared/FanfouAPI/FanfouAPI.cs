using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace FanfouWP2.FanfouAPI
{
    public class FanfouAPI
    {
        public string oauthToken;
        public string oauthSecret;

        public string username;
        public string password;

        public User currentUser { get; private set; }

        public delegate void LoginSuccessHandler(object sender, EventArgs e);
        public delegate void LoginFailedHandler(object sender, FailedEventArgs e);
        public delegate void VerifyCredentialsSuccessHandler(object sender, EventArgs e);
        public delegate void VerifyCredentialsFailedHandler(object sender, FailedEventArgs e);
        public delegate void AccountNotificationSuccessHandler(object sender, EventArgs e);
        public delegate void AccountNotificationFailedHandler(object sender, FailedEventArgs e);
        public delegate void AccountUpdateProfileSuccessHandler(object sender, EventArgs e);
        public delegate void AccountUpdateProfileFailedHandler(object sender, FailedEventArgs e);

        public event LoginSuccessHandler LoginSuccess;
        public event LoginFailedHandler LoginFailed;
        public event VerifyCredentialsSuccessHandler VerifyCredentialsSuccess;
        public event VerifyCredentialsFailedHandler VerifyCredentialsFailed;
        public event AccountNotificationSuccessHandler AccountNotificationSuccess;
        public event AccountNotificationFailedHandler AccountNotificationFailed;
        public event AccountUpdateProfileSuccessHandler AccountUpdateProfileSuccess;
        public event AccountUpdateProfileFailedHandler AccountUpdateProfileFailed;

        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);
        public delegate void StatusDestroySuccessHandler(object sender, EventArgs e);
        public delegate void StatusDestroyFailedHandler(object sender, FailedEventArgs e);

        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;
        public event StatusDestroySuccessHandler StatusDestroySuccess;
        public event StatusDestroyFailedHandler StatusDestroyFailed;

        public delegate void HomeTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void HomeTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void PublicTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void PublicTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void MentionTimelineSuccessHandler(object sender, EventArgs e);
        public delegate void MentionTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void UserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Status> e);
        public delegate void UserTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void ContextTimelineSuccessHandler(object sender, UserTimelineEventArgs<Status> e);
        public delegate void ContextTimelineFailedHandler(object sender, FailedEventArgs e);

        public event HomeTimelineSuccessHandler HomeTimelineSuccess;
        public event HomeTimelineFailedHandler HomeTimelineFailed;
        public event PublicTimelineSuccessHandler PublicTimelineSuccess;
        public event PublicTimelineFailedHandler PublicTimelineFailed;
        public event MentionTimelineSuccessHandler MentionTimelineSuccess;
        public event MentionTimelineFailedHandler MentionTimelineFailed;
        public event UserTimelineSuccessHandler UserTimelineSuccess;
        public event UserTimelineFailedHandler UserTimelineFailed;
        public event ContextTimelineSuccessHandler ContextTimelineSuccess;
        public event ContextTimelineFailedHandler ContextTimelineFailed;

        public delegate void FavoritesCreateSuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesCreateFailedHandler(object sender, FailedEventArgs e);
        public delegate void FavoritesDestroySuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesDestroyFailedHandler(object sender, FailedEventArgs e);
        public delegate void FavoritesSuccessHandler(object sender, EventArgs e);
        public delegate void FavoritesFailedHandler(object sender, FailedEventArgs e);

        public event FavoritesCreateSuccessHandler FavoritesCreateSuccess;
        public event FavoritesCreateFailedHandler FavoritesCreateFailed;
        public event FavoritesDestroySuccessHandler FavoritesDestroySuccess;
        public event FavoritesDestroyFailedHandler FavoritesDestroyFailed;
        public event FavoritesSuccessHandler FavoritesSuccess;
        public event FavoritesFailedHandler FavoritesFailed;

        public delegate void SearchTimelineSuccessHandler(object sender, UserTimelineEventArgs<Status> e);
        public delegate void SearchTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void SearchUserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Status> e);
        public delegate void SearchUserTimelineFailedHandler(object sender, FailedEventArgs e);
        public delegate void SearchUserSuccessHandler(object sender, UserTimelineEventArgs<User> e);
        public delegate void SearchUserFailedHandler(object sender, FailedEventArgs e);

        public event SearchTimelineSuccessHandler SearchTimelineSuccess;
        public event SearchTimelineFailedHandler SearchTimelineFailed;
        public event SearchUserTimelineSuccessHandler SearchUserTimelineSuccess;
        public event SearchUserTimelineFailedHandler SearchUserTimelineFailed;
        public event SearchUserSuccessHandler SearchUserSuccess;
        public event SearchUserFailedHandler SearchUserFailed;

        public delegate void TrendsListSuccessHandler(object sender, TrendsListEventArgs e);
        public delegate void TrendsListFailedHandler(object sender, FailedEventArgs e);

        public event TrendsListSuccessHandler TrendsListSuccess;
        public event TrendsListFailedHandler TrendsListFailed;

        public delegate void TagListSuccessHandler(object sender, ListEventArgs<string> e);
        public delegate void TagListFailedHandler(object sender, FailedEventArgs e);
        public delegate void TaggedSuccessHandler(object sender, UserTimelineEventArgs<User> e);
        public delegate void TaggedFailedHandler(object sender, FailedEventArgs e);

        public event TagListSuccessHandler TagListSuccess;
        public event TagListFailedHandler TagListFailed;
        public event TaggedSuccessHandler TaggedSuccess;
        public event TaggedFailedHandler TaggedFailed;

        public delegate void UsersShowSuccessHandler(object sender, EventArgs e);
        public delegate void UsersShowFailedHandler(object sender, FailedEventArgs e);
        public delegate void UsersFollowersSuccessHandler(object sender, UserTimelineEventArgs<User> e);
        public delegate void UsersFollowersFailedHandler(object sender, FailedEventArgs e);
        public delegate void UsersFriendsSuccessHandler(object sender, UserTimelineEventArgs<User> e);
        public delegate void UsersFriendsFailedHandler(object sender, FailedEventArgs e);

        public event UsersShowSuccessHandler UsersShowSuccess;
        public event UsersShowFailedHandler UsersShowFailed;
        public event UsersFollowersSuccessHandler UsersFollowersSuccess;
        public event UsersFollowersFailedHandler UsersFollowersFailed;
        public event UsersFriendsSuccessHandler UsersFriendsSuccess;
        public event UsersFriendsFailedHandler UsersFriendsFailed;

        public delegate void PhotosUploadSuccessHandler(object sender, EventArgs e);
        public delegate void PhotosUploadFailedHandler(object sender, FailedEventArgs e);
        public delegate void PhotosUserTimelineSuccessHandler(object sender, UserTimelineEventArgs<Status> e);
        public delegate void PhotosUserTimelineFailedHandler(object sender, FailedEventArgs e);

        public event PhotosUploadSuccessHandler PhotosUploadSuccess;
        public event PhotosUploadFailedHandler PhotosUploadFailed;
        public event PhotosUserTimelineSuccessHandler PhotosUserTimelineSuccess;
        public event PhotosUserTimelineFailedHandler PhotosUserTimelineFailed;

        public delegate void FriendshipsCreateSuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsCreateFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsDestroySuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsDestroyFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsRequestsSuccessHandler(object sender, UserTimelineEventArgs<User> e);
        public delegate void FriendshipsRequestsFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsAcceptSuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsAcceptFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsDenySuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsDenyFailedHandler(object sender, FailedEventArgs e);
        public delegate void FriendshipsExistsSuccessHandler(object sender, EventArgs e);
        public delegate void FriendshipsExistsFailedHandler(object sender, FailedEventArgs e);

        public event FriendshipsCreateSuccessHandler FriendshipsCreateSuccess;
        public event FriendshipsCreateFailedHandler FriendshipsCreateFailed;
        public event FriendshipsDestroySuccessHandler FriendshipsDestroySuccess;
        public event FriendshipsDestroyFailedHandler FriendshipsDestroyFailed;
        public event FriendshipsRequestsSuccessHandler FriendshipsRequestsSuccess;
        public event FriendshipsRequestsFailedHandler FriendshipsRequestsFailed;
        public event FriendshipsAcceptSuccessHandler FriendshipsAcceptSuccess;
        public event FriendshipsAcceptFailedHandler FriendshipsAcceptFailed;
        public event FriendshipsDenySuccessHandler FriendshipsDenySuccess;
        public event FriendshipsDenyFailedHandler FriendshipsDenyFailed;
        public event FriendshipsExistsSuccessHandler FriendshipsExistsSuccess;
        public event FriendshipsExistsFailedHandler FriendshipsExistsFailed;

        public delegate void DirectMessageConversationListSuccessHandler(object sender, UserTimelineEventArgs<DirectMessageItem> e);
        public delegate void DirectMessageConversationListFailedHandler(object sender, FailedEventArgs e);
        public delegate void DirectMessageConversationSuccessHandler(object sender, UserTimelineEventArgs<DirectMessage> e);
        public delegate void DirectMessageConversationFailedHandler(object sender, FailedEventArgs e);
        public delegate void DirectMessageNewSuccessHandler(object sender, EventArgs e);
        public delegate void DirectMessageNewFailedHandler(object sender, FailedEventArgs e);

        public event DirectMessageConversationListSuccessHandler DirectMessageConversationListSuccess;
        public event DirectMessageConversationListFailedHandler DirectMessageConversationListFailed;
        public event DirectMessageConversationSuccessHandler DirectMessageConversationSuccess;
        public event DirectMessageConversationFailedHandler DirectMessageConversationFailed;
        public event DirectMessageNewSuccessHandler DirectMessageNewSuccess;
        public event DirectMessageNewFailedHandler DirectMessageNewFailed;

        public delegate void SavedSearchListSuccessHandler(object sender, ListEventArgs<Search> e);
        public delegate void SavedSearchListFailedHandler(object sender, FailedEventArgs e);

        public event SavedSearchListSuccessHandler SavedSearchListSuccess;
        public event SavedSearchListFailedHandler SavedSearchListFailed;


        private static FanfouAPI instance;
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
            set
            {
                instance = value;
            }
        }
        public FanfouAPI()
        {

        }

        private int[] CountIndex = { 100, 300, 500, 1000 };

        private RestClient GetClient()
        {
            if (oauthToken != null && oauthSecret != null)
            {
                return new RestClient(FanfouConsts.API_URL, FanfouConsts.CONSUMER_KEY, FanfouConsts.CONSUMER_SECRET, oauthToken, oauthSecret);
            }
            else
            {
                return null;
            }
        }

        #region account
        public void AccountNotification()
        {

        }

        public void AccountUpdateProfile(string url = "", string location = "", string description = "", string name = "", string email = "")
        {

        }

        public async void Login(string username, string password)
        {
            this.username = username;
            this.password = password;

            try
            {
                var client = new RestClient(FanfouConsts.BASE_URL, FanfouConsts.CONSUMER_KEY, FanfouConsts.CONSUMER_SECRET);
                await client.Login(FanfouConsts.ACCESS_TOKEN, username, password);

                this.oauthToken = client.token;
                this.oauthSecret = client.tokenSecret;
                LoginSuccess(this, new EventArgs());
            }
            catch (Exception e)
            {
                var args = new FailedEventArgs();
                LoginFailed(this, args);
            }
        }


        public async void VerifyCredentials()
        {
            try
            {
                var result = await GetClient().GetRequestObject<User>(FanfouConsts.VERIFY_CREDENTIALS);
                this.currentUser = result;
                VerifyCredentialsSuccess(result, new EventArgs());
            }
            catch (Exception e)
            {
                VerifyCredentialsFailed(this, new FailedEventArgs());
            }
        }

        #endregion
        #region status
        public void StatusUpdate(string status, string in_reply_to_status_id = "", string in_reply_to_user_id = "", string repost_status_id = "", string location = "")
        {
        }


        public void StatusDestroy(string id)
        {

        }

        public void StatusContextTimeline(string id)
        {

        }

        public void StatusUserTimeline(int count, string user_id)
        {

        }
        public async void StatusHomeTimeline(int count, int page, string since_id = "", string max_id = "")
        {
            try
            {
                var client = GetClient();
                var parameters = new Parameters();
                parameters.Add("count", count.ToString());
                if (page > 0)
                    parameters.Add("page", page);
                if (since_id != "")
                    parameters.Add("since_id", since_id);
                if (max_id != "")
                    parameters.Add("max_id", max_id);

                var result = await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_HOME_TIMELINE, parameters);
                if (HomeTimelineSuccess != null)
                    HomeTimelineSuccess(result, new EventArgs());
            }
            catch (Exception e)
            {
                if (HomeTimelineFailed != null)
                    HomeTimelineFailed(this, new FailedEventArgs());
            }
        }

        public async void StatusPublicTimeline(int count = 20, string since_id = "", string max_id = "", string refresh_id = "")
        {
            try
            {
                var client = GetClient();
                var parameters = new Parameters();
                parameters.Add("count", count.ToString());
                if (since_id != "")
                    parameters.Add("since_id", since_id);
                if (max_id != "")
                    parameters.Add("max_id", max_id);

                var result = await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_PUBLIC_TIMELINE, parameters);
                PublicTimelineSuccess(result, new EventArgs());
            }
            catch (Exception e)
            {
                PublicTimelineFailed(this, new FailedEventArgs());
            }
        }
        public async void StatusMentionTimeline(int count, string since_id = "", string max_id = "", string refresh_id = "")
        {
            try
            {
                var client = GetClient();
                var parameters = new Parameters();
                parameters.Add("count", count.ToString());
                if (since_id != "")
                    parameters.Add("since_id", since_id);
                if (max_id != "")
                    parameters.Add("max_id", max_id);

                var result = await client.GetRequestObjectCollection<Status>(FanfouConsts.STATUS_MENTION_TIMELINE, parameters);
                MentionTimelineSuccess(result, new EventArgs());
            }
            catch (Exception e)
            {
                MentionTimelineFailed(this, new FailedEventArgs());
            }
        }
        public void FavoritesCreate(string id)
        {

        }

        public void FavoritesDestroy(string id)
        {

        }

        #endregion
        #region search
        public void SearchTimeline(string q, int count = 60, string max_id = "")
        {

        }

        public void SearchUserTimeline(string q, string id = "", int count = 60, string max_id = "")
        {

        }
        public void SearchUser(string q, int count = 60, int page = 0)
        {

        }

        public void TrendsList()
        {
        }

        public void TaggedList(string id)
        {

        }

        public void Tagged(string tag)
        {

        }

        public void SavedSearchList()
        {

        }
        #endregion
        #region user
        public void UsersShow(string id)
        {

        }
        public void UsersFollowers(string id, int count = 60, int page = 1)
        {

        }

        public void UsersFriends(string id, int count = 60, int page = 1)
        {
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
        public void PhotosUserTimeline(string id, int count = 60)
        {

        }
        public void PhotoUpload(string status, WriteableBitmap photo, string location = "")
        {
        }


        #endregion
        #region direct
        public void DirectMessagesConversationList(int page = 1, int count = 20)
        {

        }

        public void DirectMessagesConversation(string id, int count = 60, int page = 1)
        {

        }

        public void DirectMessagesNew(string user, string text, string in_reply_to_id)
        {

        }
        #endregion

    }
}

