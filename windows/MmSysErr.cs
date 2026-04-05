namespace LpxApi.windows;

public enum MmSysErr
{
    /// <summary>
    /// No Error.
    /// </summary>
    NoError = 0,
    
    /// <summary>
    /// Unspecified Error.
    /// </summary>
    Error = 1,
    
    /// <summary>
    /// Device ID out of range.
    /// </summary>
    BadDeviceId = 2,
    
    /// <summary>
    /// Driver failed to enable.
    /// </summary>
    NotEnabled = 3,
    
    /// <summary>
    /// Device already allocated.
    /// </summary>
    Allocated = 4,
    
    /// <summary>
    /// Device handle is invalid.
    /// </summary>
    InvalidHandle = 5,
    
    /// <summary>
    /// No device driver present.
    /// </summary>
    NoDriver = 6,
    
    /// <summary>
    /// Memory allocation error.
    /// </summary>
    NoMemory = 7,
    
    /// <summary>
    /// Function isn't supported.
    /// </summary>
    NotSupported = 8,
    
    /// <summary>
    /// Error value out of range.
    /// </summary>
    BadErrorNum = 9,
    
    /// <summary>
    /// Invalid flag passed.
    /// </summary>
    InvalidFlag = 10,
    
    /// <summary>
    /// Invalid parameter passed.
    /// </summary>
    InvalidParam = 11
}