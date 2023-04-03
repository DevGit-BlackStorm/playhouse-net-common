using System;
using System.IO;

namespace CommonLib
{

    public class RingBufferStream : Stream
    {
        private readonly RingBuffer queue;

        public RingBufferStream(RingBuffer queue)
        {
            this.queue = queue;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => queue.Count;

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return queue.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
           queue.Write(buffer, offset, count);
        }
    }



    public class RingBuffer
    {
        private PooledBuffer buffer;
        private int headIndex;
        private int tailIndex;
        private int size;
        private readonly int maxCapacity;

       

        public RingBuffer(int capacity, int maxCapacity)
        {
            if (capacity > maxCapacity)
            {
                throw new ArgumentException("capacity cannot be greater than maxCapacity");
            }
            buffer = new PooledBuffer(capacity);// new byte[capacity];
            headIndex = 0;
            tailIndex = 0;
            size = 0;
            this.maxCapacity = maxCapacity;
        }
        public RingBuffer(int capacity) : this(capacity, capacity ) { }

        public int Capacity => buffer.Capacity;
        public int Count => size;

        public void Enqueue(byte item)
        {
            if (size == buffer.Capacity)
            {
                ResizeBuffer(buffer.Capacity * 2);
            }
                        
            buffer[tailIndex] = item;
            tailIndex = NextIndex(tailIndex);
            size++;
        }

        public void Enqueue(byte[] data)
        {
            foreach (byte b in data)
            {
                Enqueue(b);
            }
        }

        private void ResizeBuffer(int newCapacity)
        {
            if (newCapacity > maxCapacity)
            {
                throw new InvalidOperationException("Queue has reached maximum capacity");
            }

            PooledBuffer newBuffer = new PooledBuffer(newCapacity);

            tailIndex = size;

            while (size != 0)
            {
                newBuffer.Append(Dequeue());
            }

            buffer.Dispose();

            buffer = newBuffer;
            headIndex = 0;
        }

        public byte Dequeue()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            byte item = buffer[headIndex];
            buffer[headIndex] = default;
            headIndex = NextIndex(headIndex);
            size--;
            return item;
        }

        internal int NextIndex(int index)
        {
            return  (index + 1) % buffer.Capacity;
        }
        

        public byte Peek()
        {
            if (size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return buffer[headIndex];
        }

        public void Clear()
        {
            //Array.Clear(buffer, 0, buffer.Length);
            buffer.Clear();
            headIndex = 0;
            tailIndex = 0;
            size = 0;
        }

        public void Clear(int count)
        {
            if (count > size)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            for(int i=0; i < count;++i)
            {
                headIndex = NextIndex(headIndex);
            }
            
            size -= count;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            while (bytesRead < count && size > 0)
            {
                buffer[offset + bytesRead] = Dequeue();
                bytesRead++;
            }

            return bytesRead;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Enqueue(buffer[offset + i]);
            }
        }

        private bool IsReadIndexValid(int index, int size)
        {
            for (int i = 0; i < size;i++)
            {
                if (!(index >= headIndex && index < tailIndex))
                {
                    return false;
                }
                else
                {
                    index = NextIndex(index);
                }
            }
            return true;
        }

        private int MoveIndex(int index,int count)
        {
            for(int i = 0; i < count; i++)
            {
                index = NextIndex(index);
            }
            return index;
        }
        private short GetInt16(int index)
        {
            return XBitConverter.ByteArrayToShort(buffer[index], buffer[ NextIndex(index)]);
        }
        private int GetInt32(int index)
        {
            return XBitConverter.ByteArrayToInt(buffer[index], buffer[index = NextIndex(index)], buffer[index = NextIndex(index)], buffer[ NextIndex(index)]);
        }


        public short PeekInt16(int offset)
        {
            int index = headIndex + offset;
            if (IsReadIndexValid(index, sizeof(short)))
            {
                return GetInt16(index);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }


        public int PeekInt32(int offset)
        {
            int index = headIndex + offset;
            if (IsReadIndexValid(index, sizeof(int)))
            {
                return GetInt32(index);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public short ReadInt16()
        {
            
            short data = PeekInt16(0);
            int count = sizeof(short);
            headIndex  = MoveIndex(headIndex, count);
            size -= count;
            return data;
        }

        public int ReadInt32()
        {
            int data = PeekInt32(0);
            int count = sizeof(int);
            headIndex = MoveIndex(headIndex, count);
            size -= count;
            return data;
        }


        public void  ReplaceInt16(int index,short value)
        {
            if (index + sizeof(short) > tailIndex)
            {
                throw new ArgumentOutOfRangeException();
            }

            XBitConverter.ShortToByteArray(value, this, index);
        }

        public int WriteInt16(short value)
        {

            int count = sizeof(short);

            if (size + count > Capacity)
            {
                ResizeBuffer(buffer.Capacity *2);
            }

            int startIndex = tailIndex;

            XBitConverter.ShortToByteArray(value, this);

            //tailIndex = MoveIndex(tailIndex, count);
            //size += count;
            
            return startIndex;
        }

        public int WriteInt32(int value)
        {
            int count = sizeof(int);

            if (size + count > Capacity)
            {
                ResizeBuffer(buffer.Capacity * 2);
            }

            int startIndex = tailIndex;
            
            XBitConverter.IntToByteArray(value, this);

            //tailIndex = MoveIndex(tailIndex, count);            
            //size += count;

            return startIndex;
        }

        public byte[] Buffer()
        {
            return buffer.Data;
        }

        public void Read(PooledBuffer body,int count)
        {
            for(int i=0; i < count; i++)
            {
                body.Append(Dequeue());
            }
        }
    }
}
