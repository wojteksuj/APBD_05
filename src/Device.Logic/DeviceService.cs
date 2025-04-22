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
        List<Device> devices = [];
        const string queryString = "SELECT * FROM Devices";

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
        string queryString;

        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
        command.Parameters.AddWithValue("@Type", device.GetType().Name);

        switch (device)
        {
            case Smartwatch sw:
                queryString = "INSERT INTO Devices (Id, Name, IsEnabled, Type, BatteryLevel) VALUES (@Id, @Name, @IsEnabled, @Type, @BatteryLevel)";
                command.Parameters.AddWithValue("@BatteryLevel", sw.BatteryLevel);
                break;

            case PersonalComputer pc:
                queryString = "INSERT INTO Devices (Id, Name, IsEnabled, Type, OperatingSystem) VALUES (@Id, @Name, @IsEnabled, @Type, @OperatingSystem)";
                command.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem);
                break;

            case Embedded ed:
                queryString = "INSERT INTO Devices (Id, Name, IsEnabled, Type, NetworkName, IpAddress, IsConnected) VALUES (@Id, @Name, @IsEnabled, @Type, @NetworkName, @IpAddress, @IsConnected)";
                command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                command.Parameters.AddWithValue("@IsConnected", ed.IsConnected);
                break;
        }
        var addedRows = command.ExecuteNonQuery();
        return addedRows != -1;
    }

    public Device? GetDeviceById(string id)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        const string queryString = "SELECT * FROM Devices WHERE Id = @Id";
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
    
    
    
}