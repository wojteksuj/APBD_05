
INSERT INTO Device (Id, Name, IsEnabled) VALUES
        ('ED-1', 'Thermostat', 1),
        ('P-1', 'ThinkPad', 1),
        ('SW-1', 'Apple Watch', 0);


INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES
    ( '192.168.1.44', 'Home_Network', 'ED-1');


INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES
    ('Windows 11', 'P-1');


INSERT INTO Smartwatch ( BatteryPercentage, DeviceId) VALUES
    (78, 'SW-1');
