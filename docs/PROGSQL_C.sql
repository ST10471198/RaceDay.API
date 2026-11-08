-- ============================================================
-- Database: RaceDay System
-- Description: Full database schema for RaceDay event management
-- ============================================================

USE master;
GO

-- Drop database if it exists
IF DB_ID('RaceDayDB') IS NOT NULL 
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END 
GO

-- Create the database
CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- 1. Organiser table
CREATE TABLE Organiser
(
    organiserID INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    phone VARCHAR(20) NOT NULL,
    passwordHash VARCHAR(255) NOT NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 2. Participant table
CREATE TABLE Participant
(
    participantID INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    passwordHash VARCHAR(255) NOT NULL,
    dateOfBirth DATE NOT NULL,
    idNumber VARCHAR(50) NOT NULL UNIQUE,
    emergencyContact VARCHAR(100) NOT NULL,
    createdAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 3. Event table
CREATE TABLE Event
(
    eventID INT IDENTITY(1,1) PRIMARY KEY,
    organiserID INT NOT NULL,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(500) NULL,
    date DATE NOT NULL,
    location VARCHAR(200) NOT NULL,
    maxParticipants INT NOT NULL CHECK (maxParticipants > 0),
    status VARCHAR(50) NOT NULL DEFAULT 'Upcoming' 
        CHECK (status IN ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled')),
    BannerImageUrl VARCHAR(500) NULL, -- Added for Phase 1

    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (organiserID)
        REFERENCES Organiser(organiserID)
        ON DELETE CASCADE
);
GO

-- 4. Category table
CREATE TABLE Category
(
    categoryID INT IDENTITY(1,1) PRIMARY KEY,
    eventID INT NOT NULL,
    name VARCHAR(100) NOT NULL,
    distanceKM DECIMAL(6,2) NOT NULL CHECK (distanceKM > 0),
    startTime TIME NOT NULL,
    ageMin INT NOT NULL CHECK (ageMin >= 0),
    ageMax INT NOT NULL, -- Removed the column-level check here
    entryFee DECIMAL(10,2) NOT NULL CHECK (entryFee >= 0),

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (eventID)
        REFERENCES Event(eventID)
        ON DELETE CASCADE,

    -- FIXED: Table-level constraint for checking age range
    CONSTRAINT CHK_Category_AgeRange CHECK (ageMax > ageMin)
);
GO

-- 5. Weather table
CREATE TABLE Weather
(
    weatherID INT IDENTITY(1,1) PRIMARY KEY,
    eventID INT NOT NULL,
    forecastDate DATE NOT NULL,
    temperature DECIMAL(5,2) NULL,
    condition VARCHAR(100) NULL,
    windSpeed DECIMAL(5,2) NULL,
    humidity DECIMAL(5,2) NULL,

    CONSTRAINT FK_Weather_Event
        FOREIGN KEY (eventID)
        REFERENCES Event(eventID)
        ON DELETE CASCADE,
    
    CONSTRAINT UQ_Weather_Event_Date UNIQUE (eventID, forecastDate)
);
GO

-- 6. Result table
CREATE TABLE Result
(
    resultID INT IDENTITY(1,1) PRIMARY KEY,
    eventID INT NOT NULL,
    participantID INT NOT NULL,
    categoryID INT NOT NULL,
    finishTime TIME NULL,
    position INT NULL CHECK (position > 0),
    status VARCHAR(50) NOT NULL DEFAULT 'Registered'
        CHECK (status IN ('Registered', 'Started', 'Finished', 'DNF', 'DNS', 'Disqualified')),

    CONSTRAINT FK_Result_Event
        FOREIGN KEY (eventID)
        REFERENCES Event(eventID)
        ON DELETE CASCADE,

    CONSTRAINT FK_Result_Participant
        FOREIGN KEY (participantID)
        REFERENCES Participant(participantID)
        ON DELETE CASCADE,

    CONSTRAINT FK_Result_Category
        FOREIGN KEY (categoryID)
        REFERENCES Category(categoryID),
    
    CONSTRAINT UQ_Result_Event_Participant UNIQUE (eventID, participantID)
);
GO

-- Create Indexes
CREATE INDEX IX_Event_OrganiserID ON Event(organiserID);
CREATE INDEX IX_Category_EventID ON Category(eventID);
CREATE INDEX IX_Weather_EventID ON Weather(eventID);
CREATE INDEX IX_Result_EventID ON Result(eventID);
CREATE INDEX IX_Result_ParticipantID ON Result(participantID);
CREATE INDEX IX_Result_CategoryID ON Result(categoryID);
CREATE INDEX IX_Event_Date ON Event(date);
CREATE INDEX IX_Event_Status ON Event(status);
CREATE INDEX IX_Participant_DateOfBirth ON Participant(dateOfBirth);
GO

-- ============================================================
-- INSERT SEED DATA
-- ============================================================

-- Organisers
INSERT INTO Organiser (name, email, phone, passwordHash, createdAt)
VALUES 
    ('Cape Town Marathon Association', 'info@capetownmarathon.co.za', '+27 21 555 0123', 'hashed_password_1', GETDATE()),
    ('Joburg Sports Events', 'events@joburgsports.co.za', '+27 11 555 0456', 'hashed_password_2', GETDATE()),
    ('Durban Running Club', 'info@durbanrunning.co.za', '+27 31 555 0789', 'hashed_password_3', GETDATE());
GO

-- Participants
INSERT INTO Participant (name, email, passwordHash, dateOfBirth, idNumber, emergencyContact, createdAt)
VALUES 
    ('Thabo Mokoena', 'thabo.m@email.com', 'hashed_password_p1', '1990-03-15', '9003151234567', 'Lerato Mokoena - 082 123 4567', GETDATE()),
    ('Sarah Johnson', 'sarah.j@email.com', 'hashed_password_p2', '1985-07-22', '8507221234567', 'Mike Johnson - 083 234 5678', GETDATE()),
    ('Michael Ndlovu', 'michael.n@email.com', 'hashed_password_p3', '1995-11-08', '9511081234567', 'Precious Ndlovu - 084 345 6789', GETDATE()),
    ('Emma Williams', 'emma.w@email.com', 'hashed_password_p4', '1988-09-30', '8809301234567', 'James Williams - 085 456 7890', GETDATE());
GO

-- Events
INSERT INTO Event (organiserID, name, description, date, location, maxParticipants, status)
VALUES 
    (1, 'Cape Town Marathon 2026', 'The premier marathon event in South Africa, featuring a scenic route along the Atlantic seaboard.', '2026-10-15', 'Green Point Stadium, Cape Town', 15000, 'Open'),
    (2, 'Joburg City Run 2026', 'A fast and flat city course through the heart of Johannesburg, perfect for PB attempts.', '2026-09-20', 'Sandton Convention Centre, Johannesburg', 8000, 'Open'),
    (3, 'Durban Beach Run 2026', 'A stunning coastal run along the Golden Mile, offering beautiful ocean views.', '2026-11-05', 'Durban Beachfront, Durban', 5000, 'Upcoming');
GO

-- Categories
INSERT INTO Category (eventID, name, distanceKM, startTime, ageMin, ageMax, entryFee)
VALUES 
    (1, 'Full Marathon (Elite)', 42.20, '06:00:00', 18, 65, 850.00),
    (1, 'Half Marathon', 21.10, '06:30:00', 16, 70, 550.00),
    (1, 'Fun Run (10km)', 10.00, '07:30:00', 12, 75, 250.00),
    (2, '21km Run', 21.10, '07:00:00', 16, 65, 450.00),
    (2, '10km Challenge', 10.00, '07:30:00', 14, 70, 300.00),
    (2, '5km Fun Walk', 5.00, '08:00:00', 8, 75, 150.00),
    (3, '15km Beach Run', 15.00, '06:30:00', 16, 60, 400.00),
    (3, '8km Coastal Run', 8.00, '07:00:00', 14, 65, 280.00),
    (3, '5km Family Run', 5.00, '07:30:00', 6, 75, 180.00);
GO

-- Weather
INSERT INTO Weather (eventID, forecastDate, temperature, condition, windSpeed, humidity)
VALUES 
    (1, '2026-10-12', 18.5, 'Partly Cloudy', 12.5, 65.0),
    (1, '2026-10-13', 19.0, 'Sunny', 10.0, 60.0),
    (1, '2026-10-14', 17.5, 'Light Rain', 15.0, 75.0),
    (1, '2026-10-15', 20.0, 'Sunny', 8.0, 55.0),
    (2, '2026-09-17', 22.0, 'Clear', 5.0, 45.0),
    (2, '2026-09-18', 23.5, 'Sunny', 7.0, 42.0),
    (2, '2026-09-19', 21.0, 'Partly Cloudy', 10.0, 50.0),
    (2, '2026-09-20', 24.0, 'Sunny', 6.0, 40.0),
    (3, '2026-11-02', 25.0, 'Sunny', 8.0, 65.0),
    (3, '2026-11-03', 26.5, 'Sunny', 10.0, 60.0),
    (3, '2026-11-04', 24.0, 'Partly Cloudy', 12.0, 70.0),
    (3, '2026-11-05', 27.0, 'Sunny', 7.0, 55.0);
GO

-- Results (Enrolments)
INSERT INTO Result (eventID, participantID, categoryID, status)
VALUES 
    (1, 1, 1, 'Registered'), 
    (1, 2, 2, 'Registered'), 
    (1, 3, 3, 'Registered'), 
    (1, 4, 2, 'Registered'),
    (2, 1, 4, 'Registered'), 
    (2, 2, 5, 'Registered'), 
    (2, 3, 6, 'Registered'),
    (3, 4, 8, 'Registered'), 
    (3, 2, 9, 'Registered'), 
    (3, 1, 7, 'Registered');
GO

ALTER TABLE Participant ADD ProfilePictureUrl VARCHAR(500) NULL;