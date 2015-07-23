using System.Collections.Generic;
using System.Text;

namespace FanfouUWP.FanfouAPI
{
    public class Parameters
    {
        public Parameters()
        {
            Items = new List<KeyValuePair<string, string>>(10);
        }

        public List<KeyValuePair<string, string>> Items { get; private set; }

        public void Clear()
        {
            Items.Clear();
        }

        public void Sort()
        {
            Items.Sort((x1, x2) =>
            {
                if (x1.Key == x2.Key)
                {
                    return string.Compare(x1.Value, x2.Value);
                }
                return string.Compare(x1.Key, x2.Key);
            });
        }

        public void Add(string key, object value)
        {
            Add(key, (value == null ? string.Empty : value.ToString()));
        }

        public void Add(string key, string value)
        {
            Items.Add(new KeyValuePair<string, string>(key, value));
        }

        public string BuildQueryString(bool encodeValue)
        {
            var buffer = new StringBuilder();
            foreach (var p in Items)
            {
                if (buffer.Length != 0) buffer.Append("&");
                buffer.AppendFormat("{0}={1}", encodeValue ? XAuthHelper.UrlEncode(p.Key) : p.Key,
                    encodeValue ? XAuthHelper.UrlEncode(p.Value) : p.Value);
            }
            return buffer.ToString();
        }
    }
}