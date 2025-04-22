using System.Text;

namespace DeviceAPI;

public class DeviceManager
{
    
    private const int MaxCapacity = 15;
    public static List<Device> devices = new(capacity: MaxCapacity);

    public void AddDevice(Device newDevice)
    {
        foreach (var storedDevice in devices)
        {
            if (storedDevice.Id.Equals(newDevice.Id))
            {
                throw new ArgumentException($"Device with ID {storedDevice.Id} is already stored.", nameof(newDevice));
            }
        }

        if (devices.Count >= MaxCapacity)
        {
            throw new Exception("Device storage is full.");
        }
        
        devices.Add(newDevice);
    }

    public void EditDevice(Device editDevice)
    {
        var targetDeviceIndex = -1;
        for (var index = 0; index < devices.Count; index++)
        {
            var storedDevice = devices[index];
            if (storedDevice.Id.Equals(editDevice.Id))
            {
                targetDeviceIndex = index;
                break;
            }
        }

        if (targetDeviceIndex == -1)
        {
            throw new ArgumentException($"Device with ID {editDevice.Id} is not stored.", nameof(editDevice));
        }

        if (editDevice is Smartwatch)
        {
            if (devices[targetDeviceIndex] is Smartwatch)
            {
                devices[targetDeviceIndex] = editDevice;
            }
            else
            {
                throw new ArgumentException($"Type mismatch between devices. " +
                                            $"Target device has type {devices[targetDeviceIndex].GetType().Name}");
            }
        }
        
        if (editDevice is PersonalComputer)
        {
            if (devices[targetDeviceIndex] is PersonalComputer)
            {
                devices[targetDeviceIndex] = editDevice;
            }
            else
            {
                throw new ArgumentException($"Type mismatch between devices. " +
                                            $"Target device has type {devices[targetDeviceIndex].GetType().Name}");
            }
        }
        
        if (editDevice is Embedded)
        {
            if (devices[targetDeviceIndex] is Embedded)
            {
                devices[targetDeviceIndex] = editDevice;
            }
            else
            {
                throw new ArgumentException($"Type mismatch between devices. " +
                                            $"Target device has type {devices[targetDeviceIndex].GetType().Name}");
            }
        }
    }

    public void RemoveDeviceById(string deviceId)
    {
        Device? targetDevice = null;
        foreach (var storedDevice in devices)
        {
            if (storedDevice.Id.Equals(deviceId))
            {
                targetDevice = storedDevice;
                break;
            }
        }

        if (targetDevice == null)
        {
            throw new ArgumentException($"Device with ID {deviceId} is not stored.", nameof(deviceId));
        }
        
        devices.Remove(targetDevice);
    }

    public void TurnOnDevice(string id)
    {
        foreach (var storedDevice in devices)
        {
            if (storedDevice.Id.Equals(id))
            {
                storedDevice.TurnOn();
                return;
            }
        }
        
        throw new ArgumentException($"Device with ID {id} is not stored.", nameof(id));
    }

    public void TurnOffDevice(string id)
    {
        foreach (var storedDevice in devices)
        {
            if (storedDevice.Id.Equals(id))
            {
                storedDevice.TurnOff();
                return;
            }
        }
        
        throw new ArgumentException($"Device with ID {id} is not stored.", nameof(id));
    }

    public Device? GetDeviceById(string id)
    {
        foreach (var storedDevice in devices)
        {
            if (storedDevice.Id.Equals(id))
            {
                return storedDevice;
            }
        }

        return null;
    }

    public object? ShowAllDevices()
    {
        return devices;
    }

    public void SaveDevices(string outputPath)
    {
        StringBuilder devicesSb = new();

        foreach (var storedDevice in devices)
        {
            if (storedDevice is Smartwatch smartwatchCopy)
            {
                devicesSb.AppendLine($"{smartwatchCopy.Id},{smartwatchCopy.Name}," +
                                     $"{smartwatchCopy.IsEnabled},{smartwatchCopy.BatteryLevel}%");
            }
            else if (storedDevice is PersonalComputer pcCopy)
            {
                devicesSb.AppendLine($"{pcCopy.Id},{pcCopy.Name}," +
                                     $"{pcCopy.IsEnabled},{pcCopy.OperatingSystem}");
            }
            else
            {
                var embeddedCopy = storedDevice as Embedded;
                devicesSb.AppendLine($"{embeddedCopy.Id},{embeddedCopy.Name}," +
                                     $"{embeddedCopy.IsEnabled},{embeddedCopy.IpAddress}," +
                                     $"{embeddedCopy.NetworkName}");
            }
        }
        
        File.WriteAllLines(outputPath, devicesSb.ToString().Split('\n'));
    }
}