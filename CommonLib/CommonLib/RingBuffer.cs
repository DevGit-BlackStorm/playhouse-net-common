using System;
using System.IO;
using PlayHouse;

namespace CommonLib
{
    //public class RingBufferStream : Stream
    //{
    //    private readonly RingBuffer _queue;

    //    public RingBufferStream(RingBuffer queue)
    //    {
    //        this._queue = queue;
    //    }

    //    public override bool CanRead => true;

    //    public override bool CanSeek => false;

    //    public override bool CanWrite => true;

    //    public override long Length => _queue.Count;

    //    public override long Position
    //    {
    //        get => throw new NotSupportedException();
    //        set => throw new NotSupportedException();
    //    }

    //    public override void Flush()
    //    {
    //    }

    //    public override int Read(byte[] buffer, int offset, int count)
    //    {
    //        return _queue.Read(buffer, offset, count);
    //    }

    //    public override long Seek(long offset, SeekOrigin origin)
    //    {
    //        throw new NotSupportedException();
    //    }

    //    public override void SetLength(long value)
    //    {
    //        throw new NotSupportedException();
    //    }

    //    public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        _queue.Write(buffer, offset, count);
    //    }
    //}


    public class RingBuffer : PooledByteBuffer
    {
        public RingBuffer(int capacity) : base(capacity)
        {
        }

        public RingBuffer(int capacity, int maxCapacity) : base(capacity, maxCapacity)
        {
        }

        public int PeekNextIndex(int offSet)
        {
            var readerIndex = ReaderIndex;
            for (var i = 0; i < offSet; i++)
            {
                readerIndex = NextIndex(readerIndex);
            }

            return readerIndex;
        }

        protected internal override int NextIndex(int index)
        {
            return (index + 1) % Capacity;
        }
    }
}