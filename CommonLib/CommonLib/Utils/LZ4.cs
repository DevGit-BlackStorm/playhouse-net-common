using K4os.Compression.LZ4;
using System;

namespace PlayHouse.Utils
{
    public static class LZ4
    {
        private static byte[]? _compressBuffer;
        private static byte[]? _depressBuffer;
        public static void Init(int bufferSize)
        {
            _compressBuffer = new byte[bufferSize];
            _depressBuffer = new byte[bufferSize];
        }

        private static void EnsureInitialized()
        {
            if (_compressBuffer == null || _depressBuffer == null)
                throw new InvalidOperationException("Buffers are not initialized. Call Init() first.");
        }

        public static ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> input)
        {
            EnsureInitialized();
            // 최대 압축 크기 계산
            int maxCompressedSize = LZ4Codec.MaximumOutputSize(input.Length);

            // LZ4 압축 수행
            int compressedSize = LZ4Codec.Encode(
                input,                         // 입력 데이터
                _compressBuffer.AsSpan(0, maxCompressedSize) // 출력 버퍼
            );

            // 압축된 데이터를 ReadOnlySpan으로 반환
            return _compressBuffer.AsSpan(0, compressedSize);
        }


        public static ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> compressed, int originalSize)
        {
            EnsureInitialized();

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
