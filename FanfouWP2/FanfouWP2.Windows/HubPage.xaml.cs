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

//“中心页”项模板在 http://go.microsoft.com/fwlink/?LinkId=321224 上有介绍

namespace FanfouWP2
{
    /// <summary>
    /// 显示分组的项集合的页。
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> mentions = new ObservableCollection<Status>();

        /// <summary>
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper 在每页上用于协助导航和
        /// 进程生命期管理
        /// </summary>
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
            throw new NotImplementedException();
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


        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO:  将可绑定组集合分配到 this.DefaultViewModel["Groups"]    
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["mentions"] = mentions;

            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(20);
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(20);
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

    }
}
