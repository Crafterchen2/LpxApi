namespace LpxApi.launchpad;

public readonly struct LpxNibble : ISysExParameter
{
    public byte Value { get; }

    public LpxNibble(byte value)
    {
        NibbleOutOfRange.Test(value);
        Value = value;
    }
    
    public static implicit operator byte(LpxNibble n) => n.Value;
    public static implicit operator LpxNibble(byte n) => new(n);

    public byte[] ToBytes() => [Value];
}