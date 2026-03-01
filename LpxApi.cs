using System.Runtime.InteropServices;
using LpxApi.launchpad;
using LpxApi.windows;

namespace LpxApi;

public static class LpxApi
{
    public const string Version = "0.5.0";

    public static long GetDeviceCount(IoType? type = null)
    {
        var rv = 0L;
        if (type is null or IoType.In)
        {
            rv += Wap.midiInGetNumDevs();
        }

        if (type is null or IoType.Out)
        {
            rv += Wap.midiOutGetNumDevs();
        }

        return rv;
    }

    public static MidiInCaps?[] GetAllInCaps()
    {
        var n = Wap.midiInGetNumDevs();
        var rv = new MidiInCaps?[n];
        for (var i = 0U; i < n; i++)
        {
            if (Wap.midiInGetDevCaps(i, out var caps, (uint)Marshal.SizeOf<MidiInCaps>()))
            {
                rv[i] = caps;
            }
            else
            {
                rv[i] = null;
            }
        }
        
        return rv;
    }

    public static MidiOutCaps?[] GetAllOutCaps()
    {
        var n = Wap.midiOutGetNumDevs();
        var rv = new MidiOutCaps?[n];
        for (var i = 0U; i < n; i++)
        {
            if (Wap.midiOutGetDevCaps(i, out var caps, (uint)Marshal.SizeOf<MidiOutCaps>()))
            {
                rv[i] = caps;
            }
            else
            {
                rv[i] = null;
            }
        }

        return rv;
    }

    public static void SendSysEx(IntPtr phmo, byte? command = null) => SendSysEx(phmo, command, []);
    public static void SendSysEx(IntPtr phmo, byte command, byte[] data) => SendSysEx(phmo, (byte?)command, data);
    private static void SendSysEx(IntPtr phmo, byte? command, byte[] data)
    {
        var sysex = command is null 
            ? new byte[] { 0xf0, 0x00, 0x20, 0x29, 0x02, 0x0c } 
            : new byte[] { 0xf0, 0x00, 0x20, 0x29, 0x02, 0x0c, command.Value };
        var sysexPtr = Marshal.AllocHGlobal(Marshal.SizeOf<byte>() * (sysex.Length + data.Length + 1));
        Marshal.Copy(sysex, 0, sysexPtr, sysex.Length);
        Marshal.Copy(data, 0, sysexPtr + sysex.Length, data.Length);
        Marshal.WriteByte(sysexPtr, sysex.Length + data.Length, 0xf7);

        var pmh = new MidiHeader(sysexPtr, (ulong)(sysex.Length + data.Length + 1), (ulong)(sysex.Length + data.Length + 1));
        var size = Marshal.SizeOf<MidiHeader>();
        var pmhPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(pmh, pmhPtr, false);
        if (!Wap.midiOutPrepareHeader(phmo, pmhPtr, (uint)size)) throw new LpxApiException("midiOutPrepareHeader failed.");
        if (!Wap.midiOutLongMsg(phmo, pmhPtr, size)) throw new LpxApiException("midiOutLongMsg failed.");
        if (!Wap.midiOutUnprepareHeader(phmo, pmhPtr, (uint)size)) throw new LpxApiException("midiOutUnprepareHeader failed");
    }
}