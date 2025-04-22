using DeviceAPI;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
}