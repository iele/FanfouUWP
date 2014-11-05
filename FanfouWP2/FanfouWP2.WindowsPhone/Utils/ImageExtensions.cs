using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FanfouWP2.Utils
{
    /// <summary>
    /// Attached properties for Images
    /// </summary>
    public static class ImageExtensions
    {

        /// <summary>
        /// Using a DependencyProperty as the backing store for WebUri.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CacheUriProperty =
            DependencyProperty.RegisterAttached(
                "CacheUri",
                typeof(string),
                typeof(ImageExtensions),
                new PropertyMetadata(null, OnCacheUriChanged));

        /// <summary>
        /// Gets the CacheUri property. This dependency property 
        /// WebUri that has to be cached
        /// </summary>
        public static string GetCacheUri(DependencyObject d)
        {
            return (string)d.GetValue(CacheUriProperty);
        }

        /// <summary>
        /// Sets the CacheUri property. This dependency property 
        /// WebUri that has to be cached
        /// </summary>
        public static void SetCacheUri(DependencyObject d, string value)
        {
            d.SetValue(CacheUriProperty, value);
        }

        private static async void OnCacheUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            //Uri oldCacheUri = (Uri)e.OldValue;
            Uri newCacheUri = new Uri((string)d.GetValue(CacheUriProperty));
            var image = (Image)d;

            if (newCacheUri != null)
            {
                try
                {
                    //Get image from cache (download and set in cache if needed)
                    var cacheUri = await WebDataCache.GetLocalUriAsync(newCacheUri);
                    //Set cache uri as source for the image
                    image.Source = new BitmapImage(cacheUri);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    //Revert to using passed URI
                    image.Source = new BitmapImage(newCacheUri);
                }
            }
            else
                image.Source = null;

        }

    }

}

