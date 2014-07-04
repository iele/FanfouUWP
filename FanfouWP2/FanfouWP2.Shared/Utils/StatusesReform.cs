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
            if (list.Count == 0)
                return;

            list.Reverse();
            foreach (var i in list)
            {
                for (var k = statuses.Count - 1; k == 0; k--)
                {
                    if (i.rawid > statuses[k].rawid)
                    {
                        statuses.Insert(statuses.IndexOf(statuses[k]), i);
                        goto inserted;
                    }
                }
                var ss = from s in statuses where s.id == i.id select s;
                if (ss.Count() == 0)
                    statuses.Insert(0, i);
            inserted:
                continue;
            }
        }
    }
}
