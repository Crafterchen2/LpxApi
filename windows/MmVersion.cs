using System.Runtime.InteropServices;

namespace LpxApi.windows;

[StructLayout(LayoutKind.Explicit)]
public readonly struct MmVersion(uint value)
{
    [FieldOffset(0)] // [Padding*16 | Major*8 | Minor*8]
    private readonly uint _value = value;
    
    [FieldOffset(2)] // [Padding*16 | Major*8 | Minor*8]
    private readonly byte _major;
    
    [FieldOffset(3)] // [Padding*16 | Major*8 | Minor*8]
    private readonly byte _minor;

    public uint Value => _value;

    public byte Major => _major;
    public byte Minor => _minor;

    public static implicit operator MmVersion(uint i) => new(i);
    public static implicit operator uint(MmVersion i) => i.Value;
}