namespace LpxApi.windows;

public readonly struct DevOptFeatures(ulong value)
{
    public ulong Value { get; } = value;

    public bool VolumeControl => (Value & 1U) == 1U;
    public bool LeftRightVolCon => (Value & 2U) == 2U;
    public bool PatchCaching => (Value & 4U) == 4U;
    public bool Stream => (Value & 8U) == 8U;

    public static implicit operator DevOptFeatures(ulong l) => new(l);
    public static implicit operator ulong(DevOptFeatures d) => d.Value;
}