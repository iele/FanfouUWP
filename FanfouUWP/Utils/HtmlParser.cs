using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;

namespace FanfouUWP.Utils
{

    public static class HTMLParser
    {
        public static MatchCollection ParseURL(this string s)
        {
            return Regex.Matches(s, @"(http(s)?://){1}([\w-]+\.)+[\w-]+(/\w[\w- ;,./?%&=]*)?");
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
