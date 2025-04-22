using DeviceAPI;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
    bool AddDevice(Device device);
    Device GetDeviceById(string deviceId);
}