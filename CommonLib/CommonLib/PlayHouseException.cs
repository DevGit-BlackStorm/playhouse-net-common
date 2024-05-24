using System;

namespace PlayHouse
{
    public class PlayHouseException : Exception
    {
        public readonly ushort ErrorCode;

        public PlayHouseException(string message, ushort errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}