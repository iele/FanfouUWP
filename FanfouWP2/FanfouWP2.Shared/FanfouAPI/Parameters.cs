using System;
using System.Collections.Generic;
using System.Text;

namespace FanfouWP2.FanfouAPI
{
    public class Parameters
    {
        public Parameters()
        {
            this.Items = new List<KeyValuePair<string, string>>(10);
        }

        public List<KeyValuePair<string, string>> Items
        {
            get;
            private set;
        }
        public void Clear()
        {
            this.Items.Clear();
        }
        public void Sort()
        {
            this.Items.Sort(new Comparison<KeyValuePair<string, string>>((x1, x2) =>
            {
                if (x1.Key == x2.Key)
                {
                    return string.Compare(x1.Value, x2.Value);
                }
                else
                {
                    return string.Compare(x1.Key, x2.Key);
                }
            }));
        }
        public void Add(string key, object value)
        {
            this.Add(key, (value == null ? string.Empty : value.ToString()));
        }
        public void Add(string key, string value)
        {
            this.Items.Add(new KeyValuePair<string, string>(key, value));
        }

        public string BuildQueryString(bool encodeValue)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var p in this.Items)
            {
                if (buffer.Length != 0) buffer.Append("&");
                buffer.AppendFormat("{0}={1}", encodeValue ? XAuthHelper.UrlEncode(p.Key) : p.Key, encodeValue ? XAuthHelper.UrlEncode(p.Value) : p.Value);
            }
            return buffer.ToString();
        }
    }
}
