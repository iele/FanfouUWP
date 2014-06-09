using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP2.FanfouAPI
{
    public class FailedEventArgs : EventArgs
    {
        private Error _error;
        public Error error
        {
            get {
                return _error;
            }
            set
            {
                if (value == null || value.error == null)
                {
                    value = new Error();
                    value.error = "";
                } _error = value;
        } }


        public FailedEventArgs() {
            this.error = new Error();
        }
        public FailedEventArgs(Error error)
        {
            this.error = error;
        }
    }
}
