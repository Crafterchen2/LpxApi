using LpxApi.launchpad;

namespace LpxApi;

public readonly struct UInt4 : IByteTransmittable
{
    public byte Value { get; }

    public UInt4(byte value)
    {
        UInt4OutOfRange.Test(value);
        Value = value;
    }
    
    public static implicit operator byte(UInt4 n) => n.Value;
    public static implicit operator UInt4(byte n) => new(n);

    public byte[] ToBytes() => [Value];
}