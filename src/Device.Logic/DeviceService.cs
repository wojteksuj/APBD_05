using System.Data;
using Microsoft.Data.SqlClient;
using DeviceAPI;

public class DeviceService : IDeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAllDevices()
    {
        Console.WriteLine("GetAllDevices was called");
        List<Device> devices = [];
        const string queryString = "SELECT Id, Name, IsEnabled FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var deviceRow = new Device
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsEnabled = reader.GetBoolean(2),
                        };
                        devices.Add(deviceRow);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return devices;
    }

    public bool AddDevice(Device device)
    {
    using SqlConnection connection = new SqlConnection(_connectionString);
    connection.Open();
    
    try
    {
        using (SqlCommand queryDevice = new SqlCommand(
            "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)", connection))
        {
            queryDevice.Parameters.AddWithValue("@Id", device.Id);
            queryDevice.Parameters.AddWithValue("@Name", device.Name);
            queryDevice.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
        }

        SqlCommand? queryString = null;

        switch (device)
        {
            case Smartwatch sw:
                queryString = new SqlCommand(
                    "INSERT INTO Smartwatch (DeviceId, BatteryLevel) VALUES (@DeviceId, @BatteryLevel)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", sw.Id);
                queryString.Parameters.AddWithValue("@BatteryLevel", sw.BatteryLevel);
                break;

            case PersonalComputer pc:
                queryString = new SqlCommand(
                    "INSERT INTO PC (DeviceId, OperatingSystem) VALUES (@DeviceId, @OperatingSystem)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", pc.Id);
                queryString.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem);
                break;

            case Embedded ed:
                queryString = new SqlCommand(
                    "INSERT INTO Embedded (DeviceId, NetworkName, IpAddress, IsConnected) VALUES (@DeviceId, @NetworkName, @IpAddress, @IsConnected)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", ed.Id);
                queryString.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                queryString.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                queryString.Parameters.AddWithValue("@IsConnected", ed.IsConnected);
                break;
        }
        return true;
    }
    catch
    {
        return false;
    }
}


    public Device? GetDeviceById(string id)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        const string queryString = "SELECT * FROM Device WHERE Id = @Id";
        using SqlCommand command = new SqlCommand(queryString, connection);
        command.Parameters.AddWithValue("@Id", id);

        using SqlDataReader reader = command.ExecuteReader();

        string type = reader.GetString(0);

        Device? device = type switch
        {
            "Smartwatch" => new Smartwatch
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsEnabled = reader.GetBoolean(2),
                BatteryLevel = reader.GetInt32(3)
            },

            "PersonalComputer" => new PersonalComputer
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsEnabled = reader.GetBoolean(2),
                OperatingSystem = reader.GetString(3)
            },

            "EmbeddedDevice" => new Embedded
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsEnabled = reader.GetBoolean(2),
                IpAddress = reader.GetString(3),
                NetworkName = reader.GetString(4),
                IsConnected = reader.GetBoolean(5)
            }
        };
        
        return device;
    }

    public bool RemoveDevice(string id)
    {
        const string queryString = "DELETE FROM Devices WHERE Id = @Id";
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        using SqlCommand command = new SqlCommand(queryString, connection);
        command.Parameters.AddWithValue("@Id", id);

        int rowsDeleted = command.ExecuteNonQuery();
        return rowsDeleted != -1;
    }
    
    public bool UpdateDevice(Device device)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        string queryString = "";
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
        command.Parameters.AddWithValue("@Type", device.GetType().Name);

        switch (device)
        {
            case Smartwatch sw:
                queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, Type = @Type, BatteryLevel = @BatteryLevel WHERE Id = @Id";
                command.Parameters.AddWithValue("@BatteryLevel", sw.BatteryLevel);
                break;

            case PersonalComputer pc:
                queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, Type = @Type, OperatingSystem = @OperatingSystem WHERE Id = @Id";
                command.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem);
                break;

            case Embedded ed:
                queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, Type = @Type, NetworkName = @NetworkName, IpAddress = @IpAddress, IsConnected = @IsConnected WHERE Id = @Id";
                command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                command.Parameters.AddWithValue("@IsConnected", ed.IsConnected);
                break;
        }

        command.CommandText = queryString;
        int rowsUpdated = command.ExecuteNonQuery();
        return rowsUpdated != -1;
    }

    
}