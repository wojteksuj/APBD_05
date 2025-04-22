using Microsoft.Data.SqlClient;
using DeviceAPI;

public class DeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceAPI.Device> GetAllDevices()
    {
        List<DeviceAPI.Device> devices = [];
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
    
    
    
}