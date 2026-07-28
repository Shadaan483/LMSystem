-- =====================================================================
-- LMS Day 3 / Day 4 setup script
-- Run this against your LMS database (same DB as Books13/BorrowRecords13)
-- in SQL Server Management Studio / Azure Data Studio before running the app.
-- The Books13 / BorrowRecords13 / Publications tables are created for you
-- automatically by EF Core migrations (see run instructions) -- do NOT
-- create those here. This script is only for the tables the Login,
-- Student, Librarian and Dashboard controllers read/write with raw ADO.NET.
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. Login table (reference only -- LoginController currently checks an
--    in-memory list, but this mirrors the PDF so you can verify data too)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.logintab', 'U') IS NULL
BEGIN
    CREATE TABLE logintab
    (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50),
        Password NVARCHAR(50)
    );

    INSERT INTO logintab (Username, Password) VALUES ('admin','12345');       -- admin
    INSERT INTO logintab (Username, Password) VALUES ('mycodingproject','myc546'); -- student
    INSERT INTO logintab (Username, Password) VALUES ('my','myc');           -- librarian
END
GO

-- ---------------------------------------------------------------------
-- 2. Students table (used by StudentController via raw SqlConnection)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Students', 'U') IS NULL
BEGIN
    CREATE TABLE Students
    (
        StudentId     INT IDENTITY(1,1) PRIMARY KEY,
        Student_Name  NVARCHAR(100) NOT NULL,
        Gender        NVARCHAR(20)  NULL,
        Email         NVARCHAR(100) NOT NULL,
        Phone_Number  NVARCHAR(20)  NOT NULL,
        Address       NVARCHAR(200) NULL
    );

    INSERT INTO Students (Student_Name, Gender, Email, Phone_Number, Address) VALUES
    ('Alice Johnson', 'Female', 'alice.j@email.com', '555-0101', '123 Maple Street'),
    ('Bob Smith', 'Male', 'bob.smith@email.com', '555-0102', '456 Oak Avenue'),
    ('Charlie Brown', 'Male', 'charlie.b@email.com', '555-0103', '789 Pine Road'),
    ('Diana Prince', 'Female', 'diana.p@email.com', '555-0104', '101 Bay Drive'),
    ('Evan Wright', 'Male', 'evan.w@email.com', '555-0105', '202 Cedar Lane');
END
GO

-- ---------------------------------------------------------------------
-- 3. Librarians table (used by LibrarianController via raw SqlConnection)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Librarians', 'U') IS NULL
BEGIN
    CREATE TABLE Librarians
    (
        LibrarianId INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Age         INT NOT NULL,
        Phone       NVARCHAR(20) NOT NULL
    );

    INSERT INTO Librarians (Name, Age, Phone) VALUES
    ('Sarah Connor', 34, '555-0201'),
    ('John Doe', 28, '555-0202'),
    ('Michael Scott', 45, '555-0203'),
    ('Ellen Ripley', 39, '555-0204'),
    ('James Bond', 40, '555-0205');
END
GO

-- ---------------------------------------------------------------------
-- 4. Sample data for Publications (table itself is created by the EF Core
--    migration for the Publication model -- run this AFTER migrations)
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Publications', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM Publications)
BEGIN
    -- Adding Sample Newspapers (Type = 0)
    INSERT INTO Publications (Title, Publisher, PublishedDate, [Type], IsAvailable)
    VALUES
    ('The Daily Times', 'Global Media Group', '2026-07-22', 0, 1),
    ('Financial Chronicle', 'WallSt Press', '2026-07-21', 0, 1),
    ('Tech Weekly News', 'Silicon Valley Pubs', '2026-07-20', 0, 1),
    ('Metro Morning Post', 'City Press House', '2026-07-22', 0, 1),
    ('Saturday Sports Herald', 'Global Media Group', '2026-07-18', 0, 0);

    -- Adding Sample Magazines (Type = 1)
    INSERT INTO Publications (Title, Publisher, PublishedDate, [Type], IsAvailable)
    VALUES
    ('National Geographic Vol 45', 'NatGeo Society', '2026-07-01', 1, 1),
    ('Vogue Fashion Summer', 'Cond\u00e9 Nast', '2026-06-15', 1, 1),
    ('Forbes Business 30 Under 30', 'Forbes Media', '2026-07-10', 1, 0),
    ('PC Gamer Ultimate', 'Future US', '2026-07-05', 1, 1),
    ('Scientific American', 'Springer Nature', '2026-06-28', 1, 1);
END
GO
