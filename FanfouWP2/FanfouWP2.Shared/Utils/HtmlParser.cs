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
        public static MatchCollection ParseURL(this string s)
        {
            return Regex.Matches(s, @"(http(s)?://)?([\w-]+\.)+[\w-]+(/\S\w[\w- ;,./?%&=]\S*)?");
        }
        public static MatchCollection ParseUsername(this string s)
        {
            return Regex.Matches(s, @"(@)((?:\w*))");
        }
        public static MatchCollection ParseHashtag(this string s)
        {
            return Regex.Matches(s, @"(#)((?:\w*))(#)");
        }
    }
}
