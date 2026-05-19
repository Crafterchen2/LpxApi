namespace LpxApi.launchpad;

public readonly record struct LpxHalfByte : IByteTransmittable
{
    public byte Value { get; }

    public LpxHalfByte(byte b)
    {   
        HalfByteOutOfRange.Test(b);
        Value = b;
    }

    public static implicit operator byte(LpxHalfByte b) => b.Value;
    public static implicit operator LpxHalfByte(byte b) => new(b);

    public byte[] ToBytes() => [Value];
}