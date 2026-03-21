using LpxApi.midi;

namespace LpxApi.launchpad;

public abstract class Launchpad : IDisposable
{
    //Technical information about Launchpad object
    #region TechInfo
    
    public abstract bool Invalid { get; }

    #endregion
    
    //Technical functions for Launchpad object
    #region TechFunc

    public abstract void Dispose();

    #endregion
    
    //Convenience functions
    #region ConvFunc

    public abstract void ResetStartupAnimation();

    #endregion

    #region SysExSet
    
    public abstract void SelectLayout(Layout layout);
    public abstract void DawFaderSetup(LpxBool horizontal, Fader[] faders);
    public abstract void ApplyColors(Colorspec[] colors);
    public abstract void VelocityCurve(Curve curve, UInt7? fixedVelocity = null);
    public abstract void TextScrolling();
    public abstract void TextScrolling(LpxBool loop);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette, string asciiText);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, UInt7 r, UInt7 g, UInt7 b, string asciiText);
    public abstract void Brightness(UInt7 brightness);
    public abstract void LedSleep(LpxBool ledOn);
    public abstract void LedFeedback(LpxBool internalOn, LpxBool externalOn);
    public abstract void AftertouchConfig(AftertouchType type, AftertouchThreshold threshold);
    public abstract void FaderVelocity(LpxBool velocitySensitive);
    public abstract void ProgrammerMode(LpxBool enter);
    public abstract void DawNoteDrumRack(NoteDrumRackMode mode);
    public abstract void DawMode(LpxBool enter);
    public abstract void DawStateClear(LpxBool clearSession, LpxBool clearDrumRack, LpxBool clearControlChanges);
    public abstract void DrumRackPosition(LpxHalfByte offset);
    public abstract void DawSessionButton(Palette paletteActive, Palette paletteInactive);
    public abstract void NoteMode(LpxBool scaleMode);
    public abstract void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition);
    public abstract void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, MidiChannel channel, NoOverlapWidth width, Scale scale);
    public abstract void DawNoteModeActiveColor(Palette palette);
    public abstract void NoteModeColors(Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale);
    public abstract void Rotation(LpxBool enable);
    public abstract void StartupAnimation(UInt7 interval, StartupRgb[] rgbs);
    
    #endregion

    #region SysExGet
    
    // The comment represents an example message that can be sent by the launchpad.
    public abstract Layout GetSelectedLayout(); // [Header F0 00 20 29 02 0C] [Command 00] [layout 01] [Footer F7]
    public abstract (LpxBool isHorizontal, Fader[] fader) GetFaderSetup(); // [Header F0 00 20 29 02 0C] [Command 01] 00 [orientation 01] [fader #0 00 00 00 00] [fader #1 01 00 00 00] [fader #2 02 00 00 00] [fader #3 03 00 00 00] [fader #4 04 00 00 00] [fader #5 05 00 00 00] [fader #6 06 00 00 00] [fader #7 07 00 00 00] [Footer F7]
    public abstract (Curve curve, UInt7 fixedVelocity) GetVelocityCurve(); // [Header F0 00 20 29 02 0C] [Command 04] [curve 03] [fixedVelocity 40] [Footer F7]
    public abstract UInt7 GetBrightness(); // [Header F0 00 20 29 02 0C] [Command 08] [brightness 7F] [Footer F7]
    public abstract LpxBool IsLedOn(); // [Header F0 00 20 29 02 0C] [Command 09] [isLedOn 01] [Footer F7]
    public abstract (LpxBool internalOn, LpxBool externalOn) GetLedFeedback(); // [Header F0 00 20 29 02 0C] [Command 0A] [internal 01] [external 01] [Footer F7]
    public abstract (AftertouchType type, AftertouchThreshold threshold) GetAftertouchConfig(); // [Header F0 00 20 29 02 0C] [Command 0B] [type 00] [threshold 01] [Footer F7]
    public abstract LpxBool IsFaderVelocitySensitive(); // [Header F0 00 20 29 02 0C] [Command 0D] [velocitySensitive 01] [Footer F7]
    public abstract LpxBool IsProgrammerMode(); // [Header F0 00 20 29 02 0C] [Command 0E] [isProgrammerMode 01] [Footer F7]
    public abstract NoteDrumRackMode GetDawNoteDrumRackMode(); // [Header F0 00 20 29 02 0C] [Command 0F] [noteDrumRackMode 00] [Footer F7]
    public abstract LpxBool IsDawModeEnabled(); // [Header F0 00 20 29 02 0C] [Command 10] [isDawModeEnabled 01] [Footer F7]
    public abstract LpxHalfByte GetDrumRackPosition(); // [Header F0 00 20 29 02 0C] [Command 13] [position 24] [Footer F7]
    public abstract (Palette active, Palette inactive) GetSessionButtonColor(); // [Header F0 00 20 29 02 0C] [Command 14] [active 20] [inactive 30] [Footer F7]
    public abstract LpxBool IsNoteModeScale(); // [Header F0 00 20 29 02 0C] [Command 15] [isNoteMode 01] [Footer F7]
    public abstract (LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, MidiChannel channel, NoOverlapWidth width, Scale scale) GetNoteModeConfiguration(); // [Header F0 00 20 29 02 0C] [Command 16] [scaleMode 00] [octave 06] [transposition 74] [midiChannel 08] [width 05] 01 [definition 00 00 00 01 01 01 00 00 00 01 01] [Footer F7]
    public abstract Palette GetDawNoteModeColor(); //[Header F0 00 20 29 02 0C] [Command 17] [color 66] [Footer F7]
    public abstract (Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale) GetNoteModeColors(); // [Header F0 00 20 29 02 0C] [Command 22] [paletteActive 11] [paletteRoot 33] [paletteInScale 44] [paletteOoScale 55] [Footer F7]
    public abstract LpxBool GetRotation(); // [Header F0 00 20 29 02 0C] [Command 23] [isRotated 01] [Footer F7]
    public abstract StartupRgb[] GetStartupAnimation(); // [Header F0 00 20 29 02 0C] [Command 78] [rgb #0 1E] [rgb #1 04] ... [Footer F7]

    #endregion
}