//using System;
//using System.IO;

//namespace CommonLib
//{
//    public class PreAllocByteArrayOutputStream : Stream
//    {
//        private byte[] _buffer;
//        private int _position;

//        public PreAllocByteArrayOutputStream(byte[] buffer)
//        {
//            _buffer = buffer;
//            _position = 0;
//        }

//        public override bool CanRead => true;
//        public override bool CanSeek => true;
//        public override bool CanWrite => true;
//        public override long Length => _buffer.Length;
//        public override long Position { get => _position; set => _position = (int)value; }

//        public override void Flush()
//        {
//            // Do nothing
//        }

//        public override int Read(byte[] buffer, int offset, int count)
//        {
//            if (_position >= _buffer.Length)
//            {
//                return 0;
//            }

//            int readCount = Math.Min(count, _buffer.Length - _position);
//            Array.Copy(_buffer, _position, buffer, offset, readCount);
//            _position += readCount;

//            return readCount;
//        }

//        public override long Seek(long offset, SeekOrigin origin)
//        {
//            switch (origin)
//            {
//                case SeekOrigin.Begin:
//                    _position = (int)offset;
//                    break;
//                case SeekOrigin.Current:
//                    _position += (int)offset;
//                    break;
//                case SeekOrigin.End:
//                    _position = _buffer.Length - (int)offset;
//                    break;
//                default:
//                    throw new ArgumentException("Invalid seek origin");
//            }
//            return _position;
//        }

//        public override void SetLength(long value)
//        {
//            if (value > _buffer.Length)
//            {
//                throw new IndexOutOfRangeException("Buffer size cannot be increased");
//            }
//            Array.Resize(ref _buffer, (int)value);
//            if (_position > value)
//            {
//                _position = (int)value;
//            }
//        }

//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            if (_position + count > _buffer.Length)
//            {
//                throw new IndexOutOfRangeException("Buffer is full");
//            }

//            Array.Copy(buffer, offset, _buffer, _position, count);
//            _position += count;
//        }

//        public override void WriteByte(byte value)
//        {
//            if (_position >= _buffer.Length)
//            {
//                throw new IndexOutOfRangeException("Buffer is full");
//            }
//            _buffer[_position++] = value;
//        }

//        public int WrittenDataLength()
//        {
//            return _position;
//        }

//        public void Reset()
//        {
//            _position = 0;
//        }

//        public int WriteShort(short value)
//        {
//            int startIndex = _position;
//            XBitConverter.ToByteArray(XBitConverter.ToNetworkOrder(value), _buffer, _position, 2);
//            _position += 2;
//            return startIndex;
//        }

//        public void ReplaceShort(int index, short value)
//        {
//            XBitConverter.ToByteArray(XBitConverter.ToNetworkOrder(value), _buffer, index, 2);
//        }

//        public short GetShort(int index)
//        {
//            if (index + 1 >= _buffer.Length)
//            {
//                throw new IndexOutOfRangeException("Index is out of bounds");
//            }

//            return XBitConverter.ToHostOrder(XBitConverter.ByteArrayToShort(_buffer, index, 2));
//        }

//        public byte[] Buffer()
//        {
//            return _buffer;
//        }
//    }
//}
