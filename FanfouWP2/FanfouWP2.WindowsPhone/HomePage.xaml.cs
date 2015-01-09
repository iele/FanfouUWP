using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Events;
using FanfouWP2.FanfouAPI.Items;
using FanfouWP2.Utils;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Phone.UI.Input;

namespace FanfouWP2
{
    public sealed partial class HomePage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly TimelineCache cache = Utils.TimelineCache.Instance;

        public readonly PaginatedCollection<Status> mentions;
        public readonly PaginatedCollection<Status> statuses;

        private User currentUser = new User();

        private TimelineStorage<Status> storage = new TimelineStorage<Status>();

        private bool is_menu_open = false;

        public HomePage()
        {
            InitializeComponent();

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            statuses = cache.statuses;
            mentions = cache.mentions;

            mentions.load = async (c) =>
            {
                if (mentions.Count > 0)
                {
                    loading.Visibility = Visibility.Visible;
                    try
                    {
                        var result =
                            await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(c, max_id: mentions.Last().id);
                        if (result.Count == 0)
                            mentions.HasMoreItems = false;
                        Utils.StatusesReform.append(mentions, result);
                        return result.Count;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                    finally
                    {
                        loading.Visibility = Visibility.Collapsed;
                    }
                }
                return 0;
            };
            statuses.load = async (c) =>
            {
                if (statuses.Count > 0)
                {
                    loading.Visibility = Visibility.Visible;
                    try
                    {
                        var result =
                            await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(c, max_id: statuses.Last().id);
                        if (result.Count == 0)
                            statuses.HasMoreItems = false;
                        Utils.StatusesReform.append(statuses, result);
                        return result.Count;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                    finally
                    {
                        loading.Visibility = Visibility.Collapsed;
                    }
                }
                return 0;
            };

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.is_menu_open)
            {
                this.hideMenuStoryboard.Begin();
                e.Handled = true;
            }
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {      
            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            defaultViewModel["currentUser"] = currentUser;

            if (this.statuses.Count == 0)
            {
                var s1 = await storage.ReadDataFromIsolatedStorage(FanfouAPI.FanfouConsts.STATUS_HOME_TIMELINE, currentUser.id);
                if (s1 != null)
                    StatusesReform.append(this.statuses, s1.ToList());
            }
            else
            {
                StatusesReform.append(this.statuses, new List<Status>());
            }
            if (this.mentions.Count == 0)
            {
                var s20 = await storage.ReadDataFromIsolatedStorage(FanfouAPI.FanfouConsts.STATUS_MENTION_TIMELINE, this.currentUser.id);
                if (s20 != null)
                    StatusesReform.append(this.mentions, s20.ToList());
            }
            else
            {
                StatusesReform.append(this.mentions, new List<Status>());
            }

            defaultViewModel["statuses"] = statuses;
            defaultViewModel["mentions"] = mentions;

            loading.Visibility = Visibility.Visible;
            try
            {
                if (this.statuses.Count != 0)
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1, since_id: this.statuses.First().id);
                    Utils.StatusesReform.insertFirst(statuses, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1);
                    Utils.StatusesReform.append(statuses, result);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (this.mentions.Count != 0)
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1, since_id: this.mentions.First().id);

                    Utils.StatusesReform.insertFirst(mentions, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1);
                    Utils.StatusesReform.append(mentions, result);
                }
            }
            catch (Exception)
            {
            }

            loading.Visibility = Visibility.Collapsed;

