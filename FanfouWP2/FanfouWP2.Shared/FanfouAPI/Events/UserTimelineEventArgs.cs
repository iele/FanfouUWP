using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class UserTimelineEventArgs<T> : EventArgs where T : Item
    {
        public ObservableCollection<T> UserStatus { get; set; }

        public UserTimelineEventArgs() { }
        public UserTimelineEventArgs(ObservableCollection<T> UserStatus)
        {
            this.UserStatus = UserStatus;
        }
    }
}
