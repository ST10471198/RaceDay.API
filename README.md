# RaceDay API - PROG6212 Part 2

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
[![.NET CI](https://github.com/ST10471198/RaceDay-API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ST10471198/RaceDay-API/actions/workflows/dotnet.yml)

A RESTful API built with ASP.NET Core Web API for managing road running, walking, and cycling events in South Africa. This project allows Organisers to manage events and Participants to enrol and track their results.

## 📋 Project Overview

This is **Part 2** of the PROG6212 Portfolio of Evidence (PoE). It implements the RESTful API layer for the RaceDay event management system, built on top of the plan and database schema designed in Part 1.

The API:
- Uses **ASP.NET Core Web API** (.NET 8)
- Connects to **SQL Server** via **Entity Framework Core**
- Implements **session-based authentication** with role-based access control
- Has **comprehensive unit tests** using xUnit and EF Core InMemory
- Uses **GitHub Actions** for continuous integration (build + test on every push)

## 🚀 Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or Express) or SQL Server Management Studio (SSMS)
- Visual Studio 2022 (with the **ASP.NET and web development** workload)

### Steps to Run Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ST10471198/RaceDay-API.git
   cd RaceDay-API
   API Endpoints Summary
Controller	Base Route	Description
Auth	/api/auth	Register, Login, Logout
Organisers	/api/organisers	Organiser profile management
Events	/api/events	Event CRUD operations
Categories	/api/events/{id}/categories	Event category management
Weather	/api/events/{id}/weather	Live weather forecasts for events
Participants	/api/participants	Participant profile management
Results	/api/events/{id}/results	Enrolments and race results
See the full endpoint list in Swagger UI once the API is running (https://localhost:7012/swagger).

🧪 Unit Testing
Tests are written with xUnit and the EF Core InMemory provider, so they don't require a real database.

Test Coverage
AuthControllerTests — Organiser/Participant registration, duplicate email conflict, invalid role rejection, successful login, failed login

EventsControllerTests — Participants can't create events (401), Organisers can create events (201), empty event list returns 404

ResultsControllerTests — Enrolment rules (Participant can enrol, Organiser cannot, duplicate enrolment rejected)

Run the Tests
In Visual Studio:

Test → Test Explorer → Run All Tests

Or from the command line:

powershell
dotnet test
🔄 CI/CD Pipeline
This project uses GitHub Actions for continuous integration. The workflow file is at .github/workflows/dotnet.yml.

On every push to master or main, GitHub Actions will:

Restore NuGet dependencies

Build the entire solution

Run all unit tests

Successful CI Build
Below is a screenshot of a successful green build with all tests passing:

https://docs/ci-screenshot.png

(Create a docs folder in the repo, upload the screenshot as ci-screenshot.png, and it will render here.)

📁 Project Structure
text
RaceDay.API/
├── Controllers/           API endpoints (Auth, Events, Categories, Weather, Participants, Results, Organisers)
├── Models/                EF Core entities (Organiser, Participant, Event, Category, Weather, Result)
├── Data/                  DbContext (RaceDayDbContext)
├── Helpers/               PasswordHasher utility
├── Migrations/            EF Core migrations
├── Program.cs             Application startup and service registration
└── appsettings.json       Configuration and connection string

RaceDay.Tests/
├── AuthControllerTests.cs
├── EventsControllerTests.cs
├── ResultsControllerTests.cs
└── TestSession.cs         In-memory session helper for tests
🛠️ Technologies Used
C# 12 / .NET 8

ASP.NET Core Web API

Entity Framework Core 8 (Code-First)

SQL Server (LocalDB / Express)

xUnit for unit testing

Moq for mocking

GitHub Actions for CI/CD

📄 License
This project is submitted as part of the PROG6212 Portfolio of Evidence at The Independent Institute of Education. Not for commercial use.

✍️ Author
Lufuno Ramasuvha — ST10471198 — Group 3
