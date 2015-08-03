using System;
using System.Collections.Generic;
using System.Text;
using FanfouUWP.FanfouAPI.Items;
using System.Runtime.Serialization.Json;
using System.IO;

namespace FanfouUWP.Utils
{
    public static class DataConverter<T>
    {
        public static string Convert(T item)
        {
            var ds = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                ds.WriteObject(ms, item);
                return System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }
        }

        public static T Convert(string str)
        {
            var ds = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (T)ds.ReadObject(ms);
            }
        }
    }
}
