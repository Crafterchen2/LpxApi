namespace LpxApi.launchpad;

public readonly struct LpxVersion : IByteTransmittable
{
    public byte Digit1 { get; }
    public byte Digit2 { get; }
    public byte Digit3 { get; }
    public byte Digit4 { get; }
    
    public int Version { get; }

    public LpxVersion(byte digit1, byte digit2, byte digit3, byte digit4)
    {
        InvalidDecimalDigit.Test(digit1);
        InvalidDecimalDigit.Test(digit2);
        InvalidDecimalDigit.Test(digit3);
        InvalidDecimalDigit.Test(digit4);
        Digit1 = digit1;
        Digit2 = digit2;
        Digit3 = digit3;
        Digit4 = digit4;
        Version = Digit4;
        Version *= 10;
        Version += Digit3;
        Version *= 10;
        Version += Digit2;
        Version *= 10;
        Version += Digit1;
    }

    public byte[] ToBytes() => [Digit1, Digit2, Digit3, Digit4];
}