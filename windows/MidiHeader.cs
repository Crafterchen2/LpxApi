using System.Runtime.InteropServices;

namespace LpxApi.windows;

[StructLayout(LayoutKind.Sequential)]
public struct MidiHeader
{
    public IntPtr Data { get; set; }
    public uint BufferLength { get; set; }
    public uint BytesRecorded { get; set; }
    public IntPtr User { get; set; }
    public MidiHeaderFlags Flags { get; set; }
    public IntPtr Next { get; set; }
    public uint Reserved { get; set; }
    public uint Offset { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    private readonly IntPtr[] _dwReserved;

    public IntPtr[] ReservedArr => _dwReserved;
    
    internal MidiHeader(IntPtr lpData, uint dwBufferLength, uint dwBytesRecorded)
    {
        _dwReserved = new IntPtr[8];
        Data = lpData;
        BufferLength = dwBufferLength;
        BytesRecorded = dwBytesRecorded;
        User = 0;
        Flags = 0;
        Next = 0;
        Reserved = 0;
        Offset = 0;
    }

    internal MidiHeader(IntPtr lpData, uint dwBufferLength, uint dwBytesRecorded, IntPtr dwUser,
        MidiHeaderFlags dwFlags, IntPtr lpNext, uint reserved, uint dwOffset, IntPtr[] dwReserved)
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