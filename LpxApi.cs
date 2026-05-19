using System.Runtime.InteropServices;
using LpxApi.launchpad;
using LpxApi.midi;
using LpxApi.windows;

namespace LpxApi;

public static class LpxApi
{
    public const string Version = "0.5.1";
    
    private static readonly Lock SendLock = new();
    private static bool _isSending = false;
    
    public static bool IsSending 
    { 
        get { lock (SendLock) { return _isSending; } }
        private set { lock (SendLock) { _isSending = value; } }
    }

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
    public static void SendSysEx(IntPtr phmo, byte? command, byte[] data, bool omitDevName = false)
    {
        if (IsSending) throw new SenderBusy();
        var totalLength = 1 + (omitDevName ? 0 : 5) + (command is null ? 0 : 1) + data.Length + 1;
        var written = 0;
        var sysexPtr = IntPtr.Zero;
        var pmhPtr = IntPtr.Zero;
        
        try
        {
            // Allocate and populate the SysEx data buffer
            sysexPtr = Marshal.AllocHGlobal(totalLength);
            
            Marshal.WriteByte(sysexPtr, written, (byte) StatusByte.SysExStart);
            written++;
            if (!omitDevName)
            {
                Marshal.Copy([(byte)0x00, (byte)0x20, (byte)0x29, (byte)0x02, (byte)0x0c], 0, sysexPtr + written, 5);
                written += 5;
            }
            if (command is not null)
            {
                Marshal.WriteByte(sysexPtr, written, command.Value);
                written++;
            }
            if (data.Length > 0)
            {
                Marshal.Copy(data, 0, sysexPtr + written, data.Length);
                written += data.Length;
            }
            Marshal.WriteByte(sysexPtr, written, (byte)StatusByte.SysExEnd);
            
            // Create and populate the MIDI header
            var pmh = new MidiHeader(sysexPtr, (uint)totalLength, (uint)totalLength);
            var size = Marshal.SizeOf<MidiHeader>();
            pmhPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(pmh, pmhPtr, false);
            
            // Prepare the header
            if (!Wap.midiOutPrepareHeader(phmo, pmhPtr, (uint)size))
            {
                throw new LpxApiException("midiOutPrepareHeader failed.");
            }
            
            SendSysExDebugPrints(113,"Header prepared.");
            
            // Double-check before setting IsSending
            if (IsSending) throw new SenderBusy();
            IsSending = true;
            
            SendSysExDebugPrints(119,"Not Busy.");
            
            // Send the message
            if (!Wap.midiOutLongMsg(phmo, pmhPtr, (uint)size))
            {
                IsSending = false;
                Wap.midiOutUnprepareHeader(phmo, pmhPtr, (uint)size);
                throw new LpxApiException("midiOutLongMsg failed.");
            }
            
            SendSysExDebugPrints(129, "Send.");
            
            // Asynchronously wait for completion and clean up
            var sysexPtrCopy = sysexPtr;
            var pmhPtrCopy = pmhPtr;
            var sizeCopy = size;
            
            Task.Run(() =>
            {
                
                SendSysExDebugPrints(139, "Task started.");
                
                try
                {
                    var hdr = Marshal.PtrToStructure<MidiHeader>(pmhPtrCopy);
                    var maxWait = 5000; // 5 seconds timeout
                    var waited = 0;
                    
                    while (!hdr.Flags.Done && waited < maxWait)
                    {
                        
                        SendSysExDebugPrints(150, $"Waiting. Already waited {waited}ms");
                        
                        Thread.Sleep(10);
                        waited += 10;
                        hdr = Marshal.PtrToStructure<MidiHeader>(pmhPtrCopy);
                    }
                    
                    if (waited >= maxWait)
                    {
                        Console.WriteLine("Warning: SysEx send timeout");
                    }
                }
                finally
                {
                    
                    SendSysExDebugPrints(165, "Wait over.");
                    
                    IsSending = false;
                    Wap.midiOutUnprepareHeader(phmo, pmhPtrCopy, (uint)sizeCopy);
                    
                    SendSysExDebugPrints(170, "Unprepared.");
                    
                    Marshal.FreeHGlobal(pmhPtrCopy);
                    Marshal.FreeHGlobal(sysexPtrCopy);
                }
            });
        }
        catch
        {
            SendSysExDebugPrints(179,"Entered Catch.");
            
            // Clean up on error (only if async task wasn't started)
            if (pmhPtr != IntPtr.Zero && !IsSending)
            {
                Marshal.FreeHGlobal(pmhPtr);
            }
            if (sysexPtr != IntPtr.Zero && !IsSending)
            {
                Marshal.FreeHGlobal(sysexPtr);
            }
            throw;
        }
    }

    private static void SendSysExDebugPrints(int line, string msg)
    {
        //Console.WriteLine($"--{{DBG: SendSysEx}}--< {line}: {msg}");
    }
}