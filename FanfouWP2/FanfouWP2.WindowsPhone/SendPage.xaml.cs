using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouWP2
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SendPage : Page, IFileOpenPickerContinuable
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private string location;

        public enum SendMode { Normal, Reply, Repost, ReplyUser, Photo };
        private SendMode mode = SendMode.Normal;
        private Status status;
        private User user;

        private StorageFile file;

        public SendPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            FanfouAPI.FanfouAPI.Instance.StatusUpdateSuccess += Instance_StatusUpdateSuccess;
            FanfouAPI.FanfouAPI.Instance.StatusUpdateFailed += Instance_StatusUpdateFailed;

            FanfouAPI.FanfouAPI.Instance.PhotosUploadSuccess += Instance_PhotosUploadSuccess;
            FanfouAPI.FanfouAPI.Instance.PhotosUploadFailed += Instance_PhotosUploadFailed;
        }

        void Instance_PhotosUploadFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_PhotosUploadSuccess(object sender, EventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed;
            this.navigationHelper.GoBack();
        }

        void Instance_StatusUpdateFailed(object sender, FanfouAPI.FailedEventArgs e)
        {
        }

        void Instance_StatusUpdateSuccess(object sender, EventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed;
            this.navigationHelper.GoBack();
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref=" Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.NavigationParameter != null)
            {
                dynamic param = e.NavigationParameter;
                mode = (SendMode)param.Item2;
                switch (mode)
                {
                    case SendMode.Normal:
                        this.title.Text = "你在做什么?";
                        break;
                    case SendMode.Photo:
                        break;
                    case SendMode.Reply:
                        status = (Status)param.Item1;
                        this.title.Text = "回复" + status.user.screen_name;
                        send.Text = "@" + status.user.screen_name;
                        break;
                    case SendMode.ReplyUser:
                        user = (User)param.Item1;
                        this.title.Text = "提及" + user.screen_name;
                        send.Text = "@" + user.screen_name;
                        break;
                    case SendMode.Repost:
                        status = (Status)param.Item1;
                        this.title.Text = "转发" + status.user.screen_name;
                        send.Text = "转@" + status.user.screen_name + " " + status.text;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SendItem_Click(object sender, RoutedEventArgs e)
        {
            var text = this.send.Text;
            if (this.send.Text.Length > 140)
                text = text.Substring(0, 140);
            switch (mode)
            {
                case SendMode.Normal:
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(text);
                    break;
                case SendMode.Photo:
                    FanfouAPI.FanfouAPI.Instance.PhotoUpload(text, file);
                    break;
                case SendMode.Reply:
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(text, in_reply_to_status_id: status.id);
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

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                file = args.Files[0];
                if (null != file)
                {
                    var bi = new BitmapImage();
                    var s = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    await bi.SetSourceAsync(s);
                    this.photo.Source = bi;
                    mode = SendMode.Photo;
                }
            }
            else
            {
            }
        }


        private void AtItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void TopicItem_Click(object sender, RoutedEventArgs e)
        {
            TagDialog msg = new TagDialog();
            msg.PrimaryButtonClick += msg_PrimaryButtonClick;
            await msg.ShowAsync();
        }

        void msg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var str = (sender as TagDialog).content;
            send.Text += " #" + str + "#";
        }

    }
}
