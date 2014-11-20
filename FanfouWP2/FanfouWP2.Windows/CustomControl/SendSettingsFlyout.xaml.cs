using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using FanfouWP2.FanfouAPI;
using FanfouWP2.Utils;

namespace FanfouWP2.CustomControl
{
    public sealed partial class SendSettingsFlyout : SettingsFlyout
    {
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);

        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);

        public enum SendMode
        {
            Normal,
            Reply,
            Repose,
            ReplyUser,
            Photo
        };

        private Item data;
        private StorageFile file;
        private string location;
        private SendMode mode;

        public SendSettingsFlyout()
        {
            InitializeComponent();

            getLocation();

            FanfouAPI.FanfouAPI.Instance.StatusUpdateSuccess += Instance_StatusUpdateSuccess;
            FanfouAPI.FanfouAPI.Instance.StatusUpdateFailed += Instance_StatusUpdateFailed;

            FanfouAPI.FanfouAPI.Instance.PhotosUploadSuccess += Instance_PhotosUploadSuccess;
            FanfouAPI.FanfouAPI.Instance.PhotosUploadFailed += Instance_PhotosUploadFailed;
        }

        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;

        private void Instance_PhotosUploadFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(this, e);
            }
        }

        private void Instance_PhotosUploadSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateSuccess != null)
            {
                StatusUpdateSuccess(this, e);
            }
        }

        private async void getLocation()
        {
            location = await GeoLocator.getGeolocator();
            if (location != "")
            {
                locate.Visibility = Visibility.Visible;
            }
            else
            {
                locate.Visibility = Visibility.Collapsed;
            }
        }

        public void ChangeMode(SendMode mode, Item data = null)
        {
            this.mode = mode;
            this.data = data;

            if (mode == SendMode.Reply)
            {
                Title = "回复消息";
                send.Text = "@" + (data as Status).user.screen_name + " ";
            }
            else if (mode == SendMode.Repose)
            {
                Title = "转发消息";
                string text = "转：@" + (data as Status).user.screen_name + " " + (data as Status).text;
                if (text.Length <= 140)
                    send.Text = text;
                else
                    send.Text = text.Substring(0, 140);
            }
            else if (mode == SendMode.ReplyUser)
            {
                Title = "回复 " + (data as User).screen_name;

                if (!send.Text.StartsWith("@" + (data as User).screen_name))
                    Title = "@" + (data as User).screen_name + " ";
            }
            else
            {
                Title = "发送新消息";
            }
        }

        private void Instance_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(this, e);
            }
        }

        private void Instance_StatusUpdateSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateSuccess != null)
            {
                StatusUpdateSuccess(this, e);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (send.Text.Length > 0 && send.Text.Length <= 140)
            {
                loading.Visibility = Visibility.Visible;
                if (mode == SendMode.Reply)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, (data as Status).id, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, (data as Status).id);
                }
                else if (mode == SendMode.Repose)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, repost_status_id: (data as Status).id,
                            location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, repost_status_id: (data as Status).id);
                }
                else if (mode == SendMode.ReplyUser)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, in_reply_to_user_id: (data as User).id,
                            location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, in_reply_to_user_id: (data as User).id);
                }
                else if (mode == SendMode.Photo)
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(send.Text, file, location);
                    else
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(send.Text, file);
                }
                else
                {
                    if (location != "")
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text, location: location);
                    else
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(send.Text);
                }
            }
        }

        private void TagAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (tag.Text != "")
                send.Text = send.Text + " #" + tag.Text + "#";
            tag.Text = "";
            TagFlyoutBase.Hide();
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
                IRandomAccessStream s = await file.OpenAsync(FileAccessMode.Read);
                await bi.SetSourceAsync(s);
                image.Source = bi;
                mode = SendMode.Photo;
            }
        }
    }
}