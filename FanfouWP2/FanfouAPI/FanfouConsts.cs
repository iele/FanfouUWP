namespace FanfouWP2.FanfouAPI
{
    public static class FanfouConsts
    {
        public const string CONSUMER_KEY = "ca01f862370339af2dc80eca5e9fa8cd";
        public const string CONSUMER_SECRET = "6faef3388beac280fc1830a7797c4e55";

        public const string BASE_URL = "http://fanfou.com";
        public const string API_URL = "http://api.fanfou.com";

        public const string ACCESS_TOKEN = "oauth/access_token";
        public const string VERIFY_CREDENTIALS = "account/verify_credentials.json";
        public const string ACCOUNT_NOTIFICATION = "account/notification.json";
        public const string ACCOUNT_UPDATE_PROFILE = "/account/update_profile.json";

        public const string STATUS_HOME_TIMELINE = "statuses/home_timeline.json";
        public const string STATUS_PUBLIC_TIMELINE = "statuses/public_timeline.json";
        public const string STATUS_USER_TIMELINE = "statuses/user_timeline.json";
        public const string STATUS_MENTION_TIMELINE = "statuses/mentions.json";
        public const string STATUS_UPDATE = "statuses/update.json";
        public const string STATUS_DESTROY = "statuses/destroy.json";
        public const string STATUSES_CONTEXT_TIMELINE = "statuses/context_timeline.json";

        public const string FAVORITES_ID = "favorites/id.json";
        public const string FAVORITES_CREATE_ID = "favorites/create/";
        public const string FAVORITES_DESTROY_ID = "favorites/destroy/";

        public const string SEARCH_PUBLIC_TIMELINE = "search/public_timeline.json";
        public const string SEARCH_USER_TIMELINE = "search/user_timeline.json";
        public const string SEARCH_USER = "search/users.json";
        public const string TRENDS_LIST = "trends/list.json";
        public const string SAVED_SEARCHES_LIST = "saved_searches/list.json";

        public const string USERS_SHOW = "users/show.json";
        public const string USERS_TAG_LIST = "users/tag_list.json";
        public const string USERS_TAGGED = "users/tagged.json";
        public const string USERS_FOLLOWERS = "users/followers.json";
        public const string USERS_FRIENDS = "users/friends.json";

        public const string PHOTOS_UPLOAD = "photos/upload.json";
        public const string PHOTOS_USER_TIMELINE = "photos/user_timeline.json";

        public const string FRIENDSHIPS_CREATE = "friendships/create.json";
        public const string FRIENDSHIPS_DESTROY = "friendships/destroy.json";
        public const string FRIENDSHIPS_REQUESTS = "friendships/requests.json";
        public const string FRIENDSHIPS_DENY = "friendships/deny.json";
        public const string FRIENDSHIPS_ACCEPT = "friendships/accept.json";
        public const string FRIENDSHIPS_EXISTS = "friendships/exists.json";

        public const string DIRECT_MESSAGES_CONVERSATION_LIST = "direct_messages/conversation_list.json";
        public const string DIRECT_MESSAGES_CONVERSATION = "direct_messages/conversation.json";
        public const string DIRECT_MESSAGES_DESTROY = "direct_messages/destroy.json";
        public const string DIRECT_MESSAGES_NEW = "direct_messages/new.json";
        
        public const string BLOCKS_BLOCKING = "blocks/blocking.json";
        public const string BLOCKS_CREATE = "blocks/create.json";
        public const string BLOCKS_DESTROY = "blocks/destroy.json";
        public const string BLOCKS_EXISTS = "blocks/exists.json";
    }
}