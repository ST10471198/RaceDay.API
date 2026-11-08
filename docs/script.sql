USE master;
GO
IF DB_ID('RaceDayDB') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayDB;
END
GO
 
CREATE DATABASE RaceDayDB;
GO
 
USE RaceDayDB;
GO
 
 
CREATE TABLE Users (
    UserID        INT IDENTITY(1,1) PRIMARY KEY,
    FullName      VARCHAR(100)  NOT NULL,
    Email         VARCHAR(150)  NOT NULL UNIQUE,
    PasswordHash  VARCHAR(255)  NOT NULL,
    PhoneNumber   VARCHAR(20)   NULL,
    Role          VARCHAR(20)   NOT NULL CHECK (Role IN ('Organiser','Participant')),
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE()
);
 
CREATE TABLE Events (
    EventID       INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID   INT           NOT NULL,
    EventName     VARCHAR(150)  NOT NULL,
    EventDate     DATE          NOT NULL,
    Location      VARCHAR(150)  NOT NULL,
    Description   VARCHAR(1000) NULL,
    Latitude      DECIMAL(9,6)  NULL,
    Longitude     DECIMAL(9,6)  NULL,
    CreatedAt     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Events_Organiser FOREIGN KEY (OrganiserID) REFERENCES Users(UserID)
);
 
CREATE TABLE Routes (
    RouteID       INT IDENTITY(1,1) PRIMARY KEY,
    EventID       INT           NOT NULL,
    RouteName     VARCHAR(100)  NOT NULL,
    DistanceKm    DECIMAL(5,2)  NOT NULL,
    Description   VARCHAR(1000) NULL,
    MapUrl        VARCHAR(255)  NULL,
    CONSTRAINT FK_Routes_Event FOREIGN KEY (EventID) REFERENCES Events(EventID)
);
 
CREATE TABLE Categories (
    CategoryID    INT IDENTITY(1,1) PRIMARY KEY,
    EventID       INT           NOT NULL,
    CategoryName  VARCHAR(100)  NOT NULL,
    DistanceKm    DECIMAL(5,2)  NOT NULL,
    EntryFee      DECIMAL(8,2)  NOT NULL DEFAULT 0,
    CONSTRAINT FK_Categories_Event FOREIGN KEY (EventID) REFERENCES Events(EventID)
);
 
CREATE TABLE Enrolments (
    EnrolmentID    INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID  INT          NOT NULL,
    CategoryID     INT          NOT NULL,
    EnrolmentDate  DATETIME     NOT NULL DEFAULT GETDATE(),
    Status         VARCHAR(20)  NOT NULL DEFAULT 'Confirmed',
    CONSTRAINT FK_Enrolments_Participant FOREIGN KEY (ParticipantID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrolments_Category FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT UQ_Participant_Category UNIQUE (ParticipantID, CategoryID)
);
 
CREATE TABLE Results (
    ResultID     INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID  INT          NOT NULL UNIQUE,
    FinishTime   TIME         NULL,
    Position     INT          NULL,
    Status       VARCHAR(20)  NOT NULL DEFAULT 'Finished',
    CONSTRAINT FK_Results_Enrolment FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);
GO
 
/* ---------- SEED DATA ---------- */
 
-- 2 Organisers, 2 Participants
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, Role) VALUES
('Thabo Nkosi',   'thabo.nkosi@raceday.co.za',   'HASHED_PW_1', '0821234567', 'Organiser'),
('Lindiwe Zulu',  'lindiwe.zulu@raceday.co.za',  'HASHED_PW_2', '0837654321', 'Organiser'),
('Johan van Wyk', 'johan.vanwyk@example.com',    'HASHED_PW_3', '0731112222', 'Participant'),
('Aisha Patel',   'aisha.patel@example.com',     'HASHED_PW_4', '0745556666', 'Participant');
 
-- 3 Events (owned by the two organisers)
INSERT INTO Events (OrganiserID, EventName, EventDate, Location, Description, Latitude, Longitude) VALUES
(1, 'Comrades Marathon 2027',     '2027-06-13', 'Pietermaritzburg to Durban', 'The ultimate human race - an ultra-marathon between PMB and Durban.', -29.6006, 30.3794),
(1, 'Cape Town Cycle Tour 2027',  '2027-03-08', 'Cape Town',                  'Iconic 109km cycling tour around the Cape Peninsula.', -33.9249, 18.4241),
(2, 'Soweto Marathon 2027',       '2027-11-07', 'Soweto, Johannesburg',       'Community road race through the streets of Soweto.', -26.2485, 27.8540);
 
-- Routes for each event
INSERT INTO Routes (EventID, RouteName, DistanceKm, Description, MapUrl) VALUES
(1, 'Down Run Route', 87.70, 'Traditional down run from Pietermaritzburg to Durban.', 'https://maps.example.com/comrades-2027'),
(2, 'Peninsula Loop',  109.00, 'Coastal loop around the Cape Peninsula.', 'https://maps.example.com/cycletour-2027'),
(3, 'Soweto Loop',      42.20, 'Marathon route through Soweto township.', 'https://maps.example.com/soweto-2027');
 
-- Categories per event
INSERT INTO Categories (EventID, CategoryName, DistanceKm, EntryFee) VALUES
(1, 'Full Ultra (87km)', 87.70, 950.00),
(2, '109km Individual',  109.00, 750.00),
(2, '55km Half Cycle',   55.00, 450.00),
(3, 'Full Marathon (42km)', 42.20, 350.00),
(3, '21km Half Marathon',   21.10, 250.00),
(3, '10km Fun Run',         10.00, 150.00);
 
-- Sample enrolments (participants entering categories)
INSERT INTO Enrolments (ParticipantID, CategoryID, Status) VALUES
(3, 1, 'Confirmed'),  -- Johan entered Comrades Full Ultra
(3, 4, 'Confirmed'),  -- Johan entered Soweto Full Marathon
(4, 3, 'Confirmed'),  -- Aisha entered 55km Half Cycle
(4, 5, 'Confirmed');  -- Aisha entered Soweto 21km
 
-- Sample results
INSERT INTO Results (EnrolmentID, FinishTime, Position, Status) VALUES
(1, '09:15:32', 1240, 'Finished'),
(3, '02:45:10', 87,   'Finished');
GO
 
PRINT 'RaceDay schema created and seeded successfully.';
