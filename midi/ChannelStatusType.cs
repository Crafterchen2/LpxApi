namespace LpxApi.midi;

public enum ChannelStatusType : byte
{
    NoteOff = 0x80,
    NoteOn = 0x90,
    PolyAftertouch = 0xa0,
    CmChange = 0xb0,
    ProgChange = 0xc0,
    ChannelAftertouch = 0xd0,
    PitchBendChange = 0xe0,
    Reserved = 0xf0
}