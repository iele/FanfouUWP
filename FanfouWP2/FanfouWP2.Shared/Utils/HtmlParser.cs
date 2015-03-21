using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

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
            return string.Format("<[{0}{1}>]", url, s);
        }

        private static string Hashtag(Match m)
        {
            string x = m.ToString();
            return x.Link("1");
        }
        private static string Username(Match m)
        {
            string x = m.ToString();
            return x.Link("2");
        }
        private static string URL(Match m)
        {
            string x = m.ToString();
            return x.Link("3");
        }
    }
}
