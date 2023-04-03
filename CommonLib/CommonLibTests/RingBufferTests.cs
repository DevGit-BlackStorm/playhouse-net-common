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
        public void Enqueue_WhenCalledWithSingleByte_ShouldAddByteToBuffer()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);

            // Act
            buffer.Enqueue((byte)42);

            // Assert
            buffer.Count.Should().Be(1);
            buffer.Peek().Should().Be(42);
        }

        [Fact]
        public void Enqueue_WhenCalledWithByteArray_ShouldAddAllBytesToBuffer()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            var data = new byte[] { 1, 2, 3 };

            // Act
            buffer.Enqueue(data);

            // Assert
            buffer.Count.Should().Be(3);
            buffer.Peek().Should().Be(1);
        }

        [Fact]
        public void Dequeue_WhenCalledWithNonEmptyBuffer_ShouldRemoveAndReturnFirstByte()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.Enqueue((byte)1);
            buffer.Enqueue((byte)2);
            buffer.Enqueue((byte)3);

            // Act
            var result = buffer.Dequeue();

            // Assert
            result.Should().Be(1);
            buffer.Count.Should().Be(2);
            buffer.Peek().Should().Be(2);
        }

        [Fact]
        public void Dequeue_WhenCalledWithEmptyBuffer_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);

            // Act & Assert
            buffer.Invoking(b => b.Dequeue()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Clear_WhenCalled_ShouldRemoveAllBytesFromBuffer()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.Enqueue((byte)1);
            buffer.Enqueue((byte)2);
            buffer.Enqueue((byte)3);

            // Act
            buffer.Clear();

            // Assert
            buffer.Count.Should().Be(0);
            buffer.Invoking(b => b.Dequeue()).Should().Throw<InvalidOperationException>();
            //buffer.Peek().Should().Be(default);
        }

        [Fact]
        public void Read_WhenCalledWithNonEmptyBuffer_ShouldReadBytesIntoProvidedBuffer()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.Enqueue(new byte[] { 1, 2, 3 });

            var data = new byte[2];

            // Act
            var bytesRead = buffer.Read(data, offset: 0, count: 2);

            // Assert
            bytesRead.Should().Be(2);
            data.Should().BeEquivalentTo(new byte[] { 1, 2 });
        }

        [Fact]
        public void Write_WhenCalledWithByteArray_ShouldAddAllBytesToBuffer()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            var data = new byte[] { 1, 2, 3 };

            // Act
            buffer.Write(data, offset: 0, count: 3);

            // Assert
            buffer.Count.Should().Be(3);
            buffer.Peek().Should().Be(1);
        }

        [Fact]
        public void PeekInt16_WhenCalledWithValidIndex_ShouldReturnCorrectValue()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.WriteInt16(value: 0x1234);
            buffer.WriteInt16(value: 0x5678);

            // Act
            var result = buffer.PeekInt16(0);

            // Assert
            result.Should().Be(0x1234);
        }

        [Fact]
        public void PeekInt16_WhenCalledWithInvalidIndex_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);

            // Act & Assert
            buffer.Invoking(b => b.PeekInt16( 0)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void PeekInt32_WhenCalledWithValidIndex_ShouldReturnCorrectValue()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.WriteInt32(value: 0x12345678);

            // Act
            var result = buffer.PeekInt32(0);

            // Assert
            result.Should().Be(0x12345678);
        }

        [Fact]
        public void PeekInt32_WhenCalledWithInvalidIndex_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);

            // Act & Assert
            buffer.Invoking(b => b.PeekInt32( 0)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ReadInt16_WhenCalledWithNonEmptyBuffer_ShouldReadAndRemoveFirstValue()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.WriteInt16(value: 0x1234);
            var index = buffer.WriteInt16(value: 0x5678);

            // Assert
            buffer.PeekInt16(index).Should().Be(0x5678);
        }

        [Fact]
        public void ReadInt32_WhenCalledWithNonEmptyBuffer_ShouldReadAndRemoveFirstValue()
        {
            // Arrange
            var buffer = new RingBuffer(capacity: 10);
            buffer.WriteInt32(value: 0x12345678);
            var index = buffer.WriteInt32(value: 0x5678DEF0);

            // Assert
            buffer.PeekInt32(index).Should().Be(0x5678DEF0);
        }


    }




}
