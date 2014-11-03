using System;
using System.Globalization;
using System.IO;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace FanfouWP2.ItemControl.ValueConverter
{
    public class ImageSourceToCacheConverter : IValueConverter
    {
        /*       private const string ImageStorageFolder = "CacheImages";
               private IsolatedStorageFile _storage;

               public delegate void ImageCompletedHander(object sender, EventArgs e);
               public event ImageCompletedHander ImageCompleted;

               public ImageSourceToCacheConverter()
               {         
                   try
                   {
                       if (_storage == null)
                       {
                           _storage = IsolatedStorageFile.GetUserStoreForApplication();
                       }
                   }
                   catch (IsolatedStorageException exception)
                   {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                   }
               }

               public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
               {           
                   var path = value as string;
                   if (String.IsNullOrEmpty(path.ToString())) return null;
                   var imageFileUri = new Uri(path);
                   if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
                   {
                       // 先看缓存  
                       if (_storage.FileExists(GetFileNameInIsolatedStorage(imageFileUri)))
                       {
                           return ExtractFromLocalStorage(imageFileUri);
                       }

                       // 再看网络情况  
                       if (!DeviceNetworkInformation.IsNetworkAvailable)
                       {
                           return LoadDefaultIfPassed(imageFileUri, (parameter ?? string.Empty).ToString());
                       }
                
                       // 最后再网上获取  
                       return DownloadFromWeb(imageFileUri);
                   }

                   // 不是网络图片,应用内的素材  
                   var bm = new BitmapImage(imageFileUri);
                   if (ImageCompleted != null)
                       ImageCompleted(this, new EventArgs());
                   return bm;
               }

               public object ConvertBack(object value, Type targetType, object parameter,
                                         CultureInfo culture)
               {
                   throw new NotImplementedException();
               }

               private object LoadDefaultIfPassed(Uri imageFileUri, string defaultImagePath)
               {
                   string defaultImageUri = (defaultImagePath ?? string.Empty);
                   if (!string.IsNullOrEmpty(defaultImageUri))
                   {
                       var bm = new BitmapImage(new Uri(defaultImageUri, UriKind.Relative)); //Load default Image  
                       if (ImageCompleted != null)
                           ImageCompleted(this, new EventArgs());
                       return bm;
                   }
                   else
                   {
                       var bm = new BitmapImage(imageFileUri);
                       if (ImageCompleted != null)
                           ImageCompleted(this, new EventArgs());
                       return bm;
                   }
               }

               private object DownloadFromWeb(Uri imageFileUri)
               {
                   var m_webClient = new WebClient(); //Load from internet  
                   var bm = new BitmapImage();

                   m_webClient.OpenReadCompleted += (o, e) =>
                   {
                       if (e.Error != null || e.Cancelled) return;
                       WriteToIsolatedStorage(IsolatedStorageFile.GetUserStoreForApplication(), e.Result,
                                              GetFileNameInIsolatedStorage(imageFileUri));
                       bm.SetSource(e.Result);
                       if (ImageCompleted != null)
                           ImageCompleted(this, new EventArgs());
                       e.Result.Close();
                   };
                   m_webClient.OpenReadAsync(imageFileUri);
                   return bm;
               }

               private object ExtractFromLocalStorage(Uri imageFileUri)
               {
                   string isolatedStoragePath = GetFileNameInIsolatedStorage(imageFileUri); //Load from local storage  
                   using (
                       IsolatedStorageFileStream sourceFile = _storage.OpenFile(isolatedStoragePath, FileMode.Open,
                                                                                FileAccess.Read))
                   {
                       var bm = new BitmapImage();
                       bm.SetSource(sourceFile); if (ImageCompleted != null)
                           ImageCompleted(this, new EventArgs());

                       return bm;
                   }
               }

               private void WriteToIsolatedStorage(IsolatedStorageFile storage, Stream inputStream,
                                                          string fileName)
               {
                   IsolatedStorageFileStream outputStream = null;
                   try
                   {
                       if (!storage.DirectoryExists(ImageStorageFolder))
                       {
                           storage.CreateDirectory(ImageStorageFolder);
                       }
                       if (storage.FileExists(fileName))
                       {
                           storage.DeleteFile(fileName);
                       }
                       outputStream = storage.CreateFile(fileName);
                       var buffer = new byte[32768];
                       int read;
                       while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                       {
                           outputStream.Write(buffer, 0, read);
                       }
                       outputStream.Close();
                   }
                   catch
                   {
                       //We cannot do anything here.  
                       if (outputStream != null) outputStream.Close();
                   }
               }

               /// <summary>  
               ///     Gets the file name in isolated storage for the Uri specified. This name should be used to search in the isolated storage.  
               /// </summary>  
               /// <param name="uri">The URI.</param>  
               /// <returns></returns>  
               public string GetFileNameInIsolatedStorage(Uri uri)
               {
                   return ImageStorageFolder + "\\" + uri.AbsoluteUri.GetHashCode() + ".img";
               }*/
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
