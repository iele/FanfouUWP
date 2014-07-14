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

            if (statuses.Count == 0)
            {
                foreach (var item in list)
                    statuses.Add(item);
                return;
            }
            foreach (var i in list)
            {
                var eq = new ObservableCollection<Status>();
                var bt = new ObservableCollection<Status>();
                var lt = new ObservableCollection<Status>();

                foreach (var s in statuses)
                {
                    if (s.rawid == i.rawid)
                    {
                        eq.Add(s);
                    } if (s.rawid <= i.rawid)
                    {
                        bt.Add(s);
                    }
                    if (s.rawid >= i.rawid)
                    {
                        lt.Add(s);
                    }

                }

                if (eq.Count() != 0)
                {
                    goto inserted;
                }
                if (bt.Count() == 0)
                {
                    statuses.Add(i);
                    goto inserted;
                }
                if (lt.Count() == 0)
                {
                    statuses.Insert(0, i);
                    goto inserted;
                }

                for (var k = statuses.Count - 1; k == 0; k--)
                {
                    if (i.rawid > statuses[k].rawid)
                    {
                        statuses.Insert(statuses.IndexOf(statuses[k]), i);
                        goto inserted;
                    }
                }
            inserted:
                continue;
            }
        }
    }
}
