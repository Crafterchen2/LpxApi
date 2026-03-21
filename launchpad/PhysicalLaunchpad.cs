using LpxApi.midi;
using LpxApi.windows;

namespace LpxApi.launchpad;

public class PhysicalLaunchpad : Launchpad
{
    private IntPtr? _phmi, _phmo;
    public override bool Invalid => _phmi is null && _phmo is null;
    
    public PhysicalLaunchpad(uint? uDeviceIdIn = null, uint? uDeviceIdOut = null)
    {
        if (uDeviceIdIn is null && uDeviceIdOut is null)
        {
            throw new ArgumentException("At least one parameter must be non-null.");
        }

        if (uDeviceIdIn is not null)
        {
            if (!Wap.midiInOpen(out var phmi, uDeviceIdIn.Value, Incoming, IntPtr.Zero, Wap.CallbackFunction)) throw new NoInDevice();
            _phmi = phmi;
            if (_phmi is null || !Wap.midiInStart(_phmi.Value)) throw new NoInDevice();
        }

        if (uDeviceIdOut is not null)
        {
            if (!Wap.midiOutOpen(out var phmo, uDeviceIdOut.Value, Outgoing, IntPtr.Zero, Wap.CallbackFunction)) throw new NoOutDevice();
            _phmo = phmo;
            if (_phmo is null) throw new NoOutDevice();
        }
    }

    private void Outgoing(IntPtr hMidiOut, int wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
    {
        Console.WriteLine($"Outgoing: (IntPtr hMidiOut = {hMidiOut}, int wMsg = {wMsg}, IntPtr dwInstance = {dwInstance}, IntPtr dwParam1 = {dwParam1}, IntPtr dwParam2 = {dwParam2})");
    }

    private void Incoming(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
    {
        Console.WriteLine($"Incoming: (IntPtr hMidiOut = {hMidiIn}, int wMsg = {wMsg}, IntPtr dwInstance = {dwInstance}, IntPtr dwParam1 = {dwParam1}, IntPtr dwParam2 = {dwParam2})");        
    }

    public override void Dispose()
    {
        if (_phmi is not null)
        {
            Wap.midiInStop(_phmi.Value);
            Wap.midiInClose(_phmi.Value);
            _phmi = null;
        }

        if (_phmo is not null)
        {
            Wap.midiOutClose(_phmo.Value);
            _phmo = null;
        }
    }

    public override void ResetStartupAnimation()
    {
        throw new NotImplementedException();
    }

    public override void SelectLayout(Layout layout)
    {
        throw new NotImplementedException();
    }

    public override void DawFaderSetup(LpxBool horizontal, Fader[] faders)
    {
        throw new NotImplementedException();
    }

    public override void ApplyColors(Colorspec[] colors)
    {
        throw new NotImplementedException();
    }

    public override void VelocityCurve(Curve curve, UInt7? fixedVelocity = null)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling()
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette, string asciiText)
    {
        throw new NotImplementedException();
    }

    public override void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b, string asciiText)
    {
        throw new NotImplementedException();
    }

    public override void Brightness(UInt7 brightness)
    {
        throw new NotImplementedException();
    }

    public override void LedSleep(LpxBool ledOn)
    {
        throw new NotImplementedException();
    }

    public override void LedFeedback(LpxBool internalOn, LpxBool externalOn)
    {
        throw new NotImplementedException();
    }

    public override void AftertouchConfig(AftertouchType type, AftertouchThreshold threshold)
    {
        throw new NotImplementedException();
    }

    public override void FaderVelocity(LpxBool velocitySensitive)
    {
        throw new NotImplementedException();
    }

    public override void ProgrammerMode(LpxBool enter)
    {
        throw new NotImplementedException();
    }

    public override void DawNoteDrumRack(NoteDrumRackMode mode)
    {
        throw new NotImplementedException();
    }

    public override void DawMode(LpxBool enter)
    {
        throw new NotImplementedException();
    }

    public override void DawStateClear(LpxBool clearSession, LpxBool clearDrumRack, LpxBool clearControlChanges)
    {
        throw new NotImplementedException();
    }

    public override void DrumRackPosition(LpxHalfByte offset)
    {
        throw new NotImplementedException();
    }

    public override void DawSessionButton(Palette paletteActive, Palette paletteInactive)
    {
        throw new NotImplementedException();
    }

    public override void NoteMode(LpxBool scaleMode)
    {
        throw new NotImplementedException();
    }

    public override void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition)
    {
        throw new NotImplementedException();
    }

    public override void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, MidiChannel channel,
        NoOverlapWidth width, Scale scale)
    {
        throw new NotImplementedException();
    }

    public override void DawNoteModeActiveColor(Palette palette)
    {
        throw new NotImplementedException();
    }

    public override void NoteModeColors(Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale)
    {
        throw new NotImplementedException();
    }

    public override void Rotation(LpxBool enable)
    {
        throw new NotImplementedException();
    }

    public override void StartupAnimation(UInt7 interval, StartupRgb[] rgbs)
    {
        throw new NotImplementedException();
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