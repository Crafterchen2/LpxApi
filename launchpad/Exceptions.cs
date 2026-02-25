namespace LpxApi.launchpad;

public class LpxApiException : Exception
{
    public LpxApiException() {}
    public LpxApiException(string? message) : base(message) {}
    public LpxApiException(string? message, Exception? innerException) : base(message, innerException) {}
}

public class NoInDevice() : LpxApiException("No device of type 'IN' configured.");
public class NoOutDevice() : LpxApiException("No device of type 'OUT' configured.");

public class ByteOutOfRange() : LpxApiException("Maximum Value for bytes is 0x7f (127).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 0x7f) throw new ByteOutOfRange();
    }
}

public class InvalidIndex() : LpxApiException("index must be a decimal number, where each digit must be between 1 and 9 (inclusive).")
{
    public static void Test(byte toTest)
    {
        if (toTest % 10 == 0 || toTest / 10 < 1 || toTest / 10 > 9) throw new InvalidIndex();
    }
}

public class InvalidFaderIndex() : LpxApiException("index must be between 0 and 7 (inclusive).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 7) throw new InvalidFaderIndex();
    }
}