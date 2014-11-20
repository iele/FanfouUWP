using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.Utils
{
    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public Func<uint, Task<IEnumerable<T>>> load;

        private bool is_loading = false;
        public bool HasMoreItems { get; protected set; }

        public PaginatedCollection()
        {
            HasMoreItems = true;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run(async c =>
            {
                if (!is_loading)
                {
                    is_loading = true;
                    try
                    {
                        var data = await load(count);

                        foreach (var item in data)
                        {
                            Add(item);
                        }

                        //HasMoreItems = data.Any();

                        return new LoadMoreItemsResult() { Count = (uint)data.Count() };
                    }
                    finally
                    {
                        is_loading = false;
                    }
                }
                else
                {
                    return new LoadMoreItemsResult() { Count = 0 };
                }
            });
        }
    }
}
