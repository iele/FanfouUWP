using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;
using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;
using System.Threading;
using Windows.UI.Popups;
using FanfouUWP.ItemControl;
using System.Collections.ObjectModel;

namespace FanfouUWP
{
    public sealed partial class HomePage : Page
    {
        private readonly NavigationHelper navigationHelper;

        private readonly TimelineCache cache = Utils.TimelineCache.Instance;

        private ObservableCollection<string> tags = new ObservableCollection<string>();

        public readonly PaginatedCollection<Status> mentions;
        public readonly PaginatedCollection<Status> statuses;

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ObservableCollection<Trends> trends = new ObservableCollection<Trends>();

        private User currentUser = new User();

        public HomePage()
        {
            InitializeComponent();

            statuses = cache.statuses;
            mentions = cache.mentions;

            mentions.load = async (c) =>
            {
                if (mentions.Count > 0)
                {
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
                        Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                        return 0;
                    }
                }
                return 0;
            };
            statuses.load = async (c) =>
            {
                if (statuses.Count > 0)
                {
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
                        Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                        return 0;
                    }
                }
                return 0;
            };

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;

            Window.Current.SizeChanged += Current_SizeChanged;
            InnerCustomPanel.SizeChanged += InnerCustomPanel_SizeChanged;
            InnerCustomPanel2.SizeChanged += InnerCustomPanel_SizeChanged2;
        }


        private void InnerCustomPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SV1.ChangeView(null, 100.0, null, true);
        }

        private void InnerCustomPanel_SizeChanged2(object sender, SizeChangedEventArgs e)
        {
            SV12.ChangeView(null, 100.0, null, true);
        }


        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            InnerCustomPanel.InvalidateMeasure();
            InnerCustomPanel2.InvalidateMeasure();
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }


        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            SV1.ChangeView(null, 100, null);
            SV12.ChangeView(null, 100, null);

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            defaultViewModel["user"] = currentUser;

            defaultViewModel["trends"] = trends;

            try
            {
                tags.Clear();
                var list = await FanfouUWP.FanfouAPI.FanfouAPI.Instance.TaggedList(currentUser.id);
                foreach (var item in list)
                    tags.Add(item);
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.TrendsList();
                trends.Clear();
                foreach (Trends item in ss.trends)
                {
                    trends.Add(item);
                }
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

            try
            {
                var result = await FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20, 1);
                Utils.StatusesReform.append(statuses, result);
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

            try
            {
                var result = await FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20, 1);
                Utils.StatusesReform.append(mentions, result);
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

            var t = new Timer((p) =>
            {
                loadingStatus();
            }, null, 60000, 60000);

            setTiles();
        }

        private async void setTiles()
        {
            Utils.TileUpdater.Clear();

            if (statuses.Count() < 5)
            {
                foreach (var i in statuses)
                {
                    Utils.TileUpdater.SetTile(i.user.screen_name, i.text);
                }
            }
            else
            {
                for (var i = 0; i < 5; i++)
                {
                    Utils.TileUpdater.SetTile(statuses[i].user.screen_name, statuses[i].text);
                }
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void PublicItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PublicPage));
        }

        private async void loadingStatus()
        {
            SV1.ChangeView(null, 0, null, true);
            VisualStateManager.GoToState(this, "PullToRefresh", false);
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
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
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
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }

        }

        private void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            loadingStatus();
        }

        private void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }


        private void SendItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage));
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), Utils.DataConverter<Status>.Convert(e.ClickedItem as Status));
        }

        private void mentionsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), Utils.DataConverter<Status>.Convert(e.ClickedItem as Status));
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

        private void timeline_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void favorite_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void follower_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void friend_Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        private void trendsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trendsGridView.SelectedIndex != -1)
            {
                var t = trendsGridView.SelectedItem as Trends;
                Frame.Navigate(typeof(SearchPage), Utils.DataConverter<Trends>.Convert(t));
            }
        }

        private async void StatusItemControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var status = (sender as StatusItemControl).DataContext as Status;

            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("个人资料", (command) =>
            {
                Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
            }));
            menu.Commands.Add(new UICommand("转发", (command) =>
            {
                Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Repost).ToString() + Utils.DataConverter<Status>.Convert(status));
            }));
            menu.Commands.Add(new UICommand("回复", (command) =>
            {
                Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Reply).ToString() + Utils.DataConverter<Status>.Convert(status));
            }));
            var chosenCommand = await menu.ShowForSelectionAsync
                (Utils.MenuRect.GetElementRect(e.GetPosition(App.RootFrame)));
            if (chosenCommand == null)
            {
            }
        }

        private async void MentionsItemControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var status = (sender as StatusItemControl).DataContext as Status;

            var menu = new PopupMenu();
            menu.Commands.Add(new UICommand("个人资料", (command) =>
            {
                Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(status.user));
            }));
            menu.Commands.Add(new UICommand("转发", (command) =>
            {
                Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Repost).ToString() + Utils.DataConverter<Status>.Convert(status));
            }));
            menu.Commands.Add(new UICommand("回复", (command) =>
            {
                Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.Reply).ToString() + Utils.DataConverter<Status>.Convert(status));
            }));
            var chosenCommand = await menu.ShowForSelectionAsync
                 (Utils.MenuRect.GetElementRect(e.GetPosition(App.RootFrame)));
            if (chosenCommand == null)
            {
            }
        }
        private void Taglist_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(TagUserPage), e.ClickedItem as string);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv.VerticalOffset == 0)
            {
                SV1.DirectManipulationCompleted += SV1_DirectManipulationCompleted;
                VisualStateManager.GoToState(this, "Refreshing", false);
            }
        }

        private void SV1_DirectManipulationCompleted(object sender, object e)
        {
            SV1.DirectManipulationCompleted -= SV1_DirectManipulationCompleted;
            loadingStatus();
        }


        private void ScrollViewer_ViewChanged2(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv.VerticalOffset == 0)
            {
                SV12.DirectManipulationCompleted += SV1_DirectManipulationCompleted2;
                VisualStateManager.GoToState(this, "Refreshing", false);
            }
        }

        private void SV1_DirectManipulationCompleted2(object sender, object e)
        {
            SV12.DirectManipulationCompleted -= SV1_DirectManipulationCompleted2;
            loadingStatus();
        }

    }
}