namespace LpxApi.launchpad;

public abstract class Launchpad : IDisposable
{
    #region Callback
    
    public delegate void ShortMsgCallbackDelegate(int status, int data1, int data2);
    public ShortMsgCallbackDelegate? ShortMsgCallback { get; set; }

    #endregion
    
    //Technical information about Launchpad object
    #region TechInfo
    
    public abstract LpxBool Invalid { get; }

    #endregion
    
    //Technical functions for Launchpad object
    #region TechFunc

    public void TransferCallback(Launchpad transferTo)
    {
        transferTo.ShortMsgCallback = ShortMsgCallback;
        ShortMsgCallback = null;
    }

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
    public abstract void VelocityCurve(Curve curve, LpxByte? fixedVelocity = null);
    public abstract void TextScrolling();
    public abstract void TextScrolling(LpxBool loop);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, LpxByte r, LpxByte g, LpxByte b);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, Palette palette, string asciiText);
    public abstract void TextScrolling(LpxBool loop, LpxSignedByte speed, LpxByte r, LpxByte g, LpxByte b, string asciiText);
    public abstract void Brightness(LpxByte brightness);
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
    public abstract void NoteModeConfig(LpxBool scaleMode, LpxSignedByte octave, Transposition transposition, byte width, Scale? scale = null);
    public abstract void DawNoteModeActiveColor(Palette palette);
    public abstract void NoteModeColors(Palette paletteActive, Palette paletteRoot, Palette paletteInScale, Palette paletteOoScale);
    public abstract void Rotation(LpxBool enable);
    public abstract void StartupAnimation(LpxByte interval, StartupRgb[] rgbs);
    
    #endregion
}