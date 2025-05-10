namespace DeviceAPI;
using DeviceAPI;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAll();
    Device? GetById(string id);
    bool Add(Device device);
    bool Update(Device device);
    bool Delete(string id);
}