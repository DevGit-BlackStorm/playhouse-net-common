using CommonLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayHouse
{
    public class PooledByteBuffer : IDisposable
    {
        private PooledBuffer _buffer;
        private int _readerIndex;
        private int _headerIndex;
        private int _size;
        private readonly int _maxCapacity;

        public PooledByteBuffer(int capacity, int maxCapacity)
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
        public PooledByteBuffer(int capacity) : this(capacity, capacity) { }

        public int Capacity => _buffer.Capacity;
        public int Count => _size;
        public int ReaderIndex => _readerIndex;

        public Span<byte> AsSpan()
        {
            return _buffer.AsSpan();
        }
        public Memory<byte> AsMemory()
        {
            return new Memory<byte>(_buffer.Data, _readerIndex,_size);
            //return new Memory<byte>(_buffer.Data, 0, _size);
        }

        virtual internal protected int NextIndex(int index)
        {
            if(index + 1 > _buffer.Capacity)
            {
                throw new Exception("index is over");
            }
            return (index + 1);
        }


        private int MoveIndex(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                index = NextIndex(index);
            }
            return index;
        }
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

        public long Append(byte data)
        {
            return _buffer.Append(data);
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

        public void WriteCount(int count)
        {
            if (_size + count > _buffer.Capacity)
            {
                throw new InvalidOperationException("Queue has reached maximum capacity");
            }

            _size += count;
        }

        public void Clear(int count)
        {
            if (count > _size)
            {
                throw new ArgumentException(nameof(count));
            }

            for (int i = 0; i < count; ++i)
            {
                _readerIndex = NextIndex(_readerIndex);
            }
            //_readerIndex += count;

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
        public void Write(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                Enqueue(buffer[i]);
            }
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

        private byte GetByte(int index)
        {
            if(index < 0 || index > _buffer.Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _buffer[index];  
        }

        private ushort GetInt16(int index)
        {
            return  (ushort)((GetByte(index) << 8) | GetByte(NextIndex(index)));
        }
        private int GetInt32(int index)
        {
            return (GetByte(index) << 24) | (GetByte(index = NextIndex(index)) << 16) | (GetByte(index = NextIndex(index)) << 8) | (GetByte(NextIndex(index)));
        }


        public ushort PeekInt16(int index)
        {
            if (_size < sizeof(short)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int16");
            }

            return GetInt16(index);
        }

        public void Read(PooledByteBuffer body, int count)
        {
            for (int i = 0; i < count; i++)
            {
                body.Enqueue(Dequeue());
            }
        }


        public int PeekInt32(int index)
        {
            if (_size < sizeof(int)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int32");
            }

            return GetInt32(index);
        }

        public ushort ReadInt16()
        {
            if (_size < sizeof(short)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int16");
            }

            ushort data = PeekInt16(_readerIndex);
            int count = sizeof(ushort);
            _readerIndex = MoveIndex(_readerIndex, count);
            _size -= count;
            return data;
        }

        public int ReadInt32()
        {
            if (_size < sizeof(int)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int32");
            }

            int data = PeekInt32(_readerIndex);
            int count = sizeof(int);
            _readerIndex = MoveIndex(_readerIndex, count);
            _size -= count;
            return data;
        }


        public void SetInt16(int index, short value)
        {
            SetByte(index, (byte)((value >> 8) & 0xFF));
            SetByte(NextIndex(index), (byte)(value & 0xFF));
        }

        public void SetByte(int index, byte value)
        {
            if (index < 0 || index > _buffer.Capacity )
            {
                throw new IndexOutOfRangeException();
            }
            _buffer[index] = value;

        }

        public int WriteInt16(ushort value)
        {

            int count = sizeof(ushort);

            if (_size + count > _buffer.Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }

            int startIndex = _headerIndex;

            Enqueue((byte)((value >> 8) & 0xFF));  // 상위 바이트
            Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)

            return startIndex;
        }

        public int WriteInt32(int value)
        {
            int count = sizeof(int);

            if (_size + count > _buffer.Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }

            int startIndex = _headerIndex;


            Enqueue((byte)((value >> 24) & 0xFF)); // 상위 바이트 (1번째 바이트)
            Enqueue((byte)((value >> 16) & 0xFF)); // 2번째 바이트
            Enqueue((byte)((value >> 8) & 0xFF));  // 3번째 바이트
            Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)


            return startIndex;
        }

        public byte[] Buffer()
        {
            return _buffer.Data;
        }
        public void Dispose()
        {
            _buffer.Dispose();
        }

        public byte ReadByte()
        {
            return Dequeue();
        }


    }
}
