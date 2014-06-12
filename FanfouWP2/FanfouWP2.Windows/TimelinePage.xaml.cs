using System;
using System.Runtime.InteropServices.WindowsRuntime;
using FanfouWP2.Common;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using FanfouWP2.FanfouAPI;
using System.Threading.Tasks;
using System.Collections.Generic;
using FanfouWP2.Utils;
using Windows.UI.Xaml.Controls.Primitives;

namespace FanfouWP2
{
    public sealed partial class TimelinePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ObservableCollection<Status> homeTimeline = new ObservableCollection<Status>();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        public TimelinePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel["statuses"] = homeTimeline;

            FanfouAPI.FanfouAPI.Instance.HomeTimelineSuccess += Instance_HomeTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.HomeTimelineFailed += Instance_HomeTimelineFailed;
            FanfouAPI.FanfouAPI.Instance.StatusHomeTimeline(60, 1);
        }

        void Instance_HomeTimelineFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_HomeTimelineSuccess(object sender, EventArgs e)
        {     
            foreach (var item in sender as List<Status>)
            {
                this.homeTimeline.Add(item);
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


        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Status;
            Frame.Navigate(typeof(StatusPage), item);
        }
    }
}
