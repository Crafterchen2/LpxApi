namespace LpxApi.launchpad;

public readonly struct ButtonIndex : IByteTransmittable
{
    public byte Index { get; }

    public ButtonIndex(byte index)
    {
        InvalidIndex.Test(index);
        Index = index;
    }

    public byte X => (byte)(Index % 10);
    public byte Y => (byte)(Index / 10);

    public static implicit operator byte(ButtonIndex i) => i.Index;
    public static implicit operator ButtonIndex(byte i) => new(i);
    public static implicit operator ButtonIndex(MenuButtonIndex i) => new((byte)i);
    public static explicit operator MenuButtonIndex(ButtonIndex i) => (MenuButtonIndex)i.Index;

    public byte[] ToBytes() => [Index];

    public bool IsMenuButton => Index != 99 && (X == 9 || Y == 9);
    public bool IsStatusLed => Index == 99;
}

public enum MenuButtonIndex : byte
{
    ArrowUp = 91,
    ArrowDown = 92,
    ArrowLeft = 93,
    ArrowRight = 94,
    SessionMixer = 95,
    Note = 96,
    Custom = 97,
    Capture = 98,
    StatusLed = 99,
    PageVolume = 89,
    PagePan = 79,
    PageSendA = 69,
    PageSendB = 59,
    PageStopClip = 49,
    PageMute = 39,
    PageSolo = 29,
    PageRecordArm = 19
}

public static class MenuButtonIndexExtension
{
    public static ButtonIndex ToStruct(this MenuButtonIndex i) => new((byte)i);
}