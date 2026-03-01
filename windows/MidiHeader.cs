using System.Runtime.InteropServices;

namespace LpxApi.windows;

public readonly struct MidiHeader
{
    public IntPtr Data { get; }
    public ulong BufferLength { get; }
    public ulong BytesRecorded { get; }
    public IntPtr User { get; }
    public MidiHeaderFlags Flags { get; }
    public IntPtr Next { get; }
    public ulong Reserved { get; }
    public ulong Offset { get; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    private readonly ulong[] _dwReserved;

    public ulong[] ReservedArr => _dwReserved;
    
    internal MidiHeader(IntPtr lpData, ulong dwBufferLength, ulong dwBytesRecorded)
    {
        _dwReserved = new ulong[8];
        Data = lpData;
        BufferLength = dwBufferLength;
        BytesRecorded = dwBytesRecorded;
        User = 0;
        Flags = 0;
        Next = 0;
        Reserved = 0;
        Offset = 0;
    }

    internal MidiHeader(IntPtr lpData, ulong dwBufferLength, ulong dwBytesRecorded, IntPtr dwUser,
        MidiHeaderFlags dwFlags, IntPtr lpNext, ulong reserved, ulong dwOffset, ulong[] dwReserved)
    {
        _dwReserved = dwReserved;
        Data = lpData;
        BufferLength = dwBufferLength;
        BytesRecorded = dwBytesRecorded;
        User = dwUser;
        Flags = dwFlags;
        Next = lpNext;
        Reserved = reserved;
        Offset = dwOffset;
    }
}