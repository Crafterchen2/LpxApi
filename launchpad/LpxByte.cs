namespace LpxApi.launchpad;

public readonly struct LpxByte : ISysExParameter
{
    public byte Value { get; }

    public LpxByte(byte b)
    {
        ByteOutOfRange.Test(b);
        Value = b;
    }

    public static implicit operator byte(LpxByte b) => b.Value;
    public static implicit operator LpxByte(byte b) => new(b);

    public byte[] ToBytes() => [Value];
}