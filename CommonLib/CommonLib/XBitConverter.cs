using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class XBitConverter
    {
        public static ushort ToNetworkOrder(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                // 하이 바이트 (상위 8비트)를 추출하고 왼쪽으로 8비트 시프트
                short highByte = (short)((value & 0xFF00) >> 8);
                // 로우 바이트 (하위 8비트)를 추출하고 오른쪽으로 8비트 시프트
                short lowByte = (short)((value & 0x00FF) << 8);

                // 변환된 하이 바이트와 로우 바이트를 합칩니다.
               ushort networkOrderValue = (ushort)(highByte | lowByte);
                return networkOrderValue;
            }
            else
            {
                return value;
            }

        }

        public static void ToByteArray(ushort value, byte[] buffer, int offset, int size)
        {
            if (size < 2)
            {
                throw new ArgumentException($"buffer size is too short : {size}");
            }

            buffer[offset] = (byte)((value & 0xFF00) >> 8); // 상위 바이트 (하이 바이트)
            buffer[offset + 1] = (byte)(value & 0x00FF); // 하위 바이트 (로우 바이트)

        }

        public static int ToNetworkOrder(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (value << 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | ((value >> 24) & 0xFF);
            }
            else
            {
                return value;
            }
        }
        //public static void IntToByteArray(int value, RingBuffer queue)
        //{

        //    queue.Enqueue((byte)((value >> 24) & 0xFF)); // 상위 바이트 (1번째 바이트)
        //    queue.Enqueue((byte)((value >> 16) & 0xFF)); // 2번째 바이트
        //    queue.Enqueue((byte)((value >> 8) & 0xFF));  // 3번째 바이트
        //    queue.Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)
        //}

        public static void IntToByteArray(int value, byte[] buffer, int offset)
        {
            if (buffer.Length < offset+4)
            {
                throw new ArgumentException($"buffer size is too short :length: {buffer.Length}, offset:{offset}");
            }
            buffer[offset] = (byte)((value >> 24) & 0xFF); // 상위 바이트 (1번째 바이트)
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF); // 2번째 바이트
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);  // 3번째 바이트
            buffer[offset + 3] = (byte)(value & 0xFF);         // 하위 바이트 (4번째 바이트)
        }

        public static void ShortToByteArray(ushort value, RingBuffer queue)
        {   
            queue.Enqueue((byte)((value >> 8) & 0xFF));  // 상위 바이트
            queue.Enqueue((byte)(value & 0xFF));         // 하위 바이트 (4번째 바이트)
        }

        //public static void ShortToByteArray(short value, RingBuffer queue,int index)
        //{
        //    //byte[] buffer = queue.Buffer();
        //    queue.SetByte(index, (byte)((value >> 8) & 0xFF));
        //    queue.SetByte(queue.NextIndex(index), (byte)(value & 0xFF));

        //    //buffer[index] = (byte)((value >> 8) & 0xFF);
        //    //buffer[queue.NextIndex(index)] = (byte)(value & 0xFF);
        //}

        public static void ShortToByteArray(ushort value, byte[] buffer, int offset)
        {
            if (buffer.Length < offset + 2)
            {
                throw new ArgumentException($"buffer size is too short :length: {buffer.Length}, offset:{offset}");
            }
            buffer[offset] = (byte)((value >> 24) & 0xFF); // 상위 바이트 (1번째 바이트)
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF); // 2번째 바이트
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);  // 3번째 바이트
            buffer[offset + 3] = (byte)(value & 0xFF);         // 하위 바이트 (4번째 바이트)
        }

        public static short ByteArrayToShort(byte[] buffer, int offset, int size)
        {
            if (size != 2)
            {
                throw new ArgumentException("Byte array must have a length of 2.");
            }

            short result = (short)((buffer[offset] << 8) | buffer[offset + 1]);
            return result;
        }

        //public static short ByteArrayToShort(byte byte1,byte byte2)
        //{            
        //    short result = (short)((byte1 << 8) | byte2);
        //    return result;
        //}

        public static int ByteArrayToInt(byte[] buffer, int offset, int size)
        {
            if (size != 4)
            {
                throw new ArgumentException("Byte array must have a length of 4.");
            }

            int result = (buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
            return result;
        }

        //public static int ByteArrayToInt(byte byte1, byte byte2, byte byte3, byte byte4)
        //{

        //    int result = (byte1 << 24) | (byte2 << 16) | (byte3 << 8) | byte4;
        //    return result;
        //}
        public static ushort ToHostOrder(ushort networkOrderValue)
        {
            if (BitConverter.IsLittleEndian)
            {
                short highByte = (short)((networkOrderValue & 0xFF00) >> 8);
                short lowByte = (short)((networkOrderValue & 0x00FF) << 8);

                ushort hostOrderValue = (ushort)(highByte | lowByte);
                return hostOrderValue;
            }
            else
            {
                return networkOrderValue;
            }
        }

        public static int ToHostOrder(int networkOrderValue)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (networkOrderValue << 24) | ((networkOrderValue & 0xFF00) << 8) | ((networkOrderValue >> 8) & 0xFF00) | ((networkOrderValue >> 24) & 0xFF);
            }
            else
            {
                return networkOrderValue;
            }
        }
    }
}
