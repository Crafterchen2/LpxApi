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
    internal static extern uint midiOutGetNumDevs();

    [DllImport("winmm.dll")]
    internal static extern uint midiInGetNumDevs();

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutGetDevCaps(uint uDeviceId, out MidiOutCaps pmoc, uint cbmoc);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInGetDevCaps(uint uDeviceId, out MidiInCaps caps, uint cbMidiInCaps);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutOpen(out IntPtr phmo, uint uDeviceId, MidiOutProc dwCallback, IntPtr dwInstance, uint fdwOpen);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInOpen(out IntPtr phmi, uint uDeviceId, MidiInProc dwCallback, IntPtr dwInstance, uint fdwOpen);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutPrepareHeader(IntPtr handle, IntPtr header, uint size);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutLongMsg(IntPtr hmo, IntPtr pmh, uint cbmh);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutUnprepareHeader(IntPtr hmo, IntPtr header, uint size);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInStart(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInStop(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInClose(IntPtr phmi);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiOutClose(IntPtr phmo);
    
    [DllImport("winmm.dll")]
    internal static extern MmResult midiInPrepareHeader(IntPtr hmi, IntPtr lpMidiInHdr, uint cbMidiInHdr);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInUnprepareHeader(IntPtr hmi, IntPtr lpMidiInHdr, uint cbMidiInHdr);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInAddBuffer(IntPtr hmi, IntPtr lpMidiInHdr, uint cbMidiInHdr);

    [DllImport("winmm.dll")]
    internal static extern MmResult midiInReset(IntPtr hmi);
    
    internal delegate void MidiInProc(IntPtr hMidiIn, uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);
    internal delegate void MidiOutProc(IntPtr hMidiOut, uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);
}