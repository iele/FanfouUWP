﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;

using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;

namespace FanfouUWP
{
    public sealed partial class TagUserPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private readonly PaginatedCollection<User> users = new PaginatedCollection<User>();
        private string tag;
        private int page = 1;

        public TagUserPage()
        {
            InitializeComponent();

            users.load = async (c) =>
            {
                try
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.Tagged(tag, SettingStorage.Instance.messageSize, ++page);
                    if (result.Count == 0)
                        users.HasMoreItems = false;

                    foreach (User i in result)
                    {
                        users.Add(i);
                    }

                    return result.Count;
                }
                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                    return 0;
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
            defaultViewModel["users"] = users;
            tag = e.NavigationParameter as string;

            title.Text = tag;

            page = 1;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.Tagged(tag, SettingStorage.Instance.messageSize, page);

                users.Clear();
                foreach (User i in ss)
                {
                    users.Add(i);
                }
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            page = 1;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.Tagged(tag, SettingStorage.Instance.messageSize, page);

                users.Clear();
                foreach (User i in ss)
                {
                    users.Add(i);
                }
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }
        }

        private void usersGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert((e.ClickedItem as User)));
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
    }
}