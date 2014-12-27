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

        public HomePage()
        {
            InitializeComponent();

            statuses = cache.statuses;
            mentions = cache.mentions;

            mentions.load = async (c) =>
            {
                if (mentions.Count > 0)
                {
                    mentions.is_loading = true;
                    loading.Visibility = Visibility.Visible;
                    try
                    {
                        var result =
                            await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(c, max_id: mentions.Last().id);
                        Utils.StatusesReform.reform(mentions, result);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        loading.Visibility = Visibility.Collapsed;
                        mentions.is_loading = false;
                    }
                }
            };
            statuses.load = async (c) =>
            {
                if (statuses.Count > 0)
                {
                    statuses.is_loading = true;
                    loading.Visibility = Visibility.Visible;
                    try
                    {
                        var result =
                            await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(c, max_id: statuses.Last().id);
                        Utils.StatusesReform.reform(statuses, result);
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        loading.Visibility = Visibility.Collapsed;
                        statuses.is_loading = false;
                    }
                }
            };

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;
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
                    StatusesReform.reform(this.statuses, s1.ToList());
            }
            else
            {
                StatusesReform.reform(this.statuses, new List<Status>());
            }
            if (this.mentions.Count == 0)
            {
                var s20 = await storage.ReadDataFromIsolatedStorage(FanfouAPI.FanfouConsts.STATUS_MENTION_TIMELINE, this.currentUser.id);
                if (s20 != null)
                    StatusesReform.reform(this.mentions, s20.ToList());
            }
            else
            {
                StatusesReform.reform(this.mentions, new List<Status>());
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
                    Utils.StatusesReform.reform(statuses, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1);
                    Utils.StatusesReform.reform(statuses, result);
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

                    Utils.StatusesReform.reform(mentions, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1);
                    Utils.StatusesReform.reform(mentions, result);
                }
            }
            catch (Exception)
            {
            }

            loading.Visibility = Visibility.Collapsed;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_MENTION_TIMELINE, this.currentUser.id, mentions, 100);
            storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_HOME_TIMELINE, this.currentUser.id, statuses, 100);
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;

            mentions.is_loading = false;

            Utils.ToastShow.ShowInformation("时间线更新失败");
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            StatusesReform.reform(mentions, ss);

            storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_MENTION_TIMELINE, this.currentUser.id, mentions, 100);

            mentions.is_loading = false;

            defaultViewModel["date"] = "更新时间 " + DateTime.Now;
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;

            statuses.is_loading = false;

            Utils.ToastShow.ShowInformation("时间线更新失败");
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            StatusesReform.reform(statuses, ss);

            storage.SaveDataToIsolatedStorageWithLimit(FanfouAPI.FanfouConsts.STATUS_HOME_TIMELINE, this.currentUser.id, statuses, 100);
            statuses.is_loading = false;

            defaultViewModel["date"] = "更新时间 " + DateTime.Now;
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
                    Utils.StatusesReform.reform(statuses, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1);
                    Utils.StatusesReform.reform(statuses, result);
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

                    Utils.StatusesReform.reform(mentions, result);
                }
                else
                {
                    var result = await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1);
                    Utils.StatusesReform.reform(mentions, result);
                }
            }
            catch (Exception)
            {
            }

            loading.Visibility = Visibility.Collapsed;
        }

        private void FavoriteGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavoritePage), currentUser);
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
            hub.ScrollToSection(hub.Sections.First());
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
                    this.statuses.Remove(e.ClickedItem as Status);
                    if (prev.id != null && next.id != null)
                    {
                        try
                        {
                            loading.Visibility = Visibility.Visible;
                            var result =
                                await
                                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1, since_id: next.id,
                                        max_id: prev.id);
                            Utils.StatusesReform.reform(statuses, result);
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
                Frame.Navigate(typeof(StatusPage), e.ClickedItem);
        }

        private async void mentionsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as Status).is_refresh)
            {
                try
                {
                    var prev = this.mentions[this.mentions.IndexOf(e.ClickedItem as Status) - 1];
                    var next = this.mentions[this.mentions.IndexOf(e.ClickedItem as Status) + 1];
                    this.mentions.Remove(e.ClickedItem as Status);
                    if (prev.id != null && next.id != null)
                    {
                        try
                        {
                            loading.Visibility = Visibility.Visible;
                            var result =
                                await
                                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1, since_id: next.id,
                                        max_id: prev.id);
                            Utils.StatusesReform.reform(mentions, result);
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
                Frame.Navigate(typeof(StatusPage), e.ClickedItem);
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
            Frame.Navigate(typeof(SendPage), new Tuple<object, SendPage.SendMode>(null, SendPage.SendMode.Photo));
        }

        private void SelfItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelfPage), currentUser);
        }
    }
}