namespace LpxApi.windows;

public readonly struct MmResult(uint value)
{
    public uint Value { get; } = value;

    public bool NoError => Value == 0;

    public static implicit operator MmResult(uint i) => new(i);
    public static implicit operator uint(MmResult i) => i.Value;
    public static implicit operator bool(MmResult i) => i.NoError;

    public override string ToString()
    {
        string err;
        try
        {
            err = ((MmSysErr)Value).ToString();
        }
        catch
        {
            err = "Undefined";
        }
        return $"{Value} ({err})";
    }
}