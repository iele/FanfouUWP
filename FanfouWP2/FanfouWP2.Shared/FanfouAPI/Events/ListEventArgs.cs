using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class ListEventArgs<T> : EventArgs
    {
        public List<T> Result { get; set; }

        public ListEventArgs() { }
        public ListEventArgs(List<T> Result)
        {
            this.Result = Result;        
        }
    }
}
