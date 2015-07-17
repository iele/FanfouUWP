using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.Utils
{
    public static class StatusesReform
    {
        public static void append(ObservableCollection<Status> statuses, List<Status> list)
        {
            if (statuses != null && list != null)
            {
                foreach (var item in list)
                {
                    if ((from s in statuses where s.id == item.id select s).Count() == 0)
                        statuses.Add(item);
                }
            }
        }

        public static void insertFirst(ObservableCollection<Status> statuses, List<Status> list)
        {
            if (statuses != null && list != null)
            {
                list.Reverse();
                if (list.Count >= 20)
                {
                    statuses.Insert(0, new Status { id = Guid.NewGuid().ToString(), is_refresh = true });
                }

                foreach (Status i in list)
                {
                    if ((from s in statuses where s.id == i.id select s).Count() == 0)
                        statuses.Insert(0, i);
                }
            }

            if (statuses.Count > 0)
            {
                if (statuses[0].is_refresh == true)
                    statuses.Remove(statuses[0]);
                if (statuses[statuses.Count - 1].is_refresh == true)
                    statuses.Remove(statuses[statuses.Count - 1]);
                for (int i = 0; i < statuses.Count - 1; i++)
                {
                    if (statuses[i].is_refresh == true && statuses[i + 1].is_refresh == true)
                    {
                        statuses.Remove(statuses[i + 1]);
                        i += 1;
                    }
                }
            }
        }

        public static void insertBetween(ObservableCollection<Status> statuses, List<Status> list, string id_prev, int count)
        {
            if (statuses != null && list != null)
            {
                int index = 0;

                for (int i = 0; i < statuses.Count; i++)
                {
                    if (statuses[i].id == id_prev)
                        index = i;
                }
                index++;

                int c = 0;
                list.Reverse();
                foreach (Status i in list)
                {
                    c++;
                    statuses.Insert(index, i);
                }

                if (list.Count >= count)
                {
                    statuses.Insert(index + c, new Status { id = Guid.NewGuid().ToString(), is_refresh = true });
                }
            }

            if (statuses.Count > 0)
            {
                if (statuses[0].is_refresh == true)
                    statuses.Remove(statuses[0]);
                if (statuses[statuses.Count - 1].is_refresh == true)
                    statuses.Remove(statuses[statuses.Count - 1]);
                for (int i = 0; i < statuses.Count - 1; i++)
                {
                    if (statuses[i].is_refresh == true && statuses[i + 1].is_refresh == true)
                    {
                        statuses.Remove(statuses[i + 1]);
                        i += 1;
                    }
                }
            }
        }

        [System.Obsolete("use other methods", true)]
        public static void reform(ObservableCollection<Status> statuses, List<Status> list)
        {
            lock (new Object())
            {
                if (list.Count == 0)
                {
                    goto end;
                }
                else if (statuses.Count == 0)
                {
                    foreach (Status item in list)
                        statuses.Add(item);
                    goto end;
                }
                else if (list.Last().rawid > statuses.First().rawid)
                {
                    list.Reverse();
                    if (list.Count >= 20)
                    {
                        statuses.Insert(0, new Status { is_refresh = true });
                    }

                    foreach (Status i in list)
                    {
                        if ((from s in statuses where s.id == i.id select s).Count() == 0)
                            statuses.Insert(0, i);
                    }
                    goto end;

                }
                else if (list.First().rawid < statuses.Last().rawid)
                {
                    foreach (Status i in list)
                    {
                        if ((from s in statuses where s.id == i.id select s).Count() == 0)
                            statuses.Add(i);
                    }
                    goto end;
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < statuses.Count; j++)
                        {
                            if (list[i].rawid < statuses[j].rawid)
                                continue;
                            if (list[i].rawid > statuses[j].rawid)
                            {
                                statuses.Insert(j, list[i]);
                                break;
                            }
                            if (list[i].rawid == statuses[j].rawid)
                                goto equal;
                        }

                        equal:
                        ;
                    }
                    if (list.Count >= 20)
                    {
                        for (int k = 0; k < statuses.Count; k++)
                        {
                            if (list.Last().rawid > statuses[k].rawid)
                            {

                                statuses.Insert(k, new Status { is_refresh = true });
                            }
                            break;
                        }
                    }
                    goto end;
                }

                end:
                if (statuses.Count > 0)
                {
                    if (statuses[0].is_refresh == true)
                        statuses.Remove(statuses[0]);
                    if (statuses[statuses.Count - 1].is_refresh == true)
                        statuses.Remove(statuses[statuses.Count - 1]);
                    for (int i = 0; i < statuses.Count - 1; i++)
                    {
                        if (statuses[i].is_refresh == true && statuses[i + 1].is_refresh == true)
                        {
                            statuses.Remove(statuses[i + 1]);
                            i += 1;
                        }
                    }
                }
            }
        }
    }
}