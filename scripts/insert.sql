
INSERT INTO Device (Id, Name, IsEnabled) VALUES
                                             ('ED-1', 'Thermostat', 1),
                                             ('P-1', 'ThinkPad', 1),
                                             ('SW-1', 'Apple Watch', 0);


INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES
    (1, '192.168.1.100', 'Home_Network', 'ED-1');


INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId) VALUES
    (1, 'Windows 11', 'P-1');


INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId) VALUES
    (1, 78, 'SW-1');
