using System;
using System.Collections.ObjectModel;

namespace FanfouWP2.FanfouAPI
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