namespace LpxApi.launchpad;

public readonly struct NoOverlapWidth : ISysExParameter
{
    public byte Value { get; }

    public NoOverlapWidth(byte value)
    {
        InvalidNoOverlapWidth.Test(value);
        Value = value;
    }

    public static implicit operator byte(NoOverlapWidth w) => w.Value;
    public static implicit operator NoOverlapWidth(byte w) => new(w);
    
    public byte[] ToBytes() => [Value];
}