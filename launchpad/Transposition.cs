namespace LpxApi.launchpad;

public readonly struct Transposition : IByteTransmittable
{
    public LpxSignedByte Value { get; }

    public Transposition(LpxSignedByte value)
    {
        var v = (sbyte)value;
        if (v is < -12 or > 12)
            throw new ArgumentOutOfRangeException(nameof(value), v, "value must be between -12 and 12 (inclusive).");
        Value = value;
    }

    public static implicit operator LpxSignedByte(Transposition t) => t.Value;
    public static implicit operator Transposition(LpxSignedByte b) => new(b);

    public byte[] ToBytes() => [Value];
}