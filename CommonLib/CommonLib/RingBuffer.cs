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
        private PooledBuffer _buffer;
        private int _readerIndex;
        private int _headerIndex;
        private int _size;
        private readonly int _maxCapacity;

       
        public RingBuffer(int capacity, int maxCapacity)
        {
            if (capacity > maxCapacity)
            {
                throw new ArgumentException("capacity cannot be greater than maxCapacity");
            }
            _buffer = new PooledBuffer(capacity);// new byte[capacity];
            _readerIndex = 0;
            _headerIndex = 0;
            _size = 0;
            this._maxCapacity = maxCapacity;
        }
        public RingBuffer(int capacity) : this(capacity, capacity ) { }

        public int Capacity => _buffer.Capacity;
        public int Count => _size;
        public int ReaderIndex => _readerIndex;

        public void Enqueue(byte item)
        {
            if (_size == _buffer.Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }
                        
            _buffer[_headerIndex] = item;
            _headerIndex = NextIndex(_headerIndex);
            _size++;
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
            if (newCapacity > _maxCapacity)
            {
                throw new InvalidOperationException("Queue has reached maximum capacity");
            }

            PooledBuffer newBuffer = new PooledBuffer(newCapacity);

            _headerIndex = _size;

            while (_size != 0)
            {
                newBuffer.Append(Dequeue());
            }

            _buffer.Dispose();
            _buffer = newBuffer;

            _size = _headerIndex;
            _readerIndex = 0;
        }

        public byte Dequeue()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            byte item = _buffer[_readerIndex];
            _buffer[_readerIndex] = default;
            _readerIndex = NextIndex(_readerIndex);
            _size--;
            return item;
        }

        internal int NextIndex(int index)
        {
            return  (index + 1) % _buffer.Capacity;
        }
        

        public byte Peek()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _buffer[_readerIndex];
        }

        public void Clear()
        {
            //Array.Clear(buffer, 0, buffer.Length);
            _buffer.Clear();
            _readerIndex = 0;
            _headerIndex = 0;
            _size = 0;
        }

        public void Clear(int count)
        {
            if (count > _size)
            {
                throw new ArgumentException(nameof(count));
            }

            for(int i=0; i < count;++i)
            {
                _readerIndex = NextIndex(_readerIndex);
            }
            
            _size -= count;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            while (bytesRead < count && _size > 0)
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

        public void Write(byte b)
        {
            Enqueue(b);
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                Enqueue(buffer[i]);
            }
        }


        private bool IsReadIndexValid(int index, int size)
        {
            for (int i = 0; i < size;i++)
            {
                if (!(index >= _readerIndex && index < _headerIndex))
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
            return  (short)((_buffer[index] << 8) | _buffer[NextIndex(index)]);
            //return XBitConverter.ByteArrayToShort(buffer[index], buffer[ NextIndex(index)]);
        }
        private int GetInt32(int index)
        {
            return (_buffer[index] << 24) | (_buffer[index = NextIndex(index)] << 16) | (_buffer[index = NextIndex(index)] << 8) |( _buffer[NextIndex(index)]);
            //return XBitConverter.ByteArrayToInt(buffer[index], buffer[index = NextIndex(index)], buffer[index = NextIndex(index)], buffer[ NextIndex(index)]);
        }


        public short PeekInt16(int index)
        {
            if (IsReadIndexValid(index, sizeof(short)))
            {
                return GetInt16(index);
            }
            else
            {
                throw new IndexOutOfRangeException(nameof(index));
            }
        }


        public int PeekInt32(int index)
        {
            if (IsReadIndexValid(index, sizeof(int)))
            {
                return GetInt32(index);
            }
            else
            {
                throw new IndexOutOfRangeException(nameof(index));
            }
        }

        public short ReadInt16()
        {
            
            short data = PeekInt16(_readerIndex);
            int count = sizeof(short);
            _readerIndex  = MoveIndex(_readerIndex, count);
            _size -= count;
            return data;
        }

        public int ReadInt32()
        {
            int data = PeekInt32(_readerIndex);
            int count = sizeof(int);
            _readerIndex = MoveIndex(_readerIndex, count);
            _size -= count;
            return data;
        }


        public void  SetInt16(int index,short value)
        {
            if(!IsReadIndexValid(index,sizeof(short)))
            {
                throw new IndexOutOfRangeException();
            }

            SetByte(index, (byte)((value >> 8) & 0xFF));
            SetByte(NextIndex(index), (byte)(value & 0xFF));
        }

        public int WriteInt16(short value)
        {

            int count = sizeof(short);

            if (_size + count > Capacity)
            {
                ResizeBuffer(_buffer.Capacity *2);
            }

            int startIndex = _headerIndex;

            //XBitConverter.ShortToByteArray(value, this);

            Enqueue((byte)((value >> 8) & 0xFF));  // 상위 바이트
            Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)

            //tailIndex = MoveIndex(tailIndex, count);
            //size += count;

            return startIndex;
        }

        public int WriteInt32(int value)
        {
            int count = sizeof(int);

            if (_size + count > Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }

            int startIndex = _headerIndex;


            Enqueue((byte)((value >> 24) & 0xFF)); // 상위 바이트 (1번째 바이트)
            Enqueue((byte)((value >> 16) & 0xFF)); // 2번째 바이트
            Enqueue((byte)((value >> 8) & 0xFF));  // 3번째 바이트
            Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)


            //tailIndex = MoveIndex(tailIndex, count);            
            //size += count;

            return startIndex;
        }

        public byte[] Buffer()
        {
            return _buffer.Data;
        }

        public void Read(PooledBuffer body,int count)
        {
            for(int i=0; i < count; i++)
            {
                body.Append(Dequeue());
            }
        }

        internal void SetByte(int index, byte value)
        {
            if(index > _buffer.Capacity)
            {
                throw new IndexOutOfRangeException();
            }
            _buffer[index] = value;

        }

        public byte ReadByte()
        {
            return Dequeue();
        }
    }
}
