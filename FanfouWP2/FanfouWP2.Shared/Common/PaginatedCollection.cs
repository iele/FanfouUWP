using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.Common
{
    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private bool is_loading;
        public Func<uint, Task<IEnumerable<T>>> load;

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
                        IEnumerable<T> data = await load(count);

                        foreach (T item in data)
                        {
                            Add(item);
                        }

                        return new LoadMoreItemsResult { Count = (uint)data.Count() };
                    }
                    finally
                    {
                        is_loading = false;
                    }
                }
                return new LoadMoreItemsResult { Count = 0 };
            });
        }
    }
}