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

            if (list.Last().rawid > statuses.Last().rawid)
            {
                list.Reverse();
                foreach (var i in list)
                {
                    statuses.Insert(0, i);
                }
                return;
            }

            if (list.First().rawid < statuses.First().rawid)
            {
                foreach (var i in list)
                {
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

                statuses.Insert(j, list[i]);
            equal:
                continue;
            }
        }
    }
}
