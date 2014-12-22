// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍
using System;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;
using Windows.UI.StartScreen;

namespace FanfouWP2
{
    /// <summary>
    ///     可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SendPage : Page, IFileOpenPickerContinuable
    {
        public enum SendMode
        {
            Normal,
            Reply,
            Repost,
            ReplyUser,
            Photo
        };

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;
        private StorageFile file;

        private string location;

        private SendMode mode = SendMode.Normal;
        private Status status;
        private User user;

        public SendPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.StatusUpdateSuccess += Instance_StatusUpdateSuccess;
            FanfouAPI.FanfouAPI.Instance.StatusUpdateFailed += Instance_StatusUpdateFailed;

            FanfouAPI.FanfouAPI.Instance.PhotosUploadSuccess += Instance_PhotosUploadSuccess;
            FanfouAPI.FanfouAPI.Instance.PhotosUploadFailed += Instance_PhotosUploadFailed;
        }

        /// <summary>
        ///     获取与此 <see cref="Page" /> 关联的 <see cref="NavigationHelper" />。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        /// <summary>
        ///     获取此 <see cref="Page" /> 的视图模型。
        ///     可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            try
            {
                if (args.Files.Count > 0)
                {
                    file = args.Files[0];
                    if (null != file)
                    {
                        var bi = new BitmapImage();
                        IRandomAccessStream s = await file.OpenAsync(FileAccessMode.Read);
                        await bi.SetSourceAsync(s);
                        photo.Source = bi;
                        mode = SendMode.Photo;
                    }
                }
                else
                {
                    var bi = new BitmapImage();
                    photo.Source = bi;
                    mode = SendMode.Normal;
                }
            }
            catch (Exception e)
            {
            }
        }

        private void Instance_PhotosUploadFailed(object sender, FailedEventArgs e)
        {
            Utils.ToastShow.ShowInformation("图片发送失败");
        }

        private void Instance_PhotosUploadSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            Utils.ToastShow.ShowInformation("图片发送成功", () => navigationHelper.GoBack());
        }

        private void Instance_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
            Utils.ToastShow.ShowInformation("消息发送失败");
        }

        private void Instance_StatusUpdateSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            Utils.ToastShow.ShowInformation("消息发送成功", () => navigationHelper.GoBack());

        }

        /// <summary>
        ///     使用在导航过程中传递的内容填充页。  在从以前的会话
        ///     重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        ///     事件的来源; 通常为 <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     事件数据，其中既提供在最初请求此页时传递给
        ///     <see cref=" Frame.Navigate(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            getLocation();

            if (e.NavigationParameter != null)
            {
                dynamic param = e.NavigationParameter;
                mode = (SendMode)param.Item2;
                switch (mode)
                {
                    case SendMode.Normal:
                        title.Text = "你在做什么?";
                        break;
                    case SendMode.Photo:
                        var openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        openPicker.FileTypeFilter.Add(".jpg");
                        openPicker.FileTypeFilter.Add(".jpeg");
                        openPicker.FileTypeFilter.Add(".png");
                        openPicker.FileTypeFilter.Add(".bmp");
                        openPicker.PickSingleFileAndContinue();
                        break;
                    case SendMode.Reply:
                        status = (Status)param.Item1;
                        title.Text = "回复" + status.user.screen_name;
                        send.Text = "@" + status.user.screen_name;
                        break;
                    case SendMode.ReplyUser:
                        user = (User)param.Item1;
                        title.Text = "提及" + user.screen_name;
                        send.Text = "@" + user.screen_name;
                        break;
                    case SendMode.Repost:
                        status = (Status)param.Item1;
                        title.Text = "转发" + status.user.screen_name;
                        send.Text = "转@" + status.user.screen_name + " " + status.text;
                        break;
                    default:
                        break;
                }
            }
        }

        private async void getLocation()
        {
            location = await Utils.GeoLocator.getGeolocator();
            if (location == "")
            {
                this.locationText.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.locationText.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///     保留与此页关联的状态，以防挂起应用程序或
        ///     从导航缓存中放弃此页。值必须符合
        ///     <see cref="SuspensionManager.SessionState" /> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     提供要使用可序列化状态填充的空字典
        ///     的事件数据。
        /// </param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void SendItem_Click(object sender, RoutedEventArgs e)
        {
            string text = send.Text;
            if (send.Text.Length > 140)
                text = text.Substring(0, 140);
            if (location == null || location == "")
            {
                switch (mode)
                {
                    case SendMode.Normal:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text);
                        break;
                    case SendMode.Photo:
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(text, file);
                        break;
                    case SendMode.Reply:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, status.id);
                        break;
                    case SendMode.ReplyUser:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, in_reply_to_user_id: user.id);
                        break;
                    case SendMode.Repost:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, repost_status_id: status.id);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (mode)
                {
                    case SendMode.Normal:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, location: location);
                        break;
                    case SendMode.Photo:
                        FanfouAPI.FanfouAPI.Instance.PhotoUpload(text, file, location: location);
                        break;
                    case SendMode.Reply:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, status.id, location: location);
                        break;
                    case SendMode.ReplyUser:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, in_reply_to_user_id: user.id, location: location);
                        break;
                    case SendMode.Repost:
                        FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, repost_status_id: status.id, location: location);
                        break;
                    default:
                        break;
                }
            }
        }

        private void PhotoItem_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.PickSingleFileAndContinue();
        }


        private void AtItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void TopicItem_Click(object sender, RoutedEventArgs e)
        {
            var msg = new TagDialog();
            msg.PrimaryButtonClick += msg_PrimaryButtonClick;
            await msg.ShowAsync();
        }

        private void msg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string str = (sender as TagDialog).content;
            send.Text += " #" + str + "#";
        }

        #region NavigationHelper 注册

        /// <summary>
        ///     此部分中提供的方法只是用于使
        ///     NavigationHelper 可响应页面的导航方法。
        ///     <para>
        ///         应将页面特有的逻辑放入用于
        ///         <see cref="NavigationHelper.LoadState" />
        ///         和 <see cref="NavigationHelper.SaveState" /> 的事件处理程序中。
        ///         除了在会话期间保留的页面状态之外
        ///         LoadState 方法中还提供导航参数。
        ///     </para>
        /// </summary>
        /// <param name="e">
        ///     提供导航方法数据和
        ///     无法取消导航请求的事件处理程序。
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void TileItem_Click(object sender, RoutedEventArgs e)
        {
            Uri square150x150Logo = new Uri("ms-appx:///Assets/Logo.png");
            SecondaryTile secondaryTile = new SecondaryTile("FanfouWP_SendPage",
                                                            "饭窗 - 发送消息",
                                                            "FanfouWP_SendPage",
                                                            square150x150Logo,
                                                            TileSize.Square150x150);
            secondaryTile.VisualElements.Square30x30Logo = new Uri("ms-appx:///Assets/SmallLogo.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;
            secondaryTile.RequestCreateAsync();
        }
    }
}