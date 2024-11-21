using K4os.Compression.LZ4;
using System;

namespace PlayHouse.Utils
{
    public  class Lz4
    {
        private const int MaxBufferSize = 1024 * 1024 * 2; // 2mb
        private byte[] _compressBuffer = new byte[1024 * 10];
        private byte[] _depressBuffer = new byte[1024 * 10];
        
        public  ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> input)
        {
            // 최대 압축 크기 계산
            int maxCompressedSize = LZ4Codec.MaximumOutputSize(input.Length);

            if (_compressBuffer.Length < maxCompressedSize)
            {
                int newSize = Math.Min(maxCompressedSize * 2, MaxBufferSize);
                _compressBuffer = new byte[newSize];
            }

            // LZ4 압축 수행
            int compressedSize = LZ4Codec.Encode(
                input,                         // 입력 데이터
                _compressBuffer.AsSpan(0, maxCompressedSize) // 출력 버퍼
            );

            // 압축된 데이터를 ReadOnlySpan으로 반환
            return _compressBuffer.AsSpan(0, compressedSize);
        }


        public ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> compressed, int originalSize)
        {
            if (_depressBuffer.Length < originalSize)
            {
                int newSize = Math.Min(originalSize * 2, MaxBufferSize);
                _depressBuffer = new byte[newSize];
            }
            // LZ4 압축 해제 수행
            int decodedSize = LZ4Codec.Decode(
                compressed,                     // 입력 데이터
                _depressBuffer              // 출력 버퍼
            );

            // 압축 해제된 크기가 원본 크기와 일치하지 않으면 오류
            if (decodedSize != originalSize)
                throw new InvalidOperationException("Decompressed size does not match original size.");

            return _depressBuffer.AsSpan(0,originalSize);
        }
    }
}