            setTiles();
        }

        private async void setTiles()
        {
            Utils.TileUpdater.Clear();

            var images = from s in statuses where s.user != null select s.user.profile_image_url;
            List<string> list = new List<string>();
            foreach (var i in images)
            {
                try
                {
                    var u = await WebDataCache.GetLocalUriAsync(new Uri(i));
                    if (!list.Contains(u.ToString()))
                        list.Add(u.ToString());
                }
                catch (Exception)
                {
                }
            }
            if (list.Count() > 6)
                list = list.GetRange(0, 6);

            if (statuses.Count() < 5)
            {
                foreach (var i in statuses)
                {
                    Utils.TileUpdater.SetTile(i.user.screen_name, i.text, list.ToArray());
                }
            }
            else
            {
                for (var i = 0; i < 5; i++)
                {
                    Utils.TileUpdater.SetTile(statuses[i].user.screen_name, statuses[i].text, list.ToArray());
                }
            }
        }

        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            await storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_MENTION_TIMELINE, this.currentUser.id, mentions, 100);
            await storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_HOME_TIMELINE, this.currentUser.id, statuses, 100);
        }

        private void PublicItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PublicPage));
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            try
            {
                if (this.statuses.Count != 0)
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1, since_id: this.statuses.First().id);
                    Utils.StatusesReform.insertFirst(statuses, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1);
                    Utils.StatusesReform.append(statuses, result);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (this.mentions.Count != 0)
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1, since_id: this.mentions.First().id);
                    Utils.StatusesReform.insertFirst(mentions, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1);
                    Utils.StatusesReform.append(mentions, result);
                }
            }
            catch (Exception)
            {
            }

            loading.Visibility = Visibility.Collapsed;
        }

        private void FavoriteGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavoritePage), Utils.DataConverter<User>.Convert(currentUser));
        }

        private void SearchGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private void FindGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FindPage));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            hideMenuStoryboard.Begin();
        }

        private void TrendsGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(TrendsPage));
        }

        private void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void SendItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage));
        }

        private async void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as Status).is_refresh)
            {
                try
                {
                    var prev = this.statuses[this.statuses.IndexOf(e.ClickedItem as Status) - 1];
                    var next = this.statuses[this.statuses.IndexOf(e.ClickedItem as Status) + 1];
                    if (prev.id != null && next.id != null)
                    {
                        try
                        {
                            loading.Visibility = Visibility.Visible;
                            var result =
                                await
                                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1, since_id: next.id,
                                        max_id: prev.id);
                            Utils.StatusesReform.insertBetween(statuses, result, prev.id);
                            this.statuses.Remove(e.ClickedItem as Status);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            loading.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            else
                Frame.Navigate(typeof(StatusPage), Utils.DataConverter<Status>.Convert(e.ClickedItem as Status));
        }

        private async void mentionsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as Status).is_refresh)
            {
                try
                {
                    var prev = this.mentions[this.mentions.IndexOf(e.ClickedItem as Status) - 1];
                    var next = this.mentions[this.mentions.IndexOf(e.ClickedItem as Status) + 1];
                    if (prev.id != null && next.id != null)
                    {
                        try
                        {
                            loading.Visibility = Visibility.Visible;
                            var result =
                                await
                                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1, since_id: next.id,
                                        max_id: prev.id);
                            Utils.StatusesReform.insertBetween(mentions, result, prev.id);
                            this.mentions.Remove(e.ClickedItem as Status);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            loading.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            else
                Frame.Navigate(typeof(StatusPage), Utils.DataConverter<Status>.Convert(e.ClickedItem as Status));
        }

        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InformationPage));
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

        private void CameraItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Photo).ToString());
        }

        private void SelfGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelfPage), Utils.DataConverter<User>.Convert(currentUser));
        }

        private void UpItem_Click(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    if (statuses.Count != 0)
                        this.statusesGridView.ScrollIntoView(this.statuses.First());
                    break;
                case 1:
                    if (mentions.Count != 0)
                        this.mentionsGridView.ScrollIntoView(this.mentions.First());
                    break;
                default:
                    break;
            }
        }

        private void showMenuStoryboard_Completed(object sender, object e)
        {
            is_menu_open = true;
        }

        private void hideMenuStoryboard_Completed(object sender, object e)
        {
            is_menu_open = false;
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!this.is_menu_open)
                showMenuStoryboard.Begin();
            else
                hideMenuStoryboard.Begin();
        }
    }
}