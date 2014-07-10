using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FanfouWP2.CustomControl
{
    public sealed partial class SendSettingsFlyout : SettingsFlyout
    {
        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);

        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;

        private string location;

        public enum SendMode { Normal, Reply, Repose, ReplyUser, Photo };
        private SendMode mode;
        private Item data;
        private StorageFile file;

        public SendSettingsFlyout()
        {
            this.InitializeComponent();

            getLocation();

            FanfouAPI.FanfouAPI.Instance.StatusUpdateSuccess += Instance_StatusUpdateSuccess;
            FanfouAPI.FanfouAPI.Instance.StatusUpdateFailed += Instance_StatusUpdateFailed;

            FanfouAPI.FanfouAPI.Instance.PhotosUploadSuccess += Instance_PhotosUploadSuccess;
            FanfouAPI.FanfouAPI.Instance.PhotosUploadFailed += Instance_PhotosUploadFailed;
        }

        void Instance_PhotosUploadFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(this, e);
            }
        }

        void Instance_PhotosUploadSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateSuccess != null)
            {
                StatusUpdateSuccess(this, e);
            }
        }

        private async void getLocation()
        {
            location = await Utils.GeoLocator.getGeolocator();
            if (location != "")
            {
                this.locate.Visibility = Visibility.Visible;
            }
            else
            {
                this.locate.Visibility = Visibility.Collapsed;
            }
        }

        public void ChangeMode(SendMode mode, Item data = null)
        {
            this.mode = mode;
            this.data = data;

            if (mode == SendMode.Reply)
            {
                this.Title = "回复消息";
                this.send.Text = "@" + (data as Status).user.screen_name + " ";
            }
            else if (mode == SendMode.Repose)
            {
                this.Title = "转发消息";
                var text = "转：@" + (data as Status).user.screen_name + " " + (data as Status).text;
                if (text.Length <= 140)
                    this.send.Text = text;
                else
                    this.send.Text = text.Substring(0, 140);
            }
            else if (mode == SendMode.ReplyUser)
            {
                this.Title = "回复 " + (data as User).screen_name;

                if (!this.send.Text.StartsWith("@" + (data as User).screen_name))
                    this.Title = "@" + (data as User).screen_name + " ";
                else
                {
                }
            }
            else
            {
                this.Title = "发送新消息";
            }
        }

        void Instance_StatusUpdateFailed(object sender, FanfouAPI.FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(this, e);
            }
        }

        void Instance_StatusUpdateSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateSuccess != null)
            {
                StatusUpdateSuccess(this, e);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.send.Text.Length > 0 && this.send.Text.Length <= 140)
            {
                loading.Visibility = Visibility.Visible;
                if (mode == SendMode.Reply)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_status_id: (data as Status).id, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_status_id: (data as Status).id);
                }
                else if (mode == SendMode.Repose)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, repost_status_id: (data as Status).id, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, repost_status_id: (data as Status).id);
                }
                else if (mode == SendMode.ReplyUser)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_user_id: (data as User).id, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_user_id: (data as User).id);
                }
                else if (mode == SendMode.Photo)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(this.send.Text, file, location);
                    else
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(this.send.Text, file);
                }
                else
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text);
                }
            }
        }

        private void TagAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.tag.Text != "")
                this.send.Text = this.send.Text + " #" + this.tag.Text + "#";
            this.tag.Text = "";
            this.TagFlyoutBase.Hide();
        }
        private async void ImageAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");
            file = await openPicker.PickSingleFileAsync();
            if (null != file)
            {
                var bi = new BitmapImage();
                var s = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                await bi.SetSourceAsync(s);
                this.image.Source = bi;
                mode = SendMode.Photo;
            }
        }
    }
}
