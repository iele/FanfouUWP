using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FanfouWP2.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FanfouWP2.FanfouAPI;
using System.Collections.ObjectModel;

namespace FanfouWP2
{
    public sealed partial class StatusPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Status status;
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private ObservableCollection<Status> context = new ObservableCollection<Status>();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public StatusPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            FanfouAPI.FanfouAPI.Instance.ContextTimelineSuccess += Instance_ContextTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.ContextTimelineFailed += Instance_ContextTimelineFailed;
            FanfouAPI.FanfouAPI.Instance.UserTimelineSuccess += Instance_UserTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.UserTimelineFailed += Instance_UserTimelineFailed;
        }

        void Instance_UserTimelineFailed(object sender, FailedEventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed;
        }

        void Instance_UserTimelineSuccess(object sender, EventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed; 
            var ss = sender as List<Status>;
            foreach (var item in ss)
            {
                this.statuses.Add(item);
            }
        }

        void Instance_ContextTimelineFailed(object sender, FailedEventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed;
        }

        void Instance_ContextTimelineSuccess(object sender, EventArgs e)
        {
            this.loading.Visibility = Visibility.Collapsed;
            var ss = sender as List<Status>;           
            foreach (var item in ss) 
            {
                this.context.Add(item);
            }
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            status = e.NavigationParameter as Status;

            this.defaultViewModel["status"] = status;
            this.defaultViewModel["statuses"] = statuses;
            this.defaultViewModel["context"] = context;

            this.loading.Visibility = Visibility.Visible;
            FanfouAPI.FanfouAPI.Instance.StatusUserTimeline(this.status.user.id, 20);      
            if (this.status.in_reply_to_status_id != "" || this.status.in_reply_to_screen_name != "")
            {
                FanfouAPI.FanfouAPI.Instance.StatusContextTimeline(this.status.id);
            }
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