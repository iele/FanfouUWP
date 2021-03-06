﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FanfouUWP.Common
{
    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public bool is_loading = false;
        public Func<int, Task<int?>> load;

        private DispatcherTimer dispatcherTimer;

        public PaginatedCollection()
        {
            HasMoreItems = true;
        }

        public bool HasMoreItems { get; set; }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async c =>
            {
                if (!is_loading)
                {
                    is_loading = true;
                    try
                    {
                        var ct = await load((int)count);
                        return new LoadMoreItemsResult() { Count = (uint)ct };
                    }
                    catch (Exception e)
                    {
                        return new LoadMoreItemsResult() { Count = 0 };
                    }
                    finally
                    {

                        if (dispatcherTimer == null)
                        {
                            dispatcherTimer = new DispatcherTimer();
                            dispatcherTimer.Tick += (s, ev) =>
                            {
                                is_loading = false;
                                dispatcherTimer.Stop();
                            };
                            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                            dispatcherTimer.Start();
                        }

                        is_loading = false;
                    }
                }
                return new LoadMoreItemsResult() { Count = 0 };
            });
        }
    }
}