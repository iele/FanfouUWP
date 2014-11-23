using System;
using System.Collections.ObjectModel;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.FanfouAPI.Events
{
    public class UserTimelineEventArgs<T> : EventArgs where T : Item
    {
        public UserTimelineEventArgs()
        {
        }

        public UserTimelineEventArgs(ObservableCollection<T> UserStatus)
        {
            this.UserStatus = UserStatus;
        }

        public ObservableCollection<T> UserStatus { get; set; }
    }
}