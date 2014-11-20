using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FanfouWP2.Common;
using FanfouWP2.FanfouAPI;

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace FanfouWP2.CustomControl
{
    public sealed partial class StatusSettingsFlyout : SettingsFlyout
    {
        public delegate void FavButtonClickHandler(object sender, RoutedEventArgs e);

        public delegate void FavCreateSuccessHandler(object sender, EventArgs e);

        public delegate void FavDestroySuccessHandler(object sender, EventArgs e);

        public delegate void ReplyButtonClickHandler(object sender, RoutedEventArgs e);

        public delegate void RepostButtonClickHandler(object sender, RoutedEventArgs e);

        public delegate void UserButtonClickHandler(object sender, RoutedEventArgs e);

        private readonly ObservableCollection<Status> context = new ObservableCollection<Status>();

        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private Status status;

        public StatusSettingsFlyout()
        {
            InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.ContextTimelineSuccess += Instance_ContextTimelineSuccess;
            FanfouAPI.FanfouAPI.Instance.ContextTimelineFailed += Instance_ContextTimelineFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesCreateSuccess += Instance_FavoritesCreateSuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesCreateFailed += Instance_FavoritesCreateFailed;

            FanfouAPI.FanfouAPI.Instance.FavoritesDestroySuccess += Instance_FavoritesDestroySuccess;
            FanfouAPI.FanfouAPI.Instance.FavoritesDestroyFailed += Instance_FavoritesDestroyFailed;
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        public event UserButtonClickHandler UserButtonClick;

        public event ReplyButtonClickHandler ReplyButtonClick;

        public event RepostButtonClickHandler RepostButtonClick;

        public event FavButtonClickHandler FavButtonClick;

        public event FavCreateSuccessHandler FavCreateSuccess;

        public event FavDestroySuccessHandler FavDestroySuccess;


        private void Instance_FavoritesDestroyFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_FavoritesDestroySuccess(object sender, EventArgs e)
        {
            status = sender as Status;
            FavButton.Icon = new SymbolIcon(Symbol.SolidStar);
            FavButton.Label = "收藏";
            if (FavDestroySuccess != null)
            {
                FavDestroySuccess(sender, e);
            }
        }

        private void Instance_FavoritesCreateFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_FavoritesCreateSuccess(object sender, EventArgs e)
        {
            status = sender as Status;
            FavButton.Icon = new SymbolIcon(Symbol.OutlineStar);
            FavButton.Label = "取消收藏";
            if (FavCreateSuccess != null)
            {
                FavCreateSuccess(sender, e);
            }
        }

        private void Instance_ContextTimelineFailed(object sender, FailedEventArgs e)
        {
        }

        private void Instance_ContextTimelineSuccess(object sender, EventArgs e)
        {
            contextTextBlock.Visibility = Visibility.Visible;
            var ss = sender as List<Status>;
            foreach (Status item in ss)
            {
                context.Add(item);
            }
        }

        public void setStatus(Status status)
        {
            this.status = status;
            context.Clear();
            contextTextBlock.Visibility = Visibility.Collapsed;

            defaultViewModel["status"] = status;
            defaultViewModel["context"] = context;

            if (status.favorited)
            {
                FavButton.Icon = new SymbolIcon(Symbol.OutlineStar);
                FavButton.Label = "取消收藏";
            }
            else
            {
                FavButton.Icon = new SymbolIcon(Symbol.SolidStar);
                FavButton.Label = "收藏";
            }

            if (this.status.in_reply_to_status_id != "" || this.status.in_reply_to_screen_name != "")
            {
                FanfouAPI.FanfouAPI.Instance.StatusContextTimeline(this.status.id);
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserButtonClick != null)
                UserButtonClick(sender, e);
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReplyButtonClick != null)
                ReplyButtonClick(sender, e);
        }

        private void RepostButton_Click(object sender, RoutedEventArgs e)
        {
            if (RepostButtonClick != null)
                RepostButtonClick(sender, e);
        }

        private void FavButton_Click(object sender, RoutedEventArgs e)
        {
            if (!status.favorited)
                FanfouAPI.FanfouAPI.Instance.FavoritesCreate(status.id);
            else
                FanfouAPI.FanfouAPI.Instance.FavoritesDestroy(status.id);

            if (FavButtonClick != null)
                FavButtonClick(sender, e);
        }
    }
}