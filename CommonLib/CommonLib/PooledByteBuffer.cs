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


        public int MoveIndex(int index, int count)
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

        public void Write(string value)
        {
            // UTF-8로 인코딩할 때 예상되는 최대 바이트 수 계산
            int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);

            // 스택에 충분한 크기를 할당할 수 없으면 예외 발생
            if (maxByteCount > 1024) // 예제로 1024바이트를 한계로 설정
            {
                throw new InvalidOperationException("String too large for stack allocation");
            }

            // 스택에 메모리 할당
            Span<byte> buffer = stackalloc byte[maxByteCount];

            // 실제로 사용된 바이트 수
            int bytesUsed = Encoding.UTF8.GetBytes(value, buffer);

            // 버퍼에 바이트 쓰기
            for (int i = 0; i < bytesUsed; i++)
            {
                Enqueue(buffer[i]);
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

        private int GetInt24(int index)
        {
            return (GetByte(index) << 16) | (GetByte(index = NextIndex(index)) << 8) | (GetByte(NextIndex(index)));
        }
        private int GetInt32(int index)
        {
            return (GetByte(index) << 24) | (GetByte(index = NextIndex(index)) << 16) | (GetByte(index = NextIndex(index)) << 8) | (GetByte(NextIndex(index)));
        }

        
        private long GetInt64(int index)
        {
            return ((long)GetByte(index) << 56) |
                   ((long)GetByte(index = NextIndex(index)) << 48) |
                   ((long)GetByte(index = NextIndex(index)) << 40) |
                   ((long)GetByte(index = NextIndex(index)) << 32) |
                   ((long)GetByte(index = NextIndex(index)) << 24) |
                   ((long)GetByte(index = NextIndex(index)) << 16) |
                   ((long)GetByte(index = NextIndex(index)) << 8) |
                   GetByte(NextIndex(index)); // 여기에서만 long 캐스팅
        }

        public ushort PeekInt16(int index)
        {
            if (_size < sizeof(short)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int16");
            }

            return GetInt16(index);
        }

        public int PeekInt24(int index)
        {
            if (_size < 3) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int16");
            }

            return GetInt24(index);
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

        public long PeekInt64(int index)
        {
            if (_size < sizeof(long)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int32");
            }

            return GetInt64(index);
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

        public int ReadInt24()
        {
            if (_size < sizeof(int) - 1) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int32");
            }

            int data = PeekInt24(_readerIndex);
            int count = 3;
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

        public long ReadInt64()
        {
            if (_size < sizeof(long)) // 버퍼에 충분한 데이터가 있는지 확인
            {
                throw new InvalidOperationException("Not enough data in the buffer to read Int32");
            }

            long data = PeekInt64(_readerIndex);
            int count = sizeof(long);
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

        public int WriteInt24(int value)
        {
            int count = sizeof(int) - 1;

            if (_size + count > _buffer.Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }

            int startIndex = _headerIndex;


            Enqueue((byte)((value >> 16) & 0xFF)); // 2번째 바이트
            Enqueue((byte)((value >> 8) & 0xFF));  // 3번째 바이트
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



        public int WriteInt64(long value)
        {
            int count = sizeof(long);

            if (_size + count > _buffer.Capacity)
            {
                ResizeBuffer(_buffer.Capacity * 2);
            }

            int startIndex = _headerIndex;

            Enqueue((byte)((value >> 56) & 0xFF)); // 가장 상위 바이트
            Enqueue((byte)((value >> 48) & 0xFF));
            Enqueue((byte)((value >> 40) & 0xFF));
            Enqueue((byte)((value >> 32) & 0xFF));
            Enqueue((byte)((value >> 24) & 0xFF)); // 상위 바이트 (1번째 바이트)
            Enqueue((byte)((value >> 16) & 0xFF)); // 2번째 바이트
            Enqueue((byte)((value >> 8) & 0xFF));  // 3번째 바이트
            Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)


            return startIndex;
        }

        public string ReadString(int msgSize, Encoding encoding = null)
        {
            // encoding이 null이면 UTF-8로 설정
            encoding ??= Encoding.UTF8;

            // msgSize가 스택 할당에 안전한지 확인합니다.
            // 이 값은 예시이며, 애플리케이션의 필요와 제약에 맞게 조정해야 합니다.
            // 스택 오버플로우를 방지하기 위해 일반적으로 안전한 한계는 1024바이트 정도입니다.
            if (msgSize > 1024) // 예시 한계값, 필요에 따라 조정
            {
                throw new ArgumentException("메시지 크기가 스택 할당에 너무 큽니다.");
            }

            // 스택에 메모리를 할당합니다.
            Span<byte> stringBytes = stackalloc byte[msgSize];

            // 버퍼에서 stringBytes 스팬으로 바이트를 읽습니다.
            for (int i = 0; i < msgSize; i++)
            {
                if (_size == 0)
                {
                    throw new InvalidOperationException("버퍼에 충분한 데이터가 없습니다.");
                }
                stringBytes[i] = Dequeue();
            }

            // 지정된 인코딩을 사용하여 바이트를 문자열로 변환합니다.
            string result = encoding.GetString(stringBytes);
            return result;
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
