namespace DeviceAPI;

public class Device
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    private int rowVersion {get;set;}

    public Device(string? id, string name, bool isEnabled)
    {
        Id = id;
        Name = name;
        IsEnabled = isEnabled;
    }

    public Device()
    {}

    public virtual void TurnOn()
    {
        IsEnabled = true;
    }

    public virtual void TurnOff()
    {
        IsEnabled = false;
    }
}