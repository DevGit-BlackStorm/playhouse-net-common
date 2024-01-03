﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayHouse.Utils
{
    public class AtomicShort
    {
        private int atomicInteger = 0;
        public ushort Get()
        {
            return (ushort)Interlocked.CompareExchange(ref atomicInteger, 0, 0);
        }

        public ushort IncrementAndGet()
        {
            int current;
            int next;
            do
            {
                current = atomicInteger;
                next = (current + 1) & ushort.MaxValue;
                if(next == 0)
                {
                    next = 1;
                }
            }
            while (Interlocked.CompareExchange(ref atomicInteger, next, current) != current);
            return (ushort)next;
        }

        public void Clear()
        {
            Interlocked.Exchange(ref atomicInteger,  0);
        }
    }

}
