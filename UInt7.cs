using LpxApi.launchpad;

namespace LpxApi;

public readonly struct UInt7 : IByteTransmittable
{
    public byte Value { get; }

    public UInt7(byte b)
    {
        UInt7OutOfRange.Test(b);
        Value = b;
    }

    public static implicit operator byte(UInt7 b) => b.Value;
    public static implicit operator UInt7(byte b) => new(b);

    public byte[] ToBytes() => [Value];
}