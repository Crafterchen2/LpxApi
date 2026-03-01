namespace LpxApi.windows;

public interface IDeviceInfo
{
    public IoType IoType { get; }
    public ushort Mid { get; }
    public ushort Pid { get; }
    public MmVersion DriverVersion { get; }
    public string ProductName { get; }
}