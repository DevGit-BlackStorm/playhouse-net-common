using CommonLib;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibTests
{
    public class RingBufferStreamTests
    {
        public RingBufferStreamTests()
        {
            PooledBuffer.Init();
        }

        [Fact]
        public void RingBufferStream_CanRead_ReturnsTrue()
        {
            var queue = new RingBuffer(10);
            var stream = new RingBufferStream(queue);

            stream.CanRead.Should().BeTrue();
        }

        [Fact]
        public void RingBufferStream_CanSeek_ReturnsFalse()
        {
            var queue = new RingBuffer(10);
            var stream = new RingBufferStream(queue);

            stream.CanSeek.Should().BeFalse();
        }

        [Fact]
        public void RingBufferStream_CanWrite_ReturnsTrue()
        {
            var queue = new RingBuffer(10);
            var stream = new RingBufferStream(queue);

            stream.CanWrite.Should().BeTrue();
        }

        //[Fact]
        //public void FixedByteQueueStream_Read_EmptyQueue_ThrowsInvalidOperationException()
        //{
        //    var queue = new FixedByteQueue(10);
        //    var stream = new FixedByteQueueStream(queue);

        //    stream.Invoking(s => s.Read(new byte[10], 0, 10))
        //        .Should().Throw<InvalidOperationException>()
        //        .WithMessage("Queue is empty");
        //}

        [Fact]
        public void RingBufferStream_Read_ReadsDataFromQueue()
        {
            var queue = new RingBuffer(10);
            queue.Enqueue(1);
            queue.Enqueue(2);
            var stream = new RingBufferStream(queue);
            var buffer = new byte[10];

            var bytesRead = stream.Read(buffer, 0, 2);

            bytesRead.Should().Be(2);
            buffer[0].Should().Be(1);
            buffer[1].Should().Be(2);
        }

        [Fact]
        public void RingBufferStream_Write_WritesDataToQueue()
        {
            var queue = new RingBuffer(10);
            var stream = new RingBufferStream(queue);
            var buffer = new byte[] { 1, 2 };

            stream.Write(buffer, 0, 2);

            queue.Count.Should().Be(2);
            queue.Peek().Should().Be(1);
            queue.Dequeue().Should().Be(1);
            queue.Peek().Should().Be(2);
            queue.Dequeue().Should().Be(2);
        }
    }
}
