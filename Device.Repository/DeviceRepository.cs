using Microsoft.Data.SqlClient;

namespace DeviceAPI;
using DeviceAPI;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<Device> GetAll() {List<Device> devices = [];
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

        return devices;}

    public Device? GetById(string id) { using SqlConnection connection = new SqlConnection(_connectionString);
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

        return null;}

    public bool Add(Device device) {using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        try
        {
            SqlCommand? queryString = null;

            switch (device)
            {
                case Smartwatch sw:
                    queryString = new SqlCommand(
                        "INSERT INTO Smartwatch (DeviceId, BatteryPercentage) VALUES (@DeviceId, @BatteryLevel)",
                        connection);
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
                        "INSERT INTO Embedded (DeviceId, NetworkName, IpAddress) VALUES (@DeviceId, @NetworkName, @IpAddress)",
                        connection);
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
        }}

    public bool Update(Device device) {using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        try
        {
            using (SqlCommand deviceCommand = new SqlCommand(
                       "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id", connection))
            {
                deviceCommand.Parameters.AddWithValue("@Id", device.Id);
                deviceCommand.Parameters.AddWithValue("@Name", device.Name);
                deviceCommand.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                deviceCommand.ExecuteNonQuery();
            }

            switch (device)
            {
                case Smartwatch sw:
                    using (SqlCommand swCommand = new SqlCommand(
                               "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId",
                               connection))
                    {
                        swCommand.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
                        swCommand.Parameters.AddWithValue("@DeviceId", sw.Id);
                        swCommand.ExecuteNonQuery();
                    }

                    break;

                case PersonalComputer pc:
                    using (SqlCommand pcCommand = new SqlCommand(
                               "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId",
                               connection))
                    {
                        pcCommand.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                        pcCommand.Parameters.AddWithValue("@DeviceId", pc.Id);
                        pcCommand.ExecuteNonQuery();
                    }

                    break;

                case Embedded ed:
                    using (SqlCommand edCommand = new SqlCommand(
                               "UPDATE Embedded SET NetworkName = @NetworkName, IpAddress = @IpAddress, IsConnected = @IsConnected WHERE DeviceId = @DeviceId",
                               connection))
                    {
                        edCommand.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                        edCommand.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                        edCommand.Parameters.AddWithValue("@IsConnected", ed.IsConnected);
                        edCommand.Parameters.AddWithValue("@DeviceId", ed.Id);
                        edCommand.ExecuteNonQuery();
                    }

                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        } }

    public bool Delete(string id) {using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        try
        {
            string[] deleteQueries =
            {
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

            const string deleteQuery = "DELETE FROM Device WHERE Id = @Id";
            using SqlCommand deviceCmd = new SqlCommand(deleteQuery, connection);
            deviceCmd.Parameters.AddWithValue("@Id", id);

            int rowsDeleted = deviceCmd.ExecuteNonQuery();
            return rowsDeleted > -1;
        }
        catch
        {
            return false;
        } }
}
