using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FanfouWP2
{
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private User currentUser = new User();
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> mentions = new ObservableCollection<Status>();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_HomeTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_HomeTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.MentionTimelineSuccess += Instance_MentionTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.MentionTimelineFailed += Instance_MentionTimelineFailed;
        }

        private void Instance_MentionTimelineFailed(object sender, FailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Instance_MentionTimelineSuccess(object sender, EventArgs e)
        {
            var ss = sender as List<Status>;
            this.mentions.Clear();
            foreach (var item in ss)
            {
                this.mentions.Add(item);
            }
        }

        private void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {
            var ss = sender as List<Status>;
            this.statuses.Clear();
            foreach (var item in ss)
            {
                this.statuses.Add(item);
            }
        }


        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["mentions"] = mentions;
         
            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            this.defaultViewModel["currentUser"] = currentUser;
            
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(30, 1);
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(30);
        }

        #region NavigationHelper 注册

        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// 
        /// 应将页面特有的逻辑放入用于
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 和 <see cref="GridCS.Common.NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void mentionsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }

        private void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            if (e.Section == this.statusesHubSection)
            {
                Frame.Navigate(typeof(TimelinePage));
            }
            else if (e.Section == this.mentionsHubSection)
            {
                Frame.Navigate(typeof(TimelinePage));
            }
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TimelinePage));
        }

    }
}
