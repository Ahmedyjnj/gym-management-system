# Gym Management System

This is a Windows Forms desktop application built with C# and Entity Framework Core (Database First) to manage gym subscriptions, attendance, and payments.

---

## 💻 Tech Stack

- Windows Forms (.NET Framework)
- C#
- SQL Server
- Entity Framework Core (Database First)
- Visual Studio 2022+

---

## 🚀 Features

- Add and manage subscriptions
- Track attendance per user
- Manage and log payments
- Display filtered data with summary rows
- Auto-detect end of subscription period

---

## 🖥️ How to Use



### 2. 📥 Clone the repository

Open Command Prompt (or Git Bash), then run:

```bash
  git [clone https://github.com/YourUsername/gym-management-system.git](https://github.com/Ahmedyjnj/gym-management-system.git)

### 3. 🧱 Set up the Database

You can create the required database and tables manually or use the following SQL script:

> 📄 [Click here to download database.sql](./database.sql)

Or copy and paste:

```sql
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

