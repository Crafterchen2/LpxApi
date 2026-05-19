namespace LpxApi.launchpad;

public readonly record struct Scale : IByteTransmittable
{
    public bool CustomScale { get; }
    public UInt4? Preset { get; }
    public LpxBool[]? Definition { get; }

    public Scale(UInt4 preset)
    {
        CustomScale = false;
        Preset = preset;
        Definition = null;
    }

    public Scale(LpxBool[] definition)
    {
        if (definition.Length != 11)
            throw new ArgumentException($"{nameof(definition)} must be 11.", nameof(definition));
        CustomScale = true;
        Preset = null;
        Definition = definition;
    }

    public byte[] ToBytes()
    {
        if (!CustomScale) return [LpxBool.False, Preset!.Value];
        var rv = new List<byte> { LpxBool.True };
        rv.AddRange(Definition!.Select(d => d.Value));
        return rv.ToArray();
    }
}