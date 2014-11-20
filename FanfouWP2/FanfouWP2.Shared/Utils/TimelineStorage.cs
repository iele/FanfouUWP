using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2.Utils
{
    public class TimelineStorage<T> where T : Item
    {
        public async Task<bool> SaveDataToIsolatedStorage(string name, string user, object data)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFolder dataFolder =
                    await localFolder.CreateFolderAsync("storage-" + user, CreationCollisionOption.OpenIfExists);
                using (
                    Stream writeStream =
                        await
                            dataFolder.OpenStreamForWriteAsync(name.Replace("/", "").Replace(".json", "") + ".store",
                                CreationCollisionOption.ReplaceExisting))
                {
                    var serializer = new DataContractJsonSerializer(data.GetType());
                    serializer.WriteObject(writeStream, data);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return false;
            }
        }


        public async Task<ObservableCollection<T>> ReadDataFromIsolatedStorage(string name, string user)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFolder dataFolder =
                    await localFolder.CreateFolderAsync("storage-" + user, CreationCollisionOption.OpenIfExists);

                using (
                    Stream readStream =
                        await dataFolder.OpenStreamForReadAsync(name.Replace("/", "").Replace(".json", "") + ".store"))
                {
                    var buff = new byte[readStream.Length];
                    await readStream.ReadAsync(buff, 0, buff.Length);
                    var stream = new MemoryStream();
                    await stream.WriteAsync(buff, 0, buff.Length);
                    var c = new ObservableCollection<T>();
                    var serializer = new DataContractJsonSerializer(c.GetType());
                    c = serializer.ReadObject(stream) as ObservableCollection<T>;
                    if (c != null)
                        return c;
                    return new ObservableCollection<T>();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }
        }
    }
}