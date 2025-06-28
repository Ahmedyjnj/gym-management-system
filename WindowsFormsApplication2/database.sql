CREATE DATABASE gymData;
USE gymData;

CREATE TABLE Subscriptions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100),
    SubscriptionPeriod NVARCHAR(50),
    Type NVARCHAR(50),
    PaymentHasBeenMade BIT
);

CREATE TABLE Attendances (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SubscribeId INT NOT NULL,
    AttendanceDate DATETIME NOT NULL,
    FOREIGN KEY (SubscribeId) REFERENCES Subscriptions(Id)
);

CREATE TABLE Moneys (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SubscribeId INT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaidAmount DECIMAL(18,2) NOT NULL,
    PaymentDate DATETIME NOT NULL,
    FOREIGN KEY (SubscribeId) REFERENCES Subscriptions(Id)
);