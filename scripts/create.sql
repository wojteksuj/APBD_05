CREATE TABLE Device (
                        Id VARCHAR(255) PRIMARY KEY,
                        Name NVARCHAR(255),
                        IsEnabled BIT
);

CREATE TABLE Embedded (
                          Id INT IDENTITY PRIMARY KEY ,
                          IpAddress VARCHAR(255),
                          NetworkName VARCHAR(255),
                          DeviceId VARCHAR(255),
                          FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE PersonalComputer (
                                  Id INT IDENTITY PRIMARY KEY,
                                  OperationSystem VARCHAR(255),
                                  DeviceId VARCHAR(255),
                                  FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE Smartwatch (
                            Id INT IDENTITY PRIMARY KEY,
                            BatteryPercentage INT,
                            DeviceId VARCHAR(255),
                            FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);



