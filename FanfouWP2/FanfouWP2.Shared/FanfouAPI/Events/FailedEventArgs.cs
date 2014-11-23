﻿using System;
using FanfouWP2.FanfouAPI.Items;

namespace FanfouWP2.FanfouAPI.Events
{
    public class FailedEventArgs : EventArgs
    {
        private Error _error;


        public FailedEventArgs()
        {
            error = new Error();
        }

        public FailedEventArgs(Error error)
        {
            this.error = error;
        }

        public Error error
        {
            get { return _error; }
            set
            {
                if (value == null || value.error == null)
                {
                    value = new Error();
                    value.error = "";
                }
                _error = value;
            }
        }
    }
}