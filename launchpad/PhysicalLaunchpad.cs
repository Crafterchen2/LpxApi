using System.Runtime.InteropServices;
using System.Text;
using LpxApi.midi;
using LpxApi.windows;

namespace LpxApi.launchpad;

public class PhysicalLaunchpad : Launchpad
{
    private IntPtr? _phmi, _phmo;
    private readonly Wap.MidiInProc _incomingProc;
    private readonly Wap.MidiOutProc _outgoingProc;
    private readonly List<IntPtr> _inputBuffers = [];
    private readonly List<IntPtr> _inputHeaders = [];

    public delegate void MidiReceivedDelegate(ChannelStatus status, UInt7 data1, UInt7 data2);
    public delegate void ButtonAnyDelegate(ChannelStatus status, ButtonIndex index, UInt7 velocity);
    public delegate void ButtonPressedDelegate(MidiChannel channel, ButtonIndex index, UInt7 velocity);
    public delegate void ButtonHoldDelegate(MidiChannel channel, ButtonIndex index, UInt7 pressure);
    public delegate void ButtonReleasedDelegate(MidiChannel channel, ButtonIndex index);
    
    public event MidiReceivedDelegate? MidiReceived;
    public event ButtonAnyDelegate? ButtonAny;
    public event ButtonPressedDelegate? ButtonPressed;
    public event ButtonHoldDelegate? ButtonHold;
    public event ButtonReleasedDelegate? ButtonReleased;

    public override bool Invalid => _phmi is null && _phmo is null;
    
    public PhysicalLaunchpad(uint? uDeviceIdIn = null, uint? uDeviceIdOut = null)
    {
        if (uDeviceIdIn is null && uDeviceIdOut is null)
        {
            throw new ArgumentException("At least one parameter must be non-null.");
        }
        
        _incomingProc = Incoming; 
        _outgoingProc = Outgoing;

        if (uDeviceIdIn is not null)
        {
            if (!Wap.midiInOpen(out var phmi, uDeviceIdIn.Value, _incomingProc, IntPtr.Zero, Wap.CallbackFunction)) throw new NoInDevice();
            _phmi = phmi;
            if (_phmi is null) throw new NoInDevice();
            
            PrepareInputBuffers(_phmi.Value);
            
            if (!Wap.midiInStart(_phmi.Value)) throw new NoInDevice();
        }

        if (uDeviceIdOut is not null)
        {
            if (!Wap.midiOutOpen(out var phmo, uDeviceIdOut.Value, _outgoingProc, IntPtr.Zero, Wap.CallbackFunction)) throw new NoOutDevice();
            _phmo = phmo;
            if (_phmo is null) throw new NoOutDevice();
        }
    }

    private void PrepareInputBuffers(IntPtr hMidiIn, int bufferCount = 4, int bufferSize = 512)
    {
        for (var i = 0; i < bufferCount; i++)
        {
            // Allocate buffer for SysEx data
            var buffer = Marshal.AllocHGlobal(bufferSize);
            _inputBuffers.Add(buffer);

            // Create MIDI header
            var header = new MidiHeader(buffer, (uint)bufferSize, 0);
            var headerPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MidiHeader>());
            Marshal.StructureToPtr(header, headerPtr, false);
            _inputHeaders.Add(headerPtr);

            // Prepare and add buffer
            if (Wap.midiInPrepareHeader(hMidiIn, headerPtr, (uint)Marshal.SizeOf<MidiHeader>()))
            {
                if (!Wap.midiInAddBuffer(hMidiIn, headerPtr, (uint)Marshal.SizeOf<MidiHeader>()))
                {
                    Console.WriteLine($"Warning: Failed to add input buffer {i}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Failed to prepare input header {i}");
            }
        }
    }

