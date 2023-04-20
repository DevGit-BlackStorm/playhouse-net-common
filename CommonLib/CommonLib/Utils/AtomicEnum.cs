using System;
using System.Threading;

namespace PlayHouse.Utils
{
    public class AtomicEnum<TEnum> where TEnum : Enum
    {
        private int _value;

        public AtomicEnum(TEnum initialValue)
        {
            _value = Convert.ToInt32(initialValue);
        }

        //public TEnum Value
        //{
        //    get => (TEnum)System.Enum.ToObject(typeof(TEnum), _value);
        //    set => Interlocked.Exchange(ref _value, Convert.ToInt32(value));
        //}
        //public void Set(TEnum value)
        //{
        //    int newIntValue = Convert.ToInt32(value);
        //    if (newIntValue != _value)
        //    {
        //        Interlocked.Exchange(ref _value, newIntValue);
        //    }
        //}
        public bool CompareAndSet(TEnum expectedValue, TEnum newValue)
        {
            int expectedIntValue = Convert.ToInt32(expectedValue);
            int newIntValue = Convert.ToInt32(newValue);
            return Interlocked.CompareExchange(ref _value, newIntValue, expectedIntValue) == expectedIntValue;
        }

        public TEnum Get()
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), _value);
        }

        public void Set(TEnum value)
        {
            Interlocked.Exchange(ref _value, Convert.ToInt32(value));
        }

        //public  ServerState Get()
        //{
        //    throw new NotImplementedException();
        //}
    }
}