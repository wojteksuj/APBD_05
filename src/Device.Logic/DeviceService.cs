using System.Data;
using Microsoft.Data.SqlClient;
using DeviceAPI;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repo;

    public DeviceService(IDeviceRepository repo)
    {
        _repo = repo;
    }

    public IEnumerable<Device> GetAllDevices() => _repo.GetAll();

    public Device? GetDeviceById(string id) => _repo.GetById(id);

    public bool AddDevice(Device device) => _repo.Add(device);

    public bool UpdateDevice(Device device) => _repo.Update(device);

    public bool RemoveDevice(string id) => _repo.Delete(id);
}
