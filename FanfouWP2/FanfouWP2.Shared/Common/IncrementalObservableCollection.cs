using FanfouWP2.FanfouAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FanfouWP2
{
    public class IncrementalObservableCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : Item
    {
        public Action action;
        bool ISupportIncrementalLoading.HasMoreItems
        {
            get { return true; }
        }
        bool _busy = false;
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                await Task.Run(() => action());
            }
            finally
            {
                _busy = false;
            }
            return new LoadMoreItemsResult();
        }
        Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> ISupportIncrementalLoading.LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }
            _busy = true;
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }
    }
}
