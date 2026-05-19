namespace LpxApi.launchpad;

public readonly record struct Fader(FaderIndex Index, LpxBool Bipolar, UInt7 ControlChange, Palette Palette) : IByteTransmittable
{
    public byte[] ToBytes() => [Index, Bipolar, ControlChange, Palette];
}

public readonly record struct FaderIndex : IByteTransmittable
{
    public byte Index { get; }

    public FaderIndex(byte index)
    {
        InvalidFaderIndex.Test(index);
        Index = index;
    }

    public static implicit operator byte(FaderIndex i) => i.Index;
    public static implicit operator FaderIndex(byte i) => new(i);

    public byte[] ToBytes() => [Index];
}