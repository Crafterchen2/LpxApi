namespace LpxApi.launchpad;

public readonly struct ButtonIndex : ISysExParameter
{
    public byte Index { get; }

    public ButtonIndex(byte index)
    {
        InvalidIndex.Test(index);
        Index = index;
    }

    public byte X => (byte)(Index % 10);
    public byte Y => (byte)(Index / 10);

    public static implicit operator byte(ButtonIndex i) => i.Index;
    public static implicit operator ButtonIndex(byte i) => new(i);

    public byte[] ToBytes() => [Index];
}