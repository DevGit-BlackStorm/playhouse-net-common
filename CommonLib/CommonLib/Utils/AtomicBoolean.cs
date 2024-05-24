using System.Threading;

namespace PlayHouse.Utils
{
    public class AtomicBoolean
    {
        private int _value;

        public AtomicBoolean(bool initialValue)
        {
            _value = initialValue ? 1 : 0;
        }

        public bool CompareAndSet(bool expected, bool update)
        {
            var expectedValue = expected ? 1 : 0;
            var newValue = update ? 1 : 0;
            return Interlocked.CompareExchange(ref _value, newValue, expectedValue) == expectedValue;
        }

        public bool Get()
        {
            return Interlocked.CompareExchange(ref _value, 0, 0) != 0;
        }

        public void Set(bool newValue)
        {
            Interlocked.Exchange(ref _value, newValue ? 1 : 0);
        }
    }
}