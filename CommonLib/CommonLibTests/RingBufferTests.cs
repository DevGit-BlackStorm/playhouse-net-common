using CommonLib;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibTests
{
    public class RingBufferTests
    {
        public RingBufferTests() {
            PooledBuffer.Init();
        }

        [Fact]
        public void Enqueue_ShouldAddAnItemToTheBuffer()
        {
            RingBuffer ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Count.Should().Be(1);
            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Enqueue_ShouldResizeTheBufferWhenItsCapacityIsReached()
        {
            RingBuffer ringBuffer = new RingBuffer(4, 8);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Enqueue(4);
            ringBuffer.Count.Should().Be(4);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(4);
            ringBuffer.Enqueue(5);
            ringBuffer.Count.Should().Be(5);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Enqueue_ShouldThrowAnExceptionWhenTheBufferHasReachedMaximumCapacity()
        {
            RingBuffer ringBuffer = new RingBuffer(4, 8);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Enqueue(4);
            ringBuffer.Enqueue(5);
            ringBuffer.Enqueue(6);
            ringBuffer.Enqueue(7);
            ringBuffer.Enqueue(8);
            ringBuffer.Count.Should().Be(8);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
            Action action = () =>
            {
                for (int i = 0; i < ringBuffer.Capacity ; i++){
                    ringBuffer.Enqueue(9);
                }
            };
            
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EnqueueData_ShouldAddAnArrayOfDataToTheBuffer()
        {
            RingBuffer ringBuffer = new RingBuffer(4, 8);
            byte[] data = new byte[] { 1, 2, 3, 4 };
            ringBuffer.Enqueue(data);
            ringBuffer.Count.Should().Be(4);
            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void ResizeBuffer_ShouldResizeTheBufferAndPreserveItsContents()
        {
            RingBuffer ringBuffer = new RingBuffer(4, 8);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Enqueue(4);
            ringBuffer.Count.Should().Be(4);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(4);

            ringBuffer.Enqueue(5);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
            ringBuffer.Count.Should().Be(5);

            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Dequeue_ShouldRemoveAnItemFromTheBuffer()
        {
            var ringBuffer = new RingBuffer(4, 8);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Count.Should().Be(3);
            ringBuffer.Dequeue().Should().Be(1);
            ringBuffer.Count.Should().Be(2);
            ringBuffer.Peek().Should().Be(2);
        }

        [Fact]
        public void Dequeue_ShouldThrowAnExceptionWhenTheBufferIsEmpty()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.Dequeue();
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Peek_ShouldReturnTheFirstItemInTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Peek_ShouldThrowAnExceptionWhenTheBufferIsEmpty()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.Peek();
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Clear_ShouldClearTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Count.Should().Be(3);
            ringBuffer.Clear();
            ringBuffer.Count.Should().Be(0);
        }

        [Fact]
        public void Clear_ShouldRemoveTheSpecifiedNumberOfItemsFromTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Count.Should().Be(3);
            ringBuffer.Clear(2);
            ringBuffer.Count.Should().Be(1);
            ringBuffer.Peek().Should().Be(3);
        }
        [Fact]
        public void Clear_ShouldThrowAnExceptionWhenTryingToClearMoreItemsThanTheCurrentSizeOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Count.Should().Be(3);
            Action action = () => ringBuffer.Clear(4);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Read_ShouldReadItemsFromTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.Enqueue(1);
            ringBuffer.Enqueue(2);
            ringBuffer.Enqueue(3);
            ringBuffer.Count.Should().Be(3);
            var buffer = new byte[2];
            ringBuffer.Read(buffer, 0, 2).Should().Be(2);
            buffer.Should().Equal(new byte[] { 1, 2 });
            ringBuffer.Count.Should().Be(1);
            ringBuffer.Peek().Should().Be(3);
        }

        [Fact]
        public void Read_ShouldReturnZeroWhenTheBufferIsEmpty()
        {
            var ringBuffer = new RingBuffer(4);
            var buffer = new byte[2];
            ringBuffer.Read(buffer, 0, 2).Should().Be(0);
        }

        [Fact]
        public void Write_ShouldWriteItemsToTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            var data = new byte[] { 1, 2, 3, 4 };
            ringBuffer.Write(data, 0, 4);
            ringBuffer.Count.Should().Be(4);
            ringBuffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Write_ShouldResizeTheBufferWhenItsCapacityIsReached()
        {
            var ringBuffer = new RingBuffer(4, 16);
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            ringBuffer.Write(data, 0, 8);
            ringBuffer.Count.Should().Be(8);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
            ringBuffer.Write(data, 0, 1);
            ringBuffer.Count.Should().Be(9);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(16);
            ringBuffer.Peek().Should().Be(1);
        }
        [Fact]
        public void PeekInt16_ShouldThrowAnExceptionWhenTryingToPeekOutsideOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.PeekInt16(2);
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void PeekInt32_ShouldReturnTheCorrectIntValueWhenPeekingAtTheSpecifiedOffset()
        {
            var ringBuffer = new RingBuffer(8);
            ringBuffer.WriteInt32(0x12345678);
            ringBuffer.WriteInt32(0x5678DEF0);
            ringBuffer.PeekInt32(4).Should().Be(0x5678DEF0);
        }

        [Fact]
        public void PeekInt32_ShouldThrowAnExceptionWhenTryingToPeekOutsideOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.PeekInt32(4);
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void ReadInt16_ShouldReturnTheCorrectShortValueAndUpdateTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.WriteInt16(0x1234);
            ringBuffer.WriteInt16(0x5678);
            ringBuffer.ReadInt16().Should().Be(0x1234);
            ringBuffer.Count.Should().Be(2);
            ringBuffer.Peek().Should().Be(0x56);
        }
        [Fact]
        public void ReadInt16_ShouldThrowAnExceptionWhenTryingToReadOutsideOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.ReadInt16();
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void ReadInt32_ShouldReturnTheCorrectIntValueAndUpdateTheBuffer()
        {
            var ringBuffer = new RingBuffer(8);
            ringBuffer.WriteInt32(0x12345678);
            ringBuffer.WriteInt32(0x5678DEF0);
            ringBuffer.ReadInt32().Should().Be(0x12345678);
            ringBuffer.Count.Should().Be(4);
            ringBuffer.Peek().Should().Be(0x56);
        }

        [Fact]
        public void ReadInt32_ShouldThrowAnExceptionWhenTryingToReadOutsideOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.ReadInt32();
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void SetInt16_ShouldSetTheShortValueAtTheSpecifiedIndex()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.WriteInt16(0x1234);
            ringBuffer.WriteInt16(0x5678);
            ringBuffer.SetInt16(2, 0x4321);
            ringBuffer.PeekInt16(2).Should().Be(0x4321);
        }

        [Fact]
        public void SetInt16_ShouldThrowAnExceptionWhenTryingToSetAValueOutsideOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            Action action = () => ringBuffer.SetInt16(2, 0x1234);
            action.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void WriteInt16_ShouldWriteTheShortValueToTheBufferAndReturnTheStartIndex()
        {
            var ringBuffer = new RingBuffer(8);
            ringBuffer.WriteInt16(0x1234);
            ringBuffer.WriteInt16(0x5678);
            ringBuffer.WriteInt16(0x4321).Should().Be(4);
            ringBuffer.Count.Should().Be(6);
            ringBuffer.Peek().Should().Be(0x12);
        }

        [Fact]
        public void WriteInt16_ShouldResizeTheBufferWhenItsCapacityIsReached()
        {
            var ringBuffer = new RingBuffer(4, 8);
            ringBuffer.WriteInt16(0x1234);
            ringBuffer.WriteInt16(0x5678);
            ringBuffer.WriteInt16(0x4321).Should().Be(4);
            ringBuffer.Count.Should().Be(6);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
            ringBuffer.WriteInt16(0x4321).Should().Be(6);
            ringBuffer.Count.Should().Be(8);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(8);
        }

        [Fact]
        public void WriteInt32_ShouldWriteTheIntValueToTheBufferAndReturnTheStartIndex()
        {
            var ringBuffer = new RingBuffer(16);
            ringBuffer.WriteInt32(0x12345678);
            ringBuffer.WriteInt32(0x5678DEF0);
            ringBuffer.WriteInt32(0x11223344).Should().Be(8);
            ringBuffer.Count.Should().Be(12);
            ringBuffer.Peek().Should().Be(0x12);
        }

        [Fact]
        public void WriteInt32_ShouldResizeTheBufferWhenItsCapacityIsReached()
        {
            var ringBuffer = new RingBuffer(8, 16);
            ringBuffer.WriteInt32(0x12345678);
            ringBuffer.WriteInt32(0x5678DEF0);
            ringBuffer.WriteInt32(0x11223344).Should().Be(8);
            ringBuffer.Count.Should().Be(12);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(16);
            ringBuffer.WriteInt32(0x55667788).Should().Be(12);
            ringBuffer.Count.Should().Be(16);
            ringBuffer.Capacity.Should().BeGreaterThanOrEqualTo(16);
        }

        [Fact]
        public void ToArray_ShouldReturnAnArrayWithTheContentsOfTheBuffer()
        {
            var ringBuffer = new RingBuffer(4);
            ringBuffer.WriteInt16(0x1234);
            ringBuffer.WriteInt16(0x5678);
            ringBuffer.Buffer().AsSpan(0,ringBuffer.Count).ToArray().Should().Equal(new byte[] { 0x12, 0x34, 0x56, 0x78 });
        }

    }




}
