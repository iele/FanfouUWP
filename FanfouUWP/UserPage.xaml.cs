using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using FanfouUWP.Common;

using FanfouUWP.FanfouAPI.Items;
using FanfouUWP.Utils;
using FanfouUWP.ItemControl;
using Windows.UI.Popups;
using System.Linq;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace FanfouUWP
{
    /// <summary>
    ///     可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserPage : Page
    {
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper navigationHelper;

        private User user;
        private ObservableCollection<string> tags = new ObservableCollection<string>();

        private readonly PaginatedCollection<Status> statuses = new PaginatedCollection<Status>();
        private readonly PaginatedCollection<Status> favorites = new PaginatedCollection<Status>();
        private readonly PaginatedCollection<User> friends = new PaginatedCollection<User>();
        private readonly PaginatedCollection<User> followers = new PaginatedCollection<User>();

        private int favoritePage = 1;
        private int friendsPage = 1;
        private int followersPage = 1;

        public UserPage()
        {
            InitializeComponent();

            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += NavigationHelper_LoadState;
            navigationHelper.SaveState += NavigationHelper_SaveState;

            App.RootFrame.SizeChanged += RootFrame_SizeChanged;

            statuses.load = async (c) =>
            {
                if (statuses.Count > 0)
                {
                    try
                    {
                        var list = await FanfouAPI.FanfouAPI.Instance.StatusUserTimeline(user.id, c, max_id: statuses.Last().id);

                        if (list.Count == 0)
                            statuses.HasMoreItems = false;
                        Utils.StatusesReform.append(statuses, list);
                        return list.Count;
                    }
                    catch (Exception)
                    {
                        Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                        return 0;
                    }
                }
                return 0;
            };

            favorites.load = async (c) =>
            {
                try
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, ++favoritePage);
                    if (result.Count == 0)
                        favorites.HasMoreItems = false;
                    StatusesReform.append(favorites, result);

                    return result.Count;
                }
                catch (Exception)
                {
                    return 0;
                }
            };


            friends.load = async (c) =>
            {
                try
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.UsersFriends(user.id, 60, ++friendsPage);
                    if (result.Count == 0)
                        friends.HasMoreItems = false;

                    foreach (User i in result)
                    {
                        friends.Add(i);
                    }

                    return result.Count;
                }
                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                    return 0;
                }
            };

            followers.load = async (c) =>
            {
                try
                {
                    var result =
                        await FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 60, ++followersPage);
                    if (result.Count == 0)
                        followers.HasMoreItems = false;

                    foreach (User i in result)
                    {
                        followers.Add(i);
                    }

                    return result.Count;
                }
                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                    return 0;
                }
            };
        }

        private void RootFrame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkUserItem();
        }

        private void checkUserItem()
        {
            var width = App.RootFrame.RenderSize.Width;
            if (width > 1241)
            {
                if (pivot.Items.Contains(userItem))
                    pivot.Items.Remove(userItem);
            }
            else if (width > 841)
            {
                if (!pivot.Items.Contains(userItem))
                    pivot.Items.Add(userItem);
            }
            else {
                if (!pivot.Items.Contains(userItem))
                    pivot.Items.Add(userItem);
            }
        }

        /// <summary>
        ///     获取与此 <see cref="Page" /> 关联的 <see cref="NavigationHelper" />。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        /// <summary>
        ///     获取此 <see cref="Page" /> 的视图模型。
        ///     可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }

        /// <summary>
        ///     使用在导航过程中传递的内容填充页。  在从以前的会话
        ///     重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        ///     事件的来源; 通常为 <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     事件数据，其中既提供在最初请求此页时传递给
        ///     <see cref=" Frame.Navigate1(Type, Object)" /> 的导航参数，又提供
        ///     此页在以前会话期间保留的状态的
        ///     字典。 首次访问页面时，该状态将为 null。
        /// </param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            user = Utils.DataConverter<User>.Convert(e.NavigationParameter as string);

            if (user.screen_name == null || user.screen_name == "") {
                try
                {
                    user = await FanfouAPI.FanfouAPI.Instance.UsersShow(user.id);
                    defaultViewModel["user"] = user;
                }
                catch (Exception)
                {
                    Utils.ToastShow.ShowInformation("加载失败，请检查网络");
                }
            }

            defaultViewModel["user"] = user;
            defaultViewModel["tags"] = tags;

            checkUserItem();
            checkFriendship();

            statuses.Clear();
            favorites.Clear();
            followers.Clear();
            friends.Clear();
            tags.Clear();

            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.StatusUserTimeline(user.id, 60);
                statuses.Clear();
                StatusesReform.append(statuses, ss);
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，可能未公开");
            }

            try
            {
                tags.Clear();
                var list = await FanfouUWP.FanfouAPI.FanfouAPI.Instance.TaggedList(this.user.id);
                foreach (var item in list)
                    tags.Add(item);
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，可能未公开");
            }

            favoritePage = 1;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.FavoritesId(user.id, 60, favoritePage);
                favorites.Clear();
                StatusesReform.append(favorites, ss);
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，可能未公开");
            }

            friendsPage = 1;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.UsersFriends(user.id, 60, friendsPage);

                friends.Clear();
                foreach (User i in ss)
                {
                    friends.Add(i);
                }
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，可能未公开");
            }

            followersPage = 1;
            try
            {
                var ss = await FanfouAPI.FanfouAPI.Instance.UsersFollowers(user.id, 60, followersPage);

                followers.Clear();
                foreach (User i in ss)
                {
                    followers.Add(i);
                }
                defaultViewModel["date"] = DateTime.Now.ToString();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，可能未公开");
            }
        }

        private void checkFriendship()
        {
            if (user.following)
            {
                FriendItem.Label = "解除好友";
            }
            else
            {
                FriendItem.Label = "加为好友";
            }
        }

        /// <summary>
        ///     保留与此页关联的状态，以防挂起应用程序或
        ///     从导航缓存中放弃此页。值必须符合
        ///     <see cref="SuspensionManager.SessionState" /> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     提供要使用可序列化状态填充的空字典
        ///     的事件数据。
        /// </param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void SearchItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchUserPage), Utils.DataConverter<User>.Convert(user));
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

        private void StatusItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TimelineUserPage), Utils.DataConverter<User>.Convert(user));
        }

        private void ImageItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImageUserPage), Utils.DataConverter<User>.Convert(user));
        }

        private void ReplyItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SendPage), ((int)SendPage.SendMode.ReplyUser).ToString() + Utils.DataConverter<User>.Convert(user));
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

        private void Taglist_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(TagUserPage), e.ClickedItem as string);
        }

        private void DirectItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MessagePage), user.id);
        }

        private async void FriendItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (user.following)
                    user = await FanfouUWP.FanfouAPI.FanfouAPI.Instance.FriendshipDestroy(this.user.id);
                else
                    user = await FanfouUWP.FanfouAPI.FanfouAPI.Instance.FriendshipCreate(this.user.id);
                defaultViewModel["user"] = user;
                checkFriendship();
            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
            }
        }

        private async void BlackListItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                user = await FanfouUWP.FanfouAPI.FanfouAPI.Instance.BlockCreate(this.user.id);
                defaultViewModel["user"] = user;
                ToastShow.ShowInformation("已经将@" + user.screen_name + "加入黑名单,若解锁请在个人页面完成");

            }
            catch (Exception)
            {
                Utils.ToastShow.ShowInformation("加载失败，请检查网络");
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

        private void statusesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), DataConverter<Status>.Convert(e.ClickedItem as Status));
        }

        private void favoriteGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(StatusPage), DataConverter<Status>.Convert(e.ClickedItem as Status));
        }

        private void friendsidView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(e.ClickedItem as User));
        }

        private void followersGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), Utils.DataConverter<User>.Convert(e.ClickedItem as User));
        }
    }
}