    private void Outgoing(IntPtr hMidiOut, uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
    {
        Console.WriteLine($"Outgoing: (IntPtr hMidiOut = {hMidiOut}, int wMsg = {wMsg}, IntPtr dwInstance = {dwInstance}, IntPtr dwParam1 = {dwParam1}, IntPtr dwParam2 = {dwParam2})");
    }

    private void Incoming(IntPtr hMidiIn, uint wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
    {
        switch (wMsg)
        {
            case Wap.MimOpen:
                Console.WriteLine("MIDI Input opened");
                break;

            case Wap.MimClose:
                Console.WriteLine("MIDI Input closed");
                break;

            case Wap.MimData:
                // Short MIDI message (3 bytes packed into dwParam1)
                var data = (uint)dwParam1;
                var status = (StatusByte)(data & 0xFF);
                var data1 = (byte)((data >> 8) & 0xFF);
                var data2 = (byte)((data >> 16) & 0xFF);
                Console.WriteLine($"Short Message: Status=0x{status}, Data1=0x{data1:X2}, Data2=0x{data2:X2}");
                Task.Run(() => MidiReceived?.Invoke(status, data1, data2));
                ChannelStatus chStatus = status;
                if (chStatus.Type 
                    is ChannelStatusType.NoteOn 
                    or ChannelStatusType.PolyAftertouch 
                    or ChannelStatusType.NoteOff)
                {
                    Task.Run(() => ButtonAny?.Invoke(status, data1, data2));
                    switch (chStatus.Type)
                    {
                        case ChannelStatusType.NoteOn or ChannelStatusType.NoteOff when data2 == 0:
                            Task.Run(() => ButtonReleased?.Invoke(chStatus.Channel, data1));
                            break;
                        case ChannelStatusType.NoteOn:
                            Task.Run(() => ButtonPressed?.Invoke(chStatus.Channel, data1, data2));
                            break;
                        case ChannelStatusType.PolyAftertouch:
                            Task.Run(() => ButtonHold?.Invoke(chStatus.Channel, data1, data2));
                            break;
                    }
                }
                break;

            case Wap.MimLongData:
                // SysEx message - dwParam1 points to MIDIHDR
                try
                {
                    var header = Marshal.PtrToStructure<MidiHeader>(dwParam1);
                    if (header.BytesRecorded > 0)
                    {
                        var sysexData = new byte[header.BytesRecorded];
                        Marshal.Copy(header.Data, sysexData, 0, (int)header.BytesRecorded);
                        
                        Console.Write("SysEx received: ");
                        foreach (var b in sysexData)
                        {
                            Console.Write($"{b:X2} ");
                        }
                        Console.WriteLine();
                    }

                    // Re-add the buffer for continued reception
                    Wap.midiInUnprepareHeader(hMidiIn, dwParam1, (uint)Marshal.SizeOf<MidiHeader>());
                    
                    // Reset the header
                    var newHeader = new MidiHeader(header.Data, header.BufferLength, 0);
                    Marshal.StructureToPtr(newHeader, dwParam1, false);
                    
                    Wap.midiInPrepareHeader(hMidiIn, dwParam1, (uint)Marshal.SizeOf<MidiHeader>());
                    Wap.midiInAddBuffer(hMidiIn, dwParam1, (uint)Marshal.SizeOf<MidiHeader>());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing SysEx: {ex.Message}");
                }
                break;

            case Wap.MimError:
                Console.WriteLine($"MIDI Input Error: {dwParam1}");
                break;

            case Wap.MimLongError:
                Console.WriteLine($"MIDI Input Long Error: {dwParam1}");
                break;

            case Wap.MimMoreData:
                Console.WriteLine("MIDI Input buffer overflow - data lost!");
                break;

            default:
                Console.WriteLine($"Unknown MIDI message: wMsg=0x{wMsg:X}");
                break;
        }
    }

    public override void Dispose()
    {
        if (_phmi is not null)
        {
            Wap.midiInStop(_phmi.Value);
            //Wap.midiInReset(_phmi.Value); Todo(bug): This hangs, why?
            // Do I need to wait here for midiInReset to be done?
            for (var i = 0; i < _inputHeaders.Count; i++)
            {
                Wap.midiInUnprepareHeader(_phmi.Value, _inputHeaders[i], (uint)Marshal.SizeOf<MidiHeader>());
                Marshal.FreeHGlobal(_inputHeaders[i]);
                Marshal.FreeHGlobal(_inputBuffers[i]);
            }
            _inputHeaders.Clear();
            _inputBuffers.Clear();
            Wap.midiInClose(_phmi.Value);
            _phmi = null;
        }

        if (_phmo is not null)
        {
            Wap.midiOutClose(_phmo.Value);
            _phmo = null;
        }
    }

    private void TrySendSysEx(IntPtr phmo, byte command, params byte[][] data)
    {
        byte[] concatData;
        switch (data.Length)
        {
            case 0: concatData = []; break;
            case 1: concatData = data[0]; break;
            default:
                concatData = new byte[data.Sum(part => part.Length)];
                var alreadyWritten = 0;
                foreach (var part in data)
                {
                    Buffer.BlockCopy(part, 0, concatData, alreadyWritten, part.Length);
                    alreadyWritten += part.Length;
                }
                break;
        }

        try
        {
            LpxApi.SendSysEx(phmo, command, concatData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"SendSysEx failed: {e.Message}");
            Dispose();
        }
    }

    private byte[] FlattenArr<T>(T[] arr) where T : IByteTransmittable
    {
        byte[] rv;
        switch (arr.Length)
        {
            case 0: rv = []; break;
            case 1: rv = arr[0].ToBytes(); break;
            default:
                rv = new byte[arr.Sum(part => part.ToBytes().Length)];
                var alreadyWritten = 0;
                foreach (var part in arr)
                {
                    var bytes = part.ToBytes();
                    Buffer.BlockCopy(bytes, 0, rv, alreadyWritten, bytes.Length);
                    alreadyWritten += bytes.Length;
                }
                break;
        }

        return rv;
    }

    private static byte[] PrepForScroll(string text)
    {
        var l = new List<byte>();
        foreach (var b in Encoding.ASCII.GetBytes(text))
        {
            if (b is >= 0x20 and <= 0x7e) l.Add(b);
        }

        return l.ToArray();
    }

    public override void ResetStartupAnimation()
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x78, [
            0x1e, 0x04, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x25, 0x00, 0x00, 0x4e,
            0x04, 0x00, 0x7f, 0x0c, 0x00, 0x7f, 0x25, 0x25, 0x00, 0x4e, 0x4e,
            0x04, 0x25, 0x7f, 0x0c, 0x0c, 0x7f, 0x25, 0x04, 0x4e, 0x4e, 0x4e,
            0x00, 0x25, 0x7f, 0x00, 0x0c, 0x7f, 0x00, 0x04, 0x4e, 0x00, 0x00,
            0x25, 0x25, 0x00, 0x00, 0x0c, 0x00, 0x00, 0x04, 0xf7
        ]);
    }

    public override void SelectLayout(Layout layout)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x00, [(byte)layout]);
    }

    public override void DawFaderSetup(LpxBool horizontal, Fader[] faders)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x01, horizontal.ToBytes(), FlattenArr(faders));
    }

    public override void ApplyColors(Colorspec[] colors)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x03, FlattenArr(colors));
    }

    public override void VelocityCurve(Curve curve, UInt7? fixedVelocity = null)
    {
        if (_phmo is null) return;
        var data = new byte[fixedVelocity is null ? 1 : 2];
        data[0] = (byte)curve;
        if (data.Length == 2) data[1] = fixedVelocity!.Value;
        TrySendSysEx(_phmo.Value, 0x04, data);
    }

    public override void TextScrolling()
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07);
    }

    public override void TextScrolling(LpxBool loop)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop]);
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop, speed]);
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop, speed, 0x00], palette.ToBytes());
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop, speed, 0x01, r, g, b]);
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette, string asciiText)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop, speed, 0x00], palette.ToBytes(), PrepForScroll(asciiText));
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b, string asciiText)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x07, [loop, speed, 0x01, r, g, b], PrepForScroll(asciiText));
    }

    public override void Brightness(UInt7 brightness)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x08, brightness.ToBytes());
    }

    public override void LedSleep(LpxBool ledOn)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x09, ledOn.ToBytes());
    }

    public override void LedFeedback(LpxBool internalOn, LpxBool externalOn)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x0a, [internalOn, externalOn]);
    }

    public override void AftertouchConfig(AftertouchType type, AftertouchThreshold threshold)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x0b, [(byte)type, (byte)threshold]);
    }

    public override void FaderVelocity(LpxBool velocitySensitive)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x0d, [velocitySensitive]);
    }

    public override void ProgrammerMode(LpxBool enter)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x0e, [enter]);
    }

    public override void DawNoteDrumRack(NoteDrumRackMode mode)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x0f, [(byte)mode]);
    }

    public override void DawMode(LpxBool enter)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x10, [enter]);
    }

    public override void DawStateClear(LpxBool clearSession, LpxBool clearDrumRack, LpxBool clearControlChanges)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x12, [clearSession, clearDrumRack, clearControlChanges]);
    }

    public override void DrumRackPosition(LpxHalfByte offset)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x13, [offset]);
    }

    public override void DawSessionButton(Palette paletteActive, Palette paletteInactive)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x14, paletteActive.ToBytes(), paletteInactive.ToBytes());
    }

    public override void NoteMode(LpxBool scaleMode)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x15, [scaleMode]);
    }

    public override void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x16, [scaleMode, octave, transposition.Value]);
    }

    public override void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, MidiChannel channel, NoOverlapWidth width, Scale scale)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x16, [scaleMode, octave, transposition.Value, (byte)channel, width], scale.ToBytes());
    }

    public override void DawNoteModeActiveColor(Palette palette)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x17, palette.ToBytes());
    }

    public override void NoteModeColors(Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x22, paletteActive.ToBytes(), paletteRoot.ToBytes(), paletteInScale.ToBytes(), paletteOoScale.ToBytes());
    }

    public override void Rotation(LpxBool enable)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x23, [enable]);
    }

    public override void StartupAnimation(UInt7 interval, StartupRgb[] rgbs)
    {
        if (_phmo is null) return;
        TrySendSysEx(_phmo.Value, 0x78, [interval], FlattenArr(rgbs));
    }

    public override LpxVersion DeviceInquiry()
    {
        try
        {
            LpxApi.SendSysEx(_phmo!.Value, null, [0x7e, 0x7f, 0x06, 0x01], true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return new LpxVersion(0, 0, 0, 0);
    }

    public override Layout GetSelectedLayout()
    {
        throw new NotImplementedException();
    }

    public override (LpxBool isHorizontal, Fader[] fader) GetFaderSetup()
    {
        throw new NotImplementedException();
    }

    public override (Curve curve, UInt7 fixedVelocity) GetVelocityCurve()
    {
        throw new NotImplementedException();
    }

    public override UInt7 GetBrightness()
    {
        throw new NotImplementedException();
    }

    public override LpxBool IsLedOn()
    {
        throw new NotImplementedException();
    }

    public override (LpxBool internalOn, LpxBool externalOn) GetLedFeedback()
    {
        throw new NotImplementedException();
    }

    public override (AftertouchType type, AftertouchThreshold threshold) GetAftertouchConfig()
    {
        throw new NotImplementedException();
    }

    public override LpxBool IsFaderVelocitySensitive()
    {
        throw new NotImplementedException();
    }

    public override LpxBool IsProgrammerMode()
    {
        throw new NotImplementedException();
    }

    public override NoteDrumRackMode GetDawNoteDrumRackMode()
    {
        throw new NotImplementedException();
    }

    public override LpxBool IsDawModeEnabled()
    {
        throw new NotImplementedException();
    }

    public override LpxHalfByte GetDrumRackPosition()
    {
        throw new NotImplementedException();
    }

    public override (Palette active, Palette inactive) GetSessionButtonColor()
    {
        throw new NotImplementedException();
    }

    public override LpxBool IsNoteModeScale()
    {
        throw new NotImplementedException();
    }

    public override (LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, MidiChannel channel, NoOverlapWidth width, Scale scale) GetNoteModeConfiguration()
    {
        throw new NotImplementedException();
    }

    public override Palette GetDawNoteModeColor()
    {
        throw new NotImplementedException();
    }

    public override (Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale) GetNoteModeColors()
    {
        throw new NotImplementedException();
    }

    public override LpxBool GetRotation()
    {
        throw new NotImplementedException();
    }

    public override StartupRgb[] GetStartupAnimation()
    {
        throw new NotImplementedException();
    }
}