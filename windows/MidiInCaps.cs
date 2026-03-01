using System.Runtime.InteropServices;

namespace LpxApi.windows;

[StructLayout(LayoutKind.Sequential)]
public readonly struct MidiInCaps : IDeviceInfo
{
    public ushort Mid { get; }
    public ushort Pid { get; }
    public MmVersion DriverVersion { get; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private readonly string szPname;

    public string ProductName => szPname; // Auto-Property is not possible because of the decorator.

    public ulong Support { get; } // Reserved, must be zero
    
    public IoType IoType => IoType.Out;

    internal MidiInCaps(ushort wMid, ushort wPid, MmVersion vDriverVersion, string szPname, uint dwSupport)
    {
        this.szPname = szPname;
        Mid = wMid;
        Pid = wPid;
        DriverVersion = vDriverVersion;
        Support = dwSupport;
    }
}