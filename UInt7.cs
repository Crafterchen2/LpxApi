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

    public static UInt7 operator +(UInt7 a, UInt7 b) => new((byte)((a.Value + b.Value) % 127));
    public static UInt7 operator -(UInt7 a, UInt7 b)
    {
        var d = a.Value - b.Value;
        if (d < 0) d += 127;
        return new UInt7((byte)(d % 127));
    }

    public static implicit operator byte(UInt7 b) => b.Value;
    public static implicit operator UInt7(byte b) => new(b);

    public byte[] ToBytes() => [Value];
}