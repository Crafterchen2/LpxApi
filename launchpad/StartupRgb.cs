namespace LpxApi.launchpad;

public readonly record struct StartupRgb(UInt7 R, UInt7 G, UInt7 B) : IByteTransmittable
{
    public byte[] ToBytes() => [R, G, B];
}