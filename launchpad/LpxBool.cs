namespace LpxApi.launchpad;

public readonly record struct LpxBool : IByteTransmittable
{
    public const byte True = 0x01, False = 0x00;
    
    public byte Value { get; }

    public LpxBool(bool value)
    {
        Value = value ? True : False;
    }

    public LpxBool(byte value)
    {
        Value = value != False ? True : False;
    }

    public static implicit operator bool(LpxBool b) => b.Value != False;
    public static implicit operator LpxBool(bool b) => new(b);
    public static implicit operator byte(LpxBool b) => b.Value;
    public static implicit operator LpxBool(byte b) => new(b);

    public byte[] ToBytes() => [Value];
}