using System.Runtime.InteropServices;

namespace LpxApi.windows;

internal static class Wap //Windows Api wraPper = WAP
{
    internal const int MimOpen      = 0x3C1;
    internal const int MimClose     = 0x3C2;
    internal const int MimData      = 0x3C3; // Short Message
    internal const int MimLongData  = 0x3C4; // SysEx
    internal const int MimError     = 0x3C5;
    internal const int MimLongError = 0x3C6;
    internal const int MimMoreData  = 0x3CC;
    internal const int CallbackFunction = 0x00030000;
    
    [DllImport("winmm.dll")]
    internal static extern int midiOutGetNumDevs();

    [DllImport("winmm.dll")]
    internal static extern int midiInGetNumDevs();

    [DllImport("winmm.dll")]
    internal static extern int midiOutGetDevCaps(int uDeviceId, out MidiOutCaps caps, int cbMidiOutCaps);

    [DllImport("winmm.dll")]
    internal static extern int midiInGetDevCaps(int uDeviceId, out MidiInCaps caps, int cbMidiInCaps);

    [DllImport("winmm.dll")]
    internal static extern int midiOutOpen(out IntPtr phmo, int uDeviceId, IntPtr dwCallback, IntPtr dwInstance, int fdwOpen);

    [DllImport("winmm.dll")]
    internal static extern int midiInOpen(out IntPtr phmi, int uDeviceId, MidiInProc dwCallback, IntPtr dwInstance, int fdwOpen);

    [DllImport("winmm.dll")]
    internal static extern int midiOutPrepareHeader(IntPtr handle, IntPtr header, int size);

    [DllImport("winmm.dll")]
    internal static extern int midiOutLongMsg(IntPtr hmo, IntPtr pmh, int cbmh);

    [DllImport("winmm.dll")]
    internal static extern int midiOutUnprepareHeader(IntPtr handle, IntPtr header, int size);

    [DllImport("winmm.dll")]
    internal static extern int midiInStart(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern int midiInStop(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern int midiInClose(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern int midiOutClose(IntPtr phmo);

    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiOutCaps
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;

        public ushort wTechnology;
        public ushort wVoices;
        public ushort wNotes;
        public ushort wChannelMask;
        public uint dwSupport;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiInCaps
    {
        public ushort wMid;
        public ushort wPid;
        public uint vDriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;

        public uint dwSupport;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Midihdr
    {
        public IntPtr lpData;
        public int dwBufferLength;
        public int dwBytesRecorded;
        public IntPtr dwUser;
        public int dwFlags;
        public IntPtr lpNext;
        public IntPtr reserved;
        public int dwOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public IntPtr[] dwReserved;
    }
    
    internal delegate void MidiInProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);
}