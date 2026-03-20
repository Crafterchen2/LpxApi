namespace LpxApi.launchpad;

public class LpxApiException : Exception
{
    public LpxApiException() {}
    public LpxApiException(string? message) : base(message) {}
    public LpxApiException(string? message, Exception? innerException) : base(message, innerException) {}
}

public class NoInDevice() : LpxApiException("No device of type 'IN' configured.");
public class NoOutDevice() : LpxApiException("No device of type 'OUT' configured.");

public class UInt7OutOfRange() : LpxApiException("Maximum Value for bytes is 0x7f (127).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 0x7f) throw new UInt7OutOfRange();
    }
}

public class HalfByteOutOfRange() : LpxApiException("Maximum Value for bytes is 0x40 (64).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 0x40) throw new HalfByteOutOfRange();
    }
}

public class SignedByteOutOfRange(bool isSigned) : LpxApiException(isSigned
    ? "sbyte must be between -0x40 and 0x37 (inclusive)."
    : "byte must smaller than or equal to 0x7f")
{
    public static void Test(byte toTest)
    {
        if (toTest > 0x7f) throw new SignedByteOutOfRange(false);
    }
    
    public static void Test(sbyte toTest)
    {
        if (toTest is < -0x40 or > 0x3f) throw new SignedByteOutOfRange(true);
    }
}

public class UInt4OutOfRange() : LpxApiException("Maximum Value for nibbles is 0x0f (15).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 0x0f) throw new UInt4OutOfRange();
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

public class InvalidNoOverlapWidth() : LpxApiException("width must be between 0 and 8 (inclusive).")
{
    public static void Test(byte toTest)
    {
        if (toTest > 8) throw new InvalidNoOverlapWidth();
    }
}