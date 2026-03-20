namespace LpxApi.launchpad;

public readonly struct StartupRgb(UInt7 r, UInt7 g, UInt7 b) : IByteTransmittable
{
    public UInt7 R { get; } = r;
    public UInt7 G { get; } = g;
    public UInt7 B { get; } = b;
    
    public byte[] ToBytes() => [R, G, B];
}