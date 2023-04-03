using CommonLib;
using FluentAssertions;

namespace CommonLibTest
{
    public class XBitConverterTests
    {
        [Theory]
        [InlineData((short)1, new byte[] { 0, 1 })]
        [InlineData((short)256, new byte[] { 1, 0 })]
        public void ShortToByteArray_ShouldConvertShortToByteArray(short value, byte[] expectedResult)
        {
            // Arrange
            byte[] buffer = new byte[2];

            // Act
            XBitConverter.ToByteArray(value, buffer, 0, 2);

            // Assert
            buffer.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData(new byte[] { 0, 1 }, (short)1)]
        [InlineData(new byte[] { 1, 0 }, (short)256)]
        public void ByteArrayToShort_ShouldConvertByteArrayToShort(byte[] value, short expectedResult)
        {
            // Act
            short result = XBitConverter.ByteArrayToShort(value, 0, 2);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData((ushort)1, new byte[] { 0, 1 })]
        [InlineData((ushort)256, new byte[] { 1, 0 })]
        public void UShortToByteArray_ShouldConvertUShortToByteArray(ushort value, byte[] expectedResult)
        {
            // Arrange
            byte[] buffer = new byte[2];

            // Act
            XBitConverter.ToByteArray((short)value, buffer, 0, 2);

            // Assert
            buffer.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData(new byte[] { 0, 1 }, (ushort)1)]
        [InlineData(new byte[] { 1, 0 }, (ushort)256)]
        public void ByteArrayToUShort_ShouldConvertByteArrayToUShort(byte[] value, ushort expectedResult)
        {
            // Act
            ushort result = (ushort)XBitConverter.ByteArrayToShort(value, 0, 2);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, new byte[] { 0, 0, 0, 1 })]
        [InlineData(256, new byte[] { 0, 0, 1, 0 })]
        public void IntToByteArray_ShouldConvertIntToByteArray(int value, byte[] expectedResult)
        {
            // Arrange
            byte[] buffer = new byte[4];

            // Act
            XBitConverter.IntToByteArray(value, buffer, 0);

            // Assert
            buffer.Should().Equal(expectedResult);
        }
        [Theory]
        [InlineData(new byte[] { 0, 0, 0, 1 }, 1)]
        [InlineData(new byte[] { 0, 0, 1, 0 }, 256)]
        public void ByteArrayToInt_ShouldConvertByteArrayToInt(byte[] value, int expectedResult)
        {
            // Act
            int result = XBitConverter.ByteArrayToInt(value, 0, 4);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData((uint)1, new byte[] { 0, 0, 0, 1 })]
        [InlineData((uint)256, new byte[] { 0, 0, 1, 0 })]
        public void UIntToByteArray_ShouldConvertUIntToByteArray(uint value, byte[] expectedResult)
        {
            // Arrange
            byte[] buffer = new byte[4];

            // Act
            XBitConverter.IntToByteArray((int)value, buffer, 0);

            // Assert
            buffer.Should().Equal(expectedResult);
        }

        [Theory]
        [InlineData(new byte[] { 0, 0, 0, 1 }, (uint)1)]
        [InlineData(new byte[] { 0, 0, 1, 0 }, (uint)256)]
        public void ByteArrayToUInt_ShouldConvertByteArrayToUInt(byte[] value, uint expectedResult)
        {
            // Act
            uint result = (uint)XBitConverter.ByteArrayToInt(value, 0, 4);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
