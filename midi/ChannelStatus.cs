namespace LpxApi.midi;

public readonly struct ChannelStatus(ChannelStatusType type, MidiChannel channel) : IByteTransmittable
{
    public StatusByte Status { get; private init; } = (StatusByte)((byte)type | (byte)channel);

    public MidiChannel Channel => (MidiChannel)((byte)Status & 0x0f);
    public ChannelStatusType Type => (ChannelStatusType)((byte)Status & 0xf0);

    public static implicit operator StatusByte(ChannelStatus s) => s.Status;
    public static implicit operator ChannelStatus(StatusByte s) => new() { Status = s };

    public byte[] ToBytes() => [(byte)Status];
}