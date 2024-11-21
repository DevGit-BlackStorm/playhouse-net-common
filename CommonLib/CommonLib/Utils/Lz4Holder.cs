using System;

namespace PlayHouse.Utils;

public class Lz4Holder
{
    private byte[]? _compressBuffer;
    private byte[]? _depressBuffer;
    private Lz4? _lz4;

    // 싱글톤 인스턴스
    private static readonly Lazy<Lz4Holder> _instance = new Lazy<Lz4Holder>(() => new Lz4Holder());

    // 외부에서 접근 가능한 싱글톤 인스턴스
    public static Lz4Holder Instance => _instance.Value;

    // Private constructor to enforce singleton pattern
    private Lz4Holder() { }

    // 초기화 함수 (BufferSize를 지정)
    public void Init(int bufferSize)
    {
        if (_compressBuffer == null || _depressBuffer == null)
        {
            _compressBuffer = new byte[bufferSize];
            _depressBuffer = new byte[bufferSize];
            _lz4 = new Lz4(_compressBuffer, _depressBuffer);
        }
    }

    public ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> input)
    {
        return _lz4!.Compress(input);
    }

    public ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> compressed, int originalSize)
    {
        return _lz4!.Decompress(compressed, originalSize);
    }
}
