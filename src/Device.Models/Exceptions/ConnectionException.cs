namespace DeviceAPI.Exceptions;

class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong netowrk name.") { }
}