namespace LpxApi.launchpad;

public struct Fader(FaderIndex index, LpxBool bipolar, UInt7 controlChange, Palette palette) : IByteTransmittable
{
    public FaderIndex Index { get; } = index;
    public LpxBool Bipolar { get; } = bipolar;
    public UInt7 ControlChange { get; } = controlChange;
    public Palette Palette { get; } = palette;

    public byte[] ToBytes() => [Index, Bipolar, ControlChange, Palette];
}

public readonly struct FaderIndex : IByteTransmittable
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