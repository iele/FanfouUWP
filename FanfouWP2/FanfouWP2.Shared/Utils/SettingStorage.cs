using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace FanfouWP.Storage
{
    public sealed class SettingStorage
    {
        public Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private static SettingStorage instance;

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
            set
            {
                instance = value;
            }
        }

        private readonly static string CONTAINER_NAME = "setting";

        public bool hasSetting
        {
            private set { }
            get
            {
                return localSettings.Containers.ContainsKey(CONTAINER_NAME);
            }
        }

        private ApplicationDataContainer getContainer()
        {
            return localSettings.CreateContainer(CONTAINER_NAME, Windows.Storage.ApplicationDataCreateDisposition.Always);
        }

        private string serialize<T>(object o) where T : Item
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream();
            serializer.WriteObject(ms, o);
            var buff = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buff, 0, buff.Length);
            var s= Encoding.UTF8.GetString(buff, 0, buff.Length);
            return s;
        }
        private object dserialize<T>(string str) where T : Item
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(str))) as T;
        }

        public UserAuth currentUserAuth
        {
            get
            {
                try
                {
                    if (getContainer().Values.ContainsKey("currentUserAuth"))
                    {
                        return dserialize<UserAuth>(getContainer().Values["currentUserAuth"] as string) as UserAuth;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                }
                return null;

            }
            set
            {
                getContainer().Values["currentUserAuth"] = serialize<UserAuth>(value);
            }
        }

    }
}
