//using CommonLib;
//using FluentAssertions;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CommonLibTests
//{
//    public class PreAllocByteArrayOutputStreamTests
//    {
//        private readonly byte[] _buffer = new byte[10];
//        private readonly PreAllocByteArrayOutputStream _byteArrayOutputStream;
        

//        public PreAllocByteArrayOutputStreamTests()
//        {
//            _byteArrayOutputStream = new PreAllocByteArrayOutputStream(_buffer);
//        }

//        private void ResetBeforeEachTest()
//        {
//            _byteArrayOutputStream.Reset();
//        }

//        [Fact]
//        public void WriteByteTest()
//        {
//            ResetBeforeEachTest();
//            _byteArrayOutputStream.WriteByte(65); // 'A' ASCII value
//            _byteArrayOutputStream.WrittenDataLength().Should().Be(1);
//            _buffer.AsSpan(0, 1).ToArray().Should().Equal(new byte[] { 65 });
//        }

//        [Fact]
//        public void WriteShortTest()
//        {
//            ResetBeforeEachTest();
//            short value = 1234;
//            var startIndex = _byteArrayOutputStream.WriteShort(value);
//            _byteArrayOutputStream.WrittenDataLength().Should().Be(2);
//            startIndex.Should().Be(0);
//            _byteArrayOutputStream.GetShort(startIndex).Should().Be(value);
//        }

//        [Fact]
//        public void ReplaceShortTest()
//        {
//            ResetBeforeEachTest();
//            short value = 1234;
//            var startIndex = _byteArrayOutputStream.WriteShort(value);
//            _byteArrayOutputStream.GetShort(0).Should().Be(value);
//            _byteArrayOutputStream.ReplaceShort(startIndex, 300);
//            _byteArrayOutputStream.GetShort(startIndex).Should().Be(300);
//        }
//    }
//}
