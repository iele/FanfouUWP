using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace FanfouWP2.Utils
{
    public class StatusesReform
    {
        public static void reform(ObservableCollection<Status> statuses, List<Status> list)
        {
            lock (new Object())
            {
                if (list.Count == 0)
                    return;

                if (statuses.Count == 0)
                {
                    foreach (var item in list)
                        statuses.Add(item);
                    return;
                }

                if (list.Last().rawid > statuses.Last().rawid)
                {
                    list.Reverse();
                    foreach (var i in list)
                    {
                        if ((from s in statuses where s.id == i.id select s).Count() == 0)
                            statuses.Insert(0, i);
                    }
                    return;
                }

                if (list.First().rawid < statuses.First().rawid)
                {
                    foreach (var i in list)
                    {
                        if ((from s in statuses where s.id == i.id select s).Count() == 0)
                            statuses.Add(i);
                    }
                    return;
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var j = 0;
                    for (j = 0; j < statuses.Count; j++)
                    {
                        if (list[i].rawid < statuses[j].rawid)
                            continue;
                        if (list[i].rawid > statuses[j].rawid)
                            break;
                        if (list[i].rawid == statuses[j].rawid)
                            goto equal;
                    }

                    if ((from s in statuses where s.id == list[i].id select s).Count() == 0)
                        statuses.Insert(j, list[i]);
                equal:
                    continue;
                }
            }
        }
    }
}
