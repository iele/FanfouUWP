using FanfouWP2.FanfouAPI.Items;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Storage;
namespace FanfouWP2.Utils
{
    public sealed class SettingStorage
    {
        private static SettingStorage instance;

        private static readonly string CONTAINER_NAME = "setting";
        public ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static SettingStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingStorage();
                }
                return instance;
            }
            set { instance = value; }
        }

        public bool hasSetting
        {
            private set { }
            get { return localSettings.Containers.ContainsKey(CONTAINER_NAME); }
        }

        public User currentUser
        {
            get
            {
                try
                {
                    if (getContainer().Values.ContainsKey("currentUser"))
                    {
                        return dserialize<User>(getContainer().Values["currentUser"] as string) as User;
                    }
                    return null;
                }
                catch (Exception e)
                {
                }
                return null;
            }
            set
            {
                if (value != null)
                    getContainer().Values["currentUser"] = serialize<User>(value);
                else
                    getContainer().Values.Remove("currentUser");
            }
        }

        private ApplicationDataContainer getContainer()
        {
            return localSettings.CreateContainer(CONTAINER_NAME, ApplicationDataCreateDisposition.Always);
        }

        private string serialize<T>(object o) where T : Item
        {
            var serializer = new DataContractJsonSerializer(typeof (T));
            var ms = new MemoryStream();
            serializer.WriteObject(ms, o);
            var buff = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buff, 0, buff.Length);
            string s = Encoding.UTF8.GetString(buff, 0, buff.Length);
            return s;
        }

        private object dserialize<T>(string str) where T : Item
        {
            var serializer = new DataContractJsonSerializer(typeof (T));
            return serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(str))) as T;
        }
    }
}