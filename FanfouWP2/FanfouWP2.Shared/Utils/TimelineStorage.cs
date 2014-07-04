using FanfouWP2.FanfouAPI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FanfouWP2.Utils
{
    public class TimelineStorage<T> where T : FanfouWP2.FanfouAPI.Item
    {
        public async Task<bool> SaveDataToIsolatedStorage(string name, string user, object data)
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                var dataFolder = await localFolder.CreateFolderAsync("storage-" + user, CreationCollisionOption.OpenIfExists);
                using (var writeStream = await dataFolder.OpenStreamForWriteAsync(name.Replace("/", "").Replace(".json", "") + ".store", CreationCollisionOption.ReplaceExisting))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(data.GetType());
                    serializer.WriteObject(writeStream, data);
                    return true;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return false;
            }
        }


        public async Task<ObservableCollection<T>> ReadDataFromIsolatedStorage(string name, string user)
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await localFolder.CreateFolderAsync("storage-" + user, CreationCollisionOption.OpenIfExists);

                using (var readStream = await dataFolder.OpenStreamForReadAsync(name.Replace("/", "").Replace(".json", "") + ".store"))
                {
                    byte[] buff = new byte[readStream.Length];
                    await readStream.ReadAsync(buff, 0, buff.Length);
                    MemoryStream stream = new MemoryStream();
                    await stream.WriteAsync(buff, 0, buff.Length);
                    ObservableCollection<T> c = new ObservableCollection<T>();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(c.GetType());
                    c = serializer.ReadObject(stream) as ObservableCollection<T>;
                    if (c != null)
                        return c;
                    return new ObservableCollection<T>();
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return null;
            }
        }

    }
}
