using System.Runtime.InteropServices;

namespace LpxApi.windows;

[StructLayout(LayoutKind.Sequential)]
public readonly struct MidiOutCaps : IDeviceInfo
{
    public ushort Mid { get; }
    public ushort Pid { get; }
    public MmVersion DriverVersion { get; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private readonly string szPname;

    public string ProductName => szPname; // Auto-Property is not possible because of the decorator.
    public OutDeviceType Technology { get; }
    public ushort Voices { get; }
    public ushort Notes { get; }
    public ChannelMask ChannelMask { get; }
    public DevOptFeatures Support { get; }

    public IoType IoType => IoType.Out;

    internal MidiOutCaps(ushort wMid, ushort wPid, MmVersion vDriverVersion, string szPname, OutDeviceType wTechnology,
        ushort wVoices, ushort wNotes, ChannelMask wChannelMask, DevOptFeatures dwSupport)
    {
        this.szPname = szPname;
        Mid = wMid;
        Pid = wPid;
        DriverVersion = vDriverVersion;
        Technology = wTechnology;
        Voices = wVoices;
        Notes = wNotes;
        ChannelMask = wChannelMask;
        Support = dwSupport;
    }
}