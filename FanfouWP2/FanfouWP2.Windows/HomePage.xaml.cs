using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.Common;
using FanfouWP2.CustomControl;
using FanfouWP2.FanfouAPI;
using FanfouWP2.Utils;

namespace FanfouWP2
{
    public sealed partial class HomePage : Page
    {
        public enum PageType
        {
            Statuses,
            Mentions,
            Publics
        };

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private readonly ObservableCollection<Status> mentions = new ObservableCollection<Status>();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableCollection<Status> publics = new ObservableCollection<Status>();
        private readonly ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private bool can_loading;

        private Status currentClick;
        private PageType currentType = PageType.Statuses;
        private User currentUser = new User();
        private bool is_loading;

        public HomePage()
        {
            InitializeComponent();

            Loaded += HomePage_Loaded;

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_HomeTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_HomeTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.MentionTimelineSuccess += Instance_MentionTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.MentionTimelineFailed += Instance_MentionTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.PublicTimelineSuccess += Instance_PublicTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.PublicTimelineFailed += Instance_PublicTimelineFailed;

            send.StatusUpdateSuccess += send_StatusUpdateSuccess;
            send.StatusUpdateFailed += send_StatusUpdateFailed;

            status.UserButtonClick += status_UserButtonClick;
            status.ReplyButtonClick += status_ReplyButtonClick;
            status.RepostButtonClick += status_RepostButtonClick;
            status.FavButtonClick += status_FavButtonClick;
            status.FavCreateSuccess += status_FavCreateSuccess;
            status.FavDestroySuccess += status_FavDestroySuccess;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        private void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            var defaultsCommand = new SettingsCommand("账户", "账户",
                handler =>
                {
                    var sf = new AccountSettingsFlyout();
                    sf.Show();
                });
            e.Request.ApplicationCommands.Add(defaultsCommand);
        }

        private void status_FavDestroySuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (Status i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
            foreach (Status i in mentions)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
            foreach (Status i in publics)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
        }

        private void status_FavCreateSuccess(object sender, EventArgs e)
        {
            var s = sender as Status;
            foreach (Status i in statuses)
            {
                if (i.id == s.id)
                {
                    i.favorited = true;
                }
            }
            foreach (Status i in mentions)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
            foreach (Status i in publics)
            {
                if (i.id == s.id)
                {
                    i.favorited = false;
                }
            }
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = VisualHelper.FindVisualChildByName<ScrollViewer>(statusesGridView, "ScrollViewer");
            var sb = VisualHelper.FindVisualChildByName<ScrollBar>(sv, "HorizontalScrollBar");
            sb.ValueChanged += sb_ValueChanged;
        }

        private void sb_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue >= (sender as ScrollBar).Maximum - 100 && !is_loading && can_loading)
            {
                loading.Visibility = Visibility.Visible;
                is_loading = true;
                switch (currentType)
                {
                    case PageType.Statuses:
                        if (statuses.Count != 0)
                            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, max_id: statuses.Last().id);
                        break;
                    case PageType.Mentions:
                        if (mentions.Count != 0)
                            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, max_id: mentions.Last().id);
                        break;
                    case PageType.Publics:
                        loading.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
                return;
            }
            //if (e.NewValue <= (sender as ScrollBar).Minimum + 100 && !is_loading && can_loading)
            //{
            //    loading.Visibility = Visibility.Visible;
            //    is_loading = true;
            //    switch (currentType)
            //    {
            //        case PageType.Statuses:
            //            if (statuses.Count != 0)
            //                FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, since_id: statuses.First().id);
            //            break;
            //        case PageType.Mentions:
            //            if (mentions.Count != 0)
            //                FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, since_id: mentions.First().id);
            //            break;
            //        case PageType.Publics:
            //            loading.Visibility = Visibility.Collapsed;
            //            break;
            //        default:
            //            break;
            //    }
            //    return;
            //}
            can_loading = true;
        }

        private void status_FavButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void status_RepostButtonClick(object sender, RoutedEventArgs e)
        {
            sendPopup.IsOpen = true;
            send.ChangeMode(SendSettingsFlyout.SendMode.Repose, currentClick);
        }

        private void status_ReplyButtonClick(object sender, RoutedEventArgs e)
        {
            sendPopup.IsOpen = true;
            send.ChangeMode(SendSettingsFlyout.SendMode.Reply, currentClick);
        }

        private void status_UserButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (UserPage), currentClick.user);
        }

        private void send_StatusUpdateFailed(object sender, FailedEventArgs e)
        {
            sendPopup.IsOpen = false;
            loading.Visibility = Visibility.Visible;
        }

        private void send_StatusUpdateSuccess(object sender, EventArgs e)
        {
            sendPopup.IsOpen = false;
            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, since_id: statuses.First().id);
                    break;
                case PageType.Mentions:
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, since_id: mentions.First().id);
                    break;
                case PageType.Publics:
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    break;
                default:
                    break;
            }
        }

        private void Instance_PublicTimelineFailed(object sender, FailedEventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_PublicTimelineSuccess(object sender, EventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            publics.Clear();
            StatusesReform.reform(publics, ss);
            defaultViewModel["date"] = "更新时间 " + DateTime.Now;
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            StatusesReform.reform(mentions, ss);
            defaultViewModel["date"] = "更新时间 " + DateTime.Now;
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            is_loading = false;
            loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;
            StatusesReform.reform(statuses, ss);
            defaultViewModel["date"] = "更新时间 " + DateTime.Now;
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            defaultViewModel["statuses"] = statuses;
            defaultViewModel["mentions"] = mentions;
            defaultViewModel["publics"] = publics;

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            defaultViewModel["currentUser"] = currentUser;

            loading.Visibility = Visibility.Visible;
            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
                    defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
                    defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    defaultViewModel["title"] = "随便看看";
                    break;
                default:
                    break;
            }

            defaultViewModel["page"] = "第1页";
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            currentClick = e.ClickedItem as Status;
            status.setStatus(currentClick);
            statusPopup.IsOpen = true;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            sendPopup.IsOpen = true;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;

            switch (currentType)
            {
                case PageType.Statuses:
                    FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, since_id: statuses.First().id);
                    defaultViewModel["title"] = "我的消息";
                    break;
                case PageType.Mentions:
                    FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, since_id: mentions.First().id);
                    defaultViewModel["title"] = "提及我的";
                    break;
                case PageType.Publics:
                    FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60, 1);
                    defaultViewModel["title"] = "随便看看";
                    break;
                default:
                    break;
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void send_BackClick(object sender, BackClickEventArgs e)
        {
            sendPopup.IsOpen = false;
        }

        private void FavAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (TimelinePage),
                new KeyValuePair<TimelinePage.PageType, object>(TimelinePage.PageType.Favorite,
                    FanfouAPI.FanfouAPI.Instance.currentUser));
        }

        private void pageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(pageTitle);
        }

        private void StatusesMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Statuses;

            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
            defaultViewModel["title"] = "我的消息";
            statusesGridView.ItemsSource = defaultViewModel["statuses"];
        }

        private void MentionsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Mentions;

            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
            defaultViewModel["title"] = "提及我的";
            statusesGridView.ItemsSource = defaultViewModel["mentions"];
        }

        private void PublicsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            currentType = PageType.Publics;

            FanfouAPI.FanfouAPI.Instance.StatusPublicTimeline(60);
            defaultViewModel["title"] = "随便看看";
            statusesGridView.ItemsSource = defaultViewModel["publics"];
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SearchPage));
        }

        private void DirectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (DirectMessagePage));
        }

        private void status_BackClick(object sender, BackClickEventArgs e)
        {
            statusPopup.IsOpen = false;
        }

        private void MenuFlyout_Closed(object sender, object e)
        {
            rotation.Rotation = 0;
        }

        private void MenuFlyout_Opened(object sender, object e)
        {
            rotation.Rotation = 180;
        }

        private void SelfButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SelfPage));
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FindPage));
        }

        #region NavigationHelper 注册

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}