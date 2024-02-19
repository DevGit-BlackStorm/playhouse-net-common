using CoreWCF.Channels;
using System;
using System.Diagnostics;
using System.Text;

namespace CommonLib
{
    public class PooledBuffer : IDisposable
    {

        private static BufferManager? _bufferManager;

        public static void Init(int maxBufferPoolSize = 1024 * 1024 * 100)
        {
            _bufferManager = BufferManager.CreateBufferManager(maxBufferPoolSize, 64*1024);
        }



        private byte[]? _data;
        private int _size;
        private int _offset;

        private bool _isPooled = true;
        /// <summary>
        /// Is the buffer empty?
        /// </summary>
        public bool IsEmpty => _data == null || _size == 0;
        /// <summary>
        /// Bytes memory buffer
        /// </summary>
        public byte[] Data => _data!;
        /// <summary>
        /// Bytes memory buffer capacity
        /// </summary>
        public int Capacity => _data!.Length;
        /// <summary>
        /// Bytes memory buffer size
        /// </summary>
        public int Size => _size;
        /// <summary>
        /// Bytes memory buffer offset
        /// </summary>
        public int Offset => _offset;

        /// <summary>
        /// Buffer indexer operator
        /// </summary>
        //public byte this[int index] => _data![index];

        public byte this[int index]
        {
            get => _data![index];
            set => _data![index] = value;
        }

        /// <summary>
        /// Initialize a new expandable buffer with zero capacity
        /// </summary>
        public PooledBuffer()
        {
            //_data = new byte[0]; 
            _data = _bufferManager!.TakeBuffer(0); _size = 0; _offset = 0;
        }
        /// <summary>
        /// Initialize a new expandable buffer with the given capacity
        /// </summary>
        public PooledBuffer(long capacity)
        {
            //_data = new byte[capacity];
            _data = _bufferManager!.TakeBuffer((int)capacity);
            _size = 0; _offset = 0;
        }
        /// <summary>
        /// Initialize a new expandable buffer with the given data
        /// </summary>
        public PooledBuffer(byte[] data)
        {
            _data = data; _size = data.Length; _offset = 0;
            _isPooled = false;
        }

        #region Memory buffer methods

        /// <summary>
        /// Get a span of bytes from the current buffer
        /// </summary>
        public Span<byte> AsSpan()
        {
            return new Span<byte>(_data, (int)_offset, (int)_size);
        }

        /// <summary>
        /// Get a string from the current buffer
        /// </summary>
        public override string ToString()
        {
            return ExtractString(0, _size);
        }

        // Clear the current buffer and its offset
        public void Clear()
        {
            _size = 0;
            _offset = 0;
        }

        /// <summary>
        /// Extract the string from buffer of the given offset and size
        /// </summary>
        public string ExtractString(long offset, long size)
        {
            Debug.Assert(offset + size <= Size, "Invalid offset & size!");
            if (offset + size > Size)
                throw new ArgumentException("Invalid offset & size!", nameof(offset));

            return Encoding.UTF8.GetString(_data!, (int)offset, (int)size);
        }

        /// <summary>
        /// Remove the buffer of the given offset and size
        /// </summary>
        public void Remove(int offset, int size)
        {
            Debug.Assert(offset + size <= Size, "Invalid offset & size!");
            if (offset + size > Size)
                throw new ArgumentException("Invalid offset & size!", nameof(offset));

            Array.Copy(_data!, offset + size, _data!, offset, _size - size - offset);
            _size -= size;
            if (_offset >= offset + size)
                _offset -= size;
            else if (_offset >= offset)
            {
                _offset -= _offset - offset;
                if (_offset > Size)
                    _offset = Size;
            }
        }

        /// <summary>
        /// Reserve the buffer of the given capacity
        /// </summary>
        public void Reserve(int capacity)
        {
            Debug.Assert(capacity >= 0, "Invalid reserve capacity!");
            if (capacity < 0)
                throw new ArgumentException("Invalid reserve capacity!", nameof(capacity));

            if (capacity > Capacity)
            {
                //byte[] data = new byte[Math.Max(capacity, 2 * Capacity)];
                byte[] data = _bufferManager!.TakeBuffer((int)Math.Max(capacity, 2 * Capacity));
                Array.Copy(_data!, 0, data, 0, _size);
                _bufferManager.ReturnBuffer(_data);
                _data = data;
            }
        }

        // Resize the current buffer
        public void Resize(int size)
        {
            Reserve(size);
            _size = size;
            if (_offset > _size)
                _offset = _size;
        }

        // Shift the current buffer offset
        public void Shift(int offset) { _offset += offset; }
        // Unshift the current buffer offset
        public void Unshift(int offset) { _offset -= offset; }

        #endregion

        #region Buffer I/O methods

        /// <summary>
        /// Append the single byte
        /// </summary>
        /// <param name="value">Byte value to append</param>
        /// <returns>Count of append bytes</returns>
        public long Append(byte value)
        {
            Reserve(_size + 1);
            _data![_size] = value;
            _size += 1;
            return 1;
        }

        /// <summary>
        /// Append the given buffer
        /// </summary>
        /// <param name="buffer">Buffer to append</param>
        /// <returns>Count of append bytes</returns>
        public long Append(byte[] buffer)
        {
            Reserve(_size + buffer.Length);
            Array.Copy(buffer, 0, _data!, _size, buffer.Length);
            _size += buffer.Length;
            return buffer.Length;
        }

        /// <summary>
        /// Append the given buffer fragment
        /// </summary>
        /// <param name="buffer">Buffer to append</param>
        /// <param name="offset">Buffer offset</param>
        /// <param name="size">Buffer size</param>
        /// <returns>Count of append bytes</returns>
        public long Append(byte[] buffer, int offset, int size)
        {
            Reserve(_size + size);
            Array.Copy(buffer, offset, _data!, _size, size);
            _size += size;
            return size;
        }

        /// <summary>
        /// Append the given span of bytes
        /// </summary>
        /// <param name="buffer">Buffer to append as a span of bytes</param>
        /// <returns>Count of append bytes</returns>
        public long Append(ReadOnlySpan<byte> buffer)
        {
            Reserve(_size + buffer.Length);
            buffer.CopyTo(new Span<byte>(_data, (int)_size, buffer.Length));
            _size += buffer.Length;
            return buffer.Length;
        }

        /// <summary>
        /// Append the given buffer
        /// </summary>
        /// <param name="buffer">Buffer to append</param>
        /// <returns>Count of append bytes</returns>
        public long Append(PooledBuffer buffer) => Append(buffer.AsSpan());

   
        public void Dispose()
        {
            if (_data != null && _isPooled)
            {
                _bufferManager!.ReturnBuffer(_data);
                _data = null;

            }

        }

        #endregion
    }
}
