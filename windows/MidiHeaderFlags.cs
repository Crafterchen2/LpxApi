namespace LpxApi.windows;

public readonly struct MidiHeaderFlags(ulong value)
{
    public ulong Value { get; } = value;

    public bool Done => (Value & 0x1) == 0x1;
    public bool Prepared => (Value & 0x2) == 0x2;
    public bool InQueue => (Value & 0x4) == 0x4;
    public bool IsStream => (Value & 0x8) == 0x8;

    public static implicit operator MidiHeaderFlags(ulong l) => new(l);
    public static implicit operator ulong(MidiHeaderFlags f) => f.Value;
}