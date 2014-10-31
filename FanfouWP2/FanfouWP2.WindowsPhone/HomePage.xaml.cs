using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FanfouWP2
{
    public sealed partial class HomePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private User currentUser = new User();
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> mentions = new ObservableCollection<Status>();

        public HomePage()
        {
            this.InitializeComponent();

            this.Loaded += HomePage_Loaded;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;


            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_HomeTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_HomeTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.MentionTimelineSuccess += Instance_MentionTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.MentionTimelineFailed += Instance_MentionTimelineFailed;
        }
        void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            //ScrollViewer sv = Utils.VisualHelper.FindVisualChildByName<ScrollViewer>(this.statusesGridView, "ScrollViewer");
            //ScrollBar sb = Utils.VisualHelper.FindVisualChildByName<ScrollBar>(sv, "HorizontalScrollBar");
            //sb.ValueChanged += sb_ValueChanged;

        }

        bool is_loading = false;
        bool can_loading = false;
        void sb_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            can_loading = true;
        }
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["mentions"] = mentions;

            currentUser = FanfouAPI.FanfouAPI.Instance.currentUser;
            this.defaultViewModel["currentUser"] = currentUser;

            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);
        }

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
            Utils.StatusesReform.reform(this.mentions, ss);
            this.defaultViewModel["date"] = "更新时间 " + DateTime.Now.ToString();
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
            Utils.StatusesReform.reform(this.statuses, ss);
            this.defaultViewModel["date"] = "更新时间 " + DateTime.Now.ToString();
        }

        private void PublicItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PublicPage));
        }

        private void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
            FanfouAPI.FanfouAPI.Instance.StatusMentionTimeline(60, 1);     
        }

        private void FavoriteGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavoritePage), this.currentUser);
        }

        private void SearchGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));    
        }

        private void FindGrid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(FindPage));  
        }

       private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.hub.ScrollToSection(this.hub.Sections.First());
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
    }
}
