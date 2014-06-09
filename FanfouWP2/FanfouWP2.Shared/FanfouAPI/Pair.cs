using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FanfouWP2
{
    public struct Pair<T1, T2>
    {
        T1 key;
        T2 value;

        public Pair(T1 key, T2 value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
