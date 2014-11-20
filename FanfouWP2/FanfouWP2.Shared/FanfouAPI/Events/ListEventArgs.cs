using System;
using System.Collections.Generic;

namespace FanfouWP2.FanfouAPI
{
    public class ListEventArgs<T> : EventArgs
    {
        public ListEventArgs()
        {
        }

        public ListEventArgs(List<T> Result)
        {
            this.Result = Result;
        }

        public List<T> Result { get; set; }
    }
}