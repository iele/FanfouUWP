using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using FanfouWP2.FanfouAPI;

namespace FanfouWP2
{
    public class IncrementalObservableCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : Item
    {
        private bool _busy;
        public Action<uint> action = e => { };

        bool ISupportIncrementalLoading.HasMoreItems
        {
            get { return true; }
        }

        IAsyncOperation<LoadMoreItemsResult> ISupportIncrementalLoading.LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }
            _busy = true;
            return AsyncInfo.Run(c => LoadMoreItemsAsync(c, count));
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                await Task.Run(() => action(count));
            }
            finally
            {
                _busy = false;
            }
            return new LoadMoreItemsResult {Count = count};
        }
    }
}