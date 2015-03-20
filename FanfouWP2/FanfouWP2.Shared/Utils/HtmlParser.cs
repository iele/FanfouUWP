using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace FanfouWP2.Utils
{

    public static class HTMLParser
    {
        public static string ParseURL(this string s)
        {
            return Regex.Replace(s, @"(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?", new MatchEvaluator(HTMLParser.URL));
        }
        public static string ParseUsername(this string s)
        {
            return Regex.Replace(s, @"(@)((?:\w*))", new MatchEvaluator(HTMLParser.Username));
        }
        public static string ParseHashtag(this string s)
        {
            return Regex.Replace(s, @"(#)((?:\w*))(#)", new MatchEvaluator(HTMLParser.Hashtag));
        }

        public static string Link(this string s, string url)
        {
            return string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, s);
        }

        private static string Hashtag(Match m)
        {
            string x = m.ToString();
            string tag = x.Replace("#", "%23");
            return x.Link("http://search.twitter.com/search?q=" + tag);
        }
        private static string Username(Match m)
        {
            string x = m.ToString();
            string username = x.Replace("@", "");
            return x.Link("http://twitter.com/" + username);
        }
        private static string URL(Match m)
        {
            string x = m.ToString();
            return x.Link(x);
        }
    }
}
