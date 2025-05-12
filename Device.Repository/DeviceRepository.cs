using System.Data;
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
                BatteryLevel = readerSW.GetInt32(0),
                RowVersion = readerSW.GetInt32(1)
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
                OperatingSystem = readerPC.GetString(0),
                RowVersion = readerSW.GetInt32(1)
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
                RowVersion = readerSW.GetInt32(2)
            };
        }

        readerED.Close();

        return null;}

    public bool Add(Device device)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            SqlCommand command = new()
            {
                Connection = connection,
                Transaction = transaction,
                CommandType = CommandType.StoredProcedure
            };

            switch (device)
            {
                case Smartwatch sw:
                    command.CommandText = "AddSmartwatch";
                    command.Parameters.AddWithValue("@DeviceId", sw.Id);
                    command.Parameters.AddWithValue("@Name", sw.Name);
                    command.Parameters.AddWithValue("@IsEnabled", sw.IsEnabled);
                    command.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
                    break;

                case PersonalComputer pc:
                    command.CommandText = "AddPC";
                    command.Parameters.AddWithValue("@DeviceId", pc.Id);
                    command.Parameters.AddWithValue("@Name", pc.Name);
                    command.Parameters.AddWithValue("@IsEnabled", pc.IsEnabled);
                    command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                    break;

                case Embedded ed:
                    command.CommandText = "AddEmbedded";
                    command.Parameters.AddWithValue("@DeviceId", ed.Id);
                    command.Parameters.AddWithValue("@Name", ed.Name);
                    command.Parameters.AddWithValue("@IsEnabled", ed.IsEnabled);
                    command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    break;

                default:
                    return false;
            }

            command.ExecuteNonQuery();
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }


    public bool Update(Device device) {using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        using SqlTransaction transaction = connection.BeginTransaction();
        try
        { 
            using (SqlCommand deviceCommand = new SqlCommand(
                       "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id and RowVer = @RowVersion", connection))
            {
                deviceCommand.Parameters.AddWithValue("@Id", device.Id);
                deviceCommand.Parameters.AddWithValue("@Name", device.Name);
                deviceCommand.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                deviceCommand.Parameters.AddWithValue("@RowVersion", device.RowVersion);
                deviceCommand.ExecuteNonQuery();
            }

            switch (device)
            {
                case Smartwatch sw:
                    using (SqlCommand swCommand = new SqlCommand(
                               "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId and RowVer = @RowVersion",
                               connection))
                    {
                        swCommand.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
                        swCommand.Parameters.AddWithValue("@DeviceId", sw.Id);
                        swCommand.Parameters.AddWithValue("@RowVersion", sw.RowVersion);
                        swCommand.ExecuteNonQuery();
                    }

                    break;
                
                case PersonalComputer pc:
                    using (SqlCommand pcCommand = new SqlCommand(
                               "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId and RowVer = @RowVersion",
                               connection))
                    {
                        pcCommand.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                        pcCommand.Parameters.AddWithValue("@DeviceId", pc.Id);
                        pcCommand.Parameters.AddWithValue("@RowVersion", pc.RowVersion);
                        pcCommand.ExecuteNonQuery();
                    }

                    break;

                case Embedded ed:
                    using (SqlCommand edCommand = new SqlCommand(
                               "UPDATE Embedded SET NetworkName = @NetworkName, IpAddress = @IpAddress, IsConnected = @IsConnected WHERE DeviceId = @DeviceId and RowVer = @RowVersion",
                               connection))
                    {
                        edCommand.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                        edCommand.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                        edCommand.Parameters.AddWithValue("@IsConnected", ed.IsConnected);
                        edCommand.Parameters.AddWithValue("@DeviceId", ed.Id);
                        edCommand.Parameters.AddWithValue("@RowVersion", ed.RowVersion);
                        edCommand.ExecuteNonQuery();
                    }

                    break;
            }
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {   
            transaction.Rollback();
            return false;
        } }

    public bool Delete(string id) {using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();
        using SqlTransaction transaction = connection.BeginTransaction();
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
            transaction.Commit();
            return rowsDeleted > -1;
        }
        catch
        {   
            transaction.Rollback();
            return false;
        } }
}
