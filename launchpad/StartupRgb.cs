namespace LpxApi.launchpad;

public readonly struct StartupRgb(LpxByte r, LpxByte g, LpxByte b) : IByteTransmittable
{
    public LpxByte R { get; } = r;
    public LpxByte G { get; } = g;
    public LpxByte B { get; } = b;
    
    public byte[] ToBytes() => [R, G, B];
}