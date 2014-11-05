using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public class ImageSourceToCacheConverter : IValueConverter
    {
        private const string ImageStorageFolder = "CacheImages";
        private StorageFolder _storage;

        public delegate void ImageCompletedHander(object sender, EventArgs e);
        public event ImageCompletedHander ImageCompleted;

        private BitmapImage bitmap;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (_storage == null)
                {
                    var localCache = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                    _storage = localCache;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                return null;
            }

            var path = value as string;
            if (String.IsNullOrEmpty(path.ToString())) return null;
            var imageFileUri = new Uri(path);
            if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
            {
                try
                {
                    ExtractFromLocalStorage(imageFileUri);
                    return bitmap;
                }
                catch (Exception)
                {
                    DownloadFromWeb(imageFileUri);
                    return bitmap;
                }
            }

            // 不是网络图片,应用内的素材  
            var bm = new BitmapImage(imageFileUri);
            if (ImageCompleted != null)
                ImageCompleted(this, new EventArgs());
            return bm;
        }

        private object LoadDefaultIfPassed(Uri imageFileUri, string defaultImagePath)
        {
            string defaultImageUri = (defaultImagePath ?? string.Empty);
            if (!string.IsNullOrEmpty(defaultImageUri))
            {
                var bm = new BitmapImage(new Uri(defaultImageUri, UriKind.Relative)); //Load default Image  
                if (ImageCompleted != null)
                    ImageCompleted(this, new EventArgs());
                bitmap = bm;
                return bm;
            }
            else
            {
                var bm = new BitmapImage(imageFileUri);
                if (ImageCompleted != null)
                    ImageCompleted(this, new EventArgs());
                bitmap = bm;
                return bm;
            }
        }

        private async void DownloadFromWeb(Uri imageFileUri)
        {
            try
            {
                var httpClient = new HttpClient();
                var bm = new BitmapImage();
                Stream stream = await httpClient.GetStreamAsync(imageFileUri);
                string isolatedStoragePath = GetFileNameInIsolatedStorage(imageFileUri);
                StorageFile sourceFile = await _storage.CreateFileAsync(isolatedStoragePath, CreationCollisionOption.OpenIfExists);
                await WriteToIsolatedStorage(stream, isolatedStoragePath);
                bm.SetSource(await sourceFile.OpenAsync(FileAccessMode.Read));
                if (ImageCompleted != null)
                    ImageCompleted(this, new EventArgs());
                bitmap = bm;
            }
            catch (Exception e)
            {
            }
        }

        private async void ExtractFromLocalStorage(Uri imageFileUri)
        {
            try
            {
                string isolatedStoragePath = GetFileNameInIsolatedStorage(imageFileUri);
                StorageFile sourceFile = await _storage.CreateFileAsync(isolatedStoragePath, CreationCollisionOption.OpenIfExists);
                var bm = new BitmapImage();
                bm.SetSource(await sourceFile.OpenAsync(FileAccessMode.Read));
                if (ImageCompleted != null)
                    ImageCompleted(this, new EventArgs());
                bitmap = bm;
            }
            catch (Exception e)
            {
            }
        }


        private async Task WriteToIsolatedStorage(Stream inputStream, string fileUri)
        {
            var file = await _storage.CreateFileAsync(fileUri, CreationCollisionOption.OpenIfExists);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                await inputStream.CopyToAsync(stream);
            }
        }

        /// <summary>  
        ///     Gets the file name in isolated storage for the Uri specified. This name should be used to search in the isolated storage.  
        /// </summary>  
        /// <param name="uri">The URI.</param>  
        /// <returns></returns>  
        public string GetFileNameInIsolatedStorage(Uri uri)
        {
            return uri.AbsoluteUri.GetHashCode() + ".img";
        }
    }
}
