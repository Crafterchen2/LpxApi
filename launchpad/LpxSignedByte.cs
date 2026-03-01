namespace LpxApi.launchpad;

public readonly struct LpxSignedByte : ISysExParameter
{
    public byte Value { get; }

    public LpxSignedByte(byte b)
    {
        SignedByteOutOfRange.Test(b);
        Value = b;
    }

    public LpxSignedByte(sbyte sb)
    {
        SignedByteOutOfRange.Test(sb);
        var neg = sb < 0;
        sb &= 0b0011_1111;
        if (neg) sb |= 0b0100_0000;
        Value = (byte)sb;
    }

    public static implicit operator byte(LpxSignedByte b) => b.Value;
    public static explicit operator LpxSignedByte(byte b) => new(b);
    public static explicit operator sbyte(LpxSignedByte b) => (sbyte)b.Value;
    public static implicit operator LpxSignedByte(sbyte b) => new(b);

    public byte[] ToBytes() => [Value];
}