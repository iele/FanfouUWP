using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml.Data;

namespace FanfouWP2.Common
{
    public class PaginatedCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public bool is_loading = false;
        public Func<Task> load;

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
                    try
                    {
                        load();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
                return new LoadMoreItemsResult() { Count = 0 };
            });
        }
    }
}