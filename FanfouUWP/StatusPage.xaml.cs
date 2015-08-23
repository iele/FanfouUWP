using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;

using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.ItemControl;
using FanfouUWP.CustomControl;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml.Documents;
using System.Text.RegularExpressions;
using FanfouUWP.Utils;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouUWP
{
    /// <summary>
    ///     可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StatusPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private Status status;

        public StatusPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
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

        /// <summary>
        ///     使用在导航过程中传递的内容填充页。  在从以前的会话
        ///     重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        ///     事件的来源; 通常为 <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     事件数据，其中既提供在最初请求此页时传递给
        ///     <see cref=" Frame.Navigate1(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            status = Utils.DataConverter<Status>.Convert(e.NavigationParameter as string);
            defaultViewModel["status"] = status;

            TextToLinks(status.text);

            contextMessage.Children.Clear();

            if (this.status.favorited)
            {
                this.FavItem.Label = "取消收藏";
                this.FavItem.Icon = new SymbolIcon(Symbol.UnFavorite);
            }
            else
            {
                this.FavItem.Label = "收藏";
                this.FavItem.Icon = new SymbolIcon(Symbol.Favorite);
            }

            if (this.status.location != "")
            {
                try
                {
                    char[] s = { ',' };
                    string[] l = this.status.location.Split(s);
                    try
                    {
                        this.Map.Center = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = double.Parse(l[0]),
                            Longitude = double.Parse(l[1])
                        });

                        this.Map.ZoomLevel = 15;
                        this.Map.MapElements.Clear();

                        MapIcon MapIcon = new MapIcon();
                        MapIcon.Location = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = double.Parse(l[0]),
                            Longitude = double.Parse(l[1])
                        });
                        MapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                        MapIcon.Title = this.status.location;
                        Map.MapElements.Add(MapIcon);

                        Map.LandmarksVisible = true;
                        Map.Visibility = Visibility.Visible;
                    }
                    catch
                    {
                        var location = (await Utils.GeoLocator.geocode(this.status.location)).Locations[0];
                        this.Map.Center = location.Point;

                        this.Map.ZoomLevel = 12;
                        this.Map.MapElements.Clear();

                        MapIcon MapIcon = new MapIcon();
                        MapIcon.Location = location.Point;
                        MapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                        MapIcon.Title = location.DisplayName;
                        Map.MapElements.Add(MapIcon);

                        Map.LandmarksVisible = true;
                        Map.Visibility = Visibility.Collapsed;
                    }

                }
                catch
                {
                    Map.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                Map.Visibility = Visibility.Collapsed;
            }

            if (this.status.user.id == FanfouAPI.FanfouAPI.Instance.currentUser.id)
            {
                this.DeleteItem.Visibility = Visibility.Visible;
                this.DeleteItem.IsEnabled = true;
            }
            else
            {
                this.DeleteItem.Visibility = Visibility.Collapsed;
                this.DeleteItem.IsEnabled = false;
            }

            if (status.in_reply_to_status_id != null && status.in_reply_to_status_id != "")
            {
                try
                {
                    var ss = await FanfouAPI.FanfouAPI.Instance.StatusContextTimeline(status.id);
                    foreach (Status item in ss)
                    {
                        var sic = new ReplyItemControl();
                        sic.Tapped += (s, a) => Frame.Navigate(typeof(UserPage),
                           Utils.DataConverter<User>.Convert(item.user));
                        sic.DataContext = item;
                        contextMessage.Children.Add(sic);
                    }
                    context.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {

                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                }
            }
            else
            {
                context.Visibility = Visibility.Collapsed;
            }
        }

        private void TextToLinks(string text)
        {
            var us = text.ParseUsername();
            var ts = text.ParseURL();
            var hs = text.ParseHashtag();

            links.Children.Clear();

            if (ts.Count == 0 && hs.Count == 0) //&& us.Count == 0)
            {
                more.Visibility = Visibility.Collapsed;
            }
            else
            {
                more.Visibility = Visibility.Visible;
            }

            if (ts.Count != 0)
            {
                TextBlock t = new TextBlock()
                {
                    Text = "访问链接",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 6, 0, 6)
                };
                links.Children.Add(t);
                foreach (var i in ts)
                {
                    try
                    {
                        Button b = new Button();
                        b.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                        b.Margin = Margin = new Thickness(0, 6, 0, 6);
                        b.Content = i.ToString();
                        b.Click += async (s, a) => await Windows.System.Launcher.LaunchUriAsync(new Uri(i.ToString()));
                        links.Children.Add(b);
                    }
                    finally { }
                }
            }
            if (hs.Count != 0)
            {
                TextBlock t = new TextBlock()
                {
                    Text = "搜索话题",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 6, 0, 6)
                };
                links.Children.Add(t);
                foreach (var i in hs)
                {
                    if (!i.ToString().Equals("##"))
                    {
                        try
                        {
                            Button b = new Button();
                            b.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                            b.Margin = Margin = new Thickness(0, 6, 0, 6);
                            b.Content = i.ToString();
                            b.Click += (s, a) => Frame.Navigate(typeof(SearchPage), i.ToString().Substring(1, i.ToString().Length - 2));
                            links.Children.Add(b);
                        }
                        finally { }
                    }
                }
            }

            //if (us.Count != 0)
            //{
            //    TextBlock t = new TextBlock()
            //    {
            //        Text = "查看用户",
            //        FontSize = 16,
            //        Foreground = new SolidColorBrush(Colors.Gray),
            //        Margin = new Thickness(0, 6, 0, 6)
            //    };
            //    links.Children.Add(t);
            //    foreach (var i in us)
            //    {
            //        if (!i.ToString().Equals("@"))
            //        {
            //            try
            //            {
            //                Button b = new Button();
            //                b.IsEnabled = false;
            //                b.Margin = Margin = new Thickness(0, 6, 0, 6);
            //                b.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            //                b.Content = i.ToString();
            //                b.Click += async (s, a) =>
            //                {
            //                    try
            //                    {
            //                        var name = i.ToString();
            //                        var ss = await FanfouAPI.FanfouAPI.Instance.SearchUser(name.Substring(1, name.Length - 1), 2);
            //                        switch (ss.users.Count)
            //                        {
            //                            case 0:
            //                                Utils.ToastShow.ShowInformation("没有找到这个用户");
            //                                break;
            //                            case 1:
            //                                Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(ss.users[0]));
            //                                break;
            //                            default:
            //                                Frame.Navigate(typeof(FindPage), name.Substring(1, name.Length - 1));
            //                                break;
            //                        }
            //                    }
            //                    catch { }
            //                };
            //                links.Children.Add(b);
            //            }
            //            finally { }
            //        }
            //    }
            //}
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void RepostItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Repost).ToString() + Utils.DataConverter<Status>.Convert(status));
        }

        private void UserItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
        }

        private async void FavItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.status.favorited)
                {
                    status = await FanfouAPI.FanfouAPI.Instance.FavoritesCreate(this.status.id);
                    defaultViewModel["status"] = status;
                    this.FavItem.Label = "取消收藏";
                    this.FavItem.Icon = new SymbolIcon(Symbol.UnFavorite);

                    Utils.TimelineCache.Instance.FindAndChange(status);
                }

                else
                {
                    status = await FanfouAPI.FanfouAPI.Instance.FavoritesDestroy(this.status.id);
                    defaultViewModel["status"] = status;
                    this.FavItem.Label = "收藏";
                    this.FavItem.Icon = new SymbolIcon(Symbol.Favorite);
                    Utils.TimelineCache.Instance.FindAndChange(status);
                }
            }
            catch (Exception)
            {
            }
        }

        private void ReplyItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Reply).ToString() + Utils.DataConverter<Status>.Convert(status));
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImagePage), status.photo.largeurl);
        }

        private void Profile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
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

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await FanfouAPI.FanfouAPI.Instance.StatusDestroy(this.status.id);
                Utils.TimelineCache.Instance.FindAndDelete(this.status);
                navigationHelper.GoBack();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void CopyItem_OnClick(object sender, RoutedEventArgs e)
        {
            var data = new DataPackage();
            data.SetText("@" + status.user.screen_name + ": " + status.text);
            Clipboard.SetContent(data);

            Utils.ToastShow.ShowInformation("该消息已复制到剪贴板");
        }

        private void ShareItem_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager,
    DataRequestedEventArgs>(this.ShareTextHandler);
            DataTransferManager.ShowShareUI();
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "【来自饭否】";
            request.Data.SetText("@" + status.user.screen_name + ": " + status.text);
        }

        private async void Reply_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (status.in_reply_to_user_id != null && status.in_reply_to_user_id != "")
            {
                try
                {
                    Frame.Navigate(typeof(UserPage),
                        Utils.DataConverter<User>.Convert(
                            await FanfouAPI.FanfouAPI.Instance.UsersShow(status.in_reply_to_user_id)));
                }

                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                }
            }
        }

        private async void Repost_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (status.repost_user_id != null && status.repost_user_id != "")
            {
                try
                {
                    Frame.Navigate(typeof(UserPage),
                        Utils.DataConverter<User>.Convert(
                         await FanfouAPI.FanfouAPI.Instance.UsersShow(status.repost_user_id)));
                }

                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                }
            }
        }
    }
}