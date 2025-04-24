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
                    "INSERT INTO Smartwatch (DeviceId, BatteryPercentage) VALUES (@DeviceId, @BatteryLevel)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", sw.Id);
                queryString.Parameters.AddWithValue("@BatteryLevel", sw.BatteryLevel);
                break;

            case PersonalComputer pc:
                queryString = new SqlCommand(
                    "INSERT INTO PC (DeviceId, OperationSystem) VALUES (@DeviceId, @OperatingSystem)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", pc.Id);
                queryString.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem);
                break;

            case Embedded ed:
                queryString = new SqlCommand(
                    "INSERT INTO Embedded (DeviceId, NetworkName, IpAddress) VALUES (@DeviceId, @NetworkName, @IpAddress)", connection);
                queryString.Parameters.AddWithValue("@DeviceId", ed.Id);
                queryString.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                queryString.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
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
    
    const string baseQuery = "SELECT Name, IsEnabled FROM Device WHERE Id = @Id";
    using SqlCommand baseCommand = new SqlCommand(baseQuery, connection);
    baseCommand.Parameters.AddWithValue("@Id", id);

    using SqlDataReader baseReader = baseCommand.ExecuteReader();
    if (!baseReader.Read())
        return null;

    string name = baseReader.GetString(0);
    bool isEnabled = baseReader.GetBoolean(1);

    baseReader.Close(); 
    
    string query = "SELECT BatteryPercentage FROM Smartwatch WHERE DeviceId = @Id";
    using SqlCommand commandSW = new SqlCommand(query, connection);
    commandSW.Parameters.AddWithValue("@Id", id);
    using SqlDataReader readerSW = commandSW.ExecuteReader();
    if (readerSW.Read())
    {
        return new Smartwatch
        {
            Id = id,
            Name = name,
            IsEnabled = isEnabled,
            BatteryLevel = readerSW.GetInt32(0)
        };
    }
    readerSW.Close();
    
    query = "SELECT OperationSystem FROM PersonalComputer WHERE DeviceId = @Id";
    using SqlCommand commandPC = new SqlCommand(query, connection);
    commandPC.Parameters.AddWithValue("@Id", id);
    using SqlDataReader readerPC = commandPC.ExecuteReader();
    if (readerPC.Read())
    {
        return new PersonalComputer
        {
            Id = id,
            Name = name,
            IsEnabled = isEnabled,
            OperatingSystem = readerPC.GetString(0)
        };
    }
    readerPC.Close();
    
    query = "SELECT IpAddress, NetworkName FROM Embedded WHERE DeviceId = @Id";
    using SqlCommand commandED = new SqlCommand(query, connection);
    commandED.Parameters.AddWithValue("@Id", id);
    using SqlDataReader readerED = commandED.ExecuteReader();
    if (readerED.Read())
    {
        return new Embedded
        {
            Id = id,
            Name = name,
            IsEnabled = isEnabled,
            IpAddress = readerED.GetString(0),
            NetworkName = readerED.GetString(1),
        };
    }
    readerED.Close();

    return null;
}

    public bool RemoveDevice(string id)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        try
        {
            // Step 1: Delete from subtype tables first
            string[] deleteQueries = {
                "DELETE FROM Smartwatch WHERE DeviceId = @Id",
                "DELETE FROM PersonalComputer WHERE DeviceId = @Id",
                "DELETE FROM Embedded WHERE DeviceId = @Id"
            };

            foreach (var query in deleteQueries)
            {
                using SqlCommand commandString = new SqlCommand(query, connection);
                commandString.Parameters.AddWithValue("@Id", id);
                commandString.ExecuteNonQuery();
            }
            
            const string deleteQuery= "DELETE FROM Device WHERE Id = @Id";
            using SqlCommand deviceCmd = new SqlCommand(deleteQuery, connection);
            deviceCmd.Parameters.AddWithValue("@Id", id);
            
            int rowsDeleted = deviceCmd.ExecuteNonQuery();
            return rowsDeleted > -1;
        }
        catch
        {
            return false;
        }
    }
    
    
    
}