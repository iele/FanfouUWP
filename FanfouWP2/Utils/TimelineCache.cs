using FanfouWP2.Common;
using FanfouWP2.FanfouAPI.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace FanfouWP2.Utils
{
    public class TimelineCache
    {
        public readonly PaginatedCollection<Status> mentions = new PaginatedCollection<Status>();
        public readonly PaginatedCollection<Status> statuses = new PaginatedCollection<Status>();

        private static TimelineCache instance;

        private TimelineStorage<Status> storage = new TimelineStorage<Status>();

        public static TimelineCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimelineCache();
                }
                return instance;
            }
            set { instance = value; }
        }

        public void FindAndDelete(Status status)
        {
            var i = from s in statuses where status.id == s.id select s;
            if (i.Count() != 0)
            {
                foreach (var item in i.ToArray())
                {
                    statuses.Remove(item);
                }
            }
            i = from s in mentions where status.id == s.id select s;
            if (i.Count() != 0)
            {
                foreach (var item in i.ToArray())
                {
                    mentions.Remove(item);
                }
            }
        }

        public void FindAndChange(Status status)
        {
            var i = from s in statuses where status.id == s.id select s;
            if (i.Count() != 0)
            {
                foreach (var item in i.ToArray())
                {
                    statuses[statuses.IndexOf(item)] = status;
                }
            }
            i = from s in mentions where status.id == s.id select s;
            if (i.Count() != 0)
            {
                foreach (var item in i.ToArray())
                {
                    mentions[mentions.IndexOf(item)] = status;
                }
            }
        }
    }
}
