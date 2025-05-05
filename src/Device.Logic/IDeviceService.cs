using DeviceAPI;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
    bool AddDevice(Device device);
    Device? GetDeviceById(string deviceId);
    bool RemoveDevice(string deviceId);
    bool UpdateDevice(Device device);
}