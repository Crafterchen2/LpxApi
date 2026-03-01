namespace LpxApi.windows;

public readonly struct ChannelMask(ushort value)
{
    public ushort Value { get; } = value;

    public bool this[int channel]
    {
        get
        {
            if (channel is < 0 or > 15) throw new ArgumentOutOfRangeException(nameof(channel), channel, "channel must be between 0 and 15 (inclusive).");
            var mask = 1 << channel;
            return (Value & mask) == mask;
        }
    }

    public static implicit operator ChannelMask(ushort s) => new(s);
    public static implicit operator ushort(ChannelMask c) => c.Value;

}