using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace FanfouWP2.Utils
{
    public class StatusesReform
    {
        public static void reform(ObservableCollection<Status> statuses, List<Status> list)
        {
            list.Reverse();
            foreach (var item in list)
            {
                int i = 0;
                for (i = 0; i < statuses.Count(); i++)
                {
                    if (item.rawid > statuses[i].rawid)
                        break;
                }
                var l = from p in statuses where p.id == item.id select p;
                if (l.Count() == 0)
                    statuses.Insert(i, item);
            }
        }
    }
}
