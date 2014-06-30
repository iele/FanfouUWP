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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace FanfouWP2.CustomControl
{
    public sealed partial class StatusSettingsFlyout : SettingsFlyout
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Status status;
        private ObservableCollection<Status> context = new ObservableCollection<Status>();

        public delegate void UserButtonClickHandler(object sender, RoutedEventArgs e);
        public event UserButtonClickHandler UserButtonClick;
        public delegate void ReplyButtonClickHandler(object sender, RoutedEventArgs e);
        public event ReplyButtonClickHandler ReplyButtonClick;
        public delegate void RepostButtonClickHandler(object sender, RoutedEventArgs e);
        public event RepostButtonClickHandler RepostButtonClick;
        public delegate void FavButtonClickHandler(object sender, RoutedEventArgs e);
        public event FavButtonClickHandler FavButtonClick;
    

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        public StatusSettingsFlyout()
        {
            this.InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.ContextTimelineSuccess += Instance_ContextTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.ContextTimelineFailed += Instance_ContextTimelineFailed;
        }

        void Instance_ContextTimelineFailed(object sender, FailedEventArgs e)
        {
        }

        void Instance_ContextTimelineSuccess(object sender, EventArgs e)
        {
            this.contextTextBlock.Visibility = Visibility.Visible;
            var ss = sender as List<Status>;
            foreach (var item in ss)
            {
                this.context.Add(item);
            }
        }

        public void setStatus(Status status)
        {
            this.status = status;
            this.context.Clear();
            this.contextTextBlock.Visibility = Visibility.Collapsed;
          
            this.defaultViewModel["status"] = status;
            this.defaultViewModel["context"] = context;
          
            if (this.status.in_reply_to_status_id != "" || this.status.in_reply_to_screen_name != "")
            {
                FanfouAPI.FanfouAPI.Instance.StatusContextTimeline(this.status.id);
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.UserButtonClick != null)
                this.UserButtonClick(sender, e);
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ReplyButtonClick != null)
                this.ReplyButtonClick(sender, e);     
        }

        private void RepostButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.RepostButtonClick != null)
                this.RepostButtonClick(sender, e);      
        }

        private void FavButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.FavButtonClick != null)
                this.FavButtonClick(sender, e);      
        }
    }
}
