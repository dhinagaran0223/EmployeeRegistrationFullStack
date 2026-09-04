IF DB_ID('EmployeeRegistrationDb') IS NULL
BEGIN
    CREATE DATABASE EmployeeRegistrationDb;
END
GO

USE EmployeeRegistrationDb;
GO

IF OBJECT_ID('dbo.Country_Mst', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Country_Mst
    (
        CountryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CountryName VARCHAR(100) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.State_Mst', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.State_Mst
    (
        StateId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StateName VARCHAR(100) NOT NULL,
        CountryId INT NOT NULL,
        CONSTRAINT FK_State_Country FOREIGN KEY (CountryId)
            REFERENCES dbo.Country_Mst(CountryId)
    );
END
GO

IF OBJECT_ID('dbo.Employee_Mst', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employee_Mst
    (
        EmployeeId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Employee Name] VARCHAR(30) NOT NULL,
        Age INT NOT NULL,
        MobileNum VARCHAR(10) NOT NULL,
        Pincode VARCHAR(6) NOT NULL,
        DOB DATETIME2 NULL,
        AddressLine1 VARCHAR(250) NOT NULL,
        AddressLine2 VARCHAR(250) NULL,
        StateId INT NOT NULL,
        CountryId INT NOT NULL,
        CONSTRAINT FK_Employee_State FOREIGN KEY (StateId)
            REFERENCES dbo.State_Mst(StateId),
        CONSTRAINT FK_Employee_Country FOREIGN KEY (CountryId)
            REFERENCES dbo.Country_Mst(CountryId),
        CONSTRAINT UQ_Employee_Mobile UNIQUE (MobileNum)
    );
END
GO
