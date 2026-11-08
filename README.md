# RaceDay - PROG6212 Portfolio of Evidence (Part 3)

![.NET 10](https://img.shields.io/badge/.NET-10.0-blue)
[![RaceDay CI/CD](https://github.com/ST10471198/RaceDay-API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ST10471198/RaceDay-API/actions/workflows/dotnet.yml)

RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. It allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and manage their profiles.

## 📋 Project Overview

This is the final submission (**Part 3**) of the PROG6212 Portfolio of Evidence. It builds upon Part 1 (Database Planning) and Part 2 (RESTful API) by adding a complete **ASP.NET Core MVC** web frontend and integrating **Azure Blob Storage** for cloud-based image handling.

### 👥 User Roles
The system supports two distinct roles with tailored interfaces and permissions:

*   **Organiser:** Can create, edit, and delete events; upload event banner images to Azure; manage event categories; view all participant enrolments; and capture/publish race results.
*   **Participant:** Can create an account, browse upcoming events, enrol in events by selecting a category, view their registered events, track their personal race results (finish times and positions), and update their profile picture.

## 🚀 Setup Instructions

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   SQL Server (LocalDB or Express) or SQL Server Management Studio (SSMS)
*   Visual Studio 2022 (with the **ASP.NET and web development** workload)
*   An Azure Storage Account (for Blob Storage integration)

### Steps to Run Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ST10471198/RaceDay-API.git
   cd RaceDay-API


2. Set up the Database:
   Open the SQL script provided in the /docs folder (or your root folder).
   Run the script in SQL Server Management Studio (SSMS) to create the RaceDayDB database, tables, and seed data.

3. Configure the API:
	Open the RaceDay.API project.
	In appsettings.Development.json, add your SQL Server connection string and your Azure Storage connection string:
					
	{
         "ConnectionStrings": {
         "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RaceDayDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
         "AzureStorage": {
         "ConnectionString": "YOUR_AZURE_CONNECTION_STRING_HERE",
         "ContainerName": "raceday-images"
        }
    }

    Run the API. Swagger UI will open at https://localhost:7012/swagger


4. Run the MVC Application:
    Set RaceDay.MVC as the startup project.
    Note: Ensure the API is running in the background.
    Run the MVC app. It will open in your browser at https://localhost:7176.


🔄 CI/CD Pipeline
This project uses GitHub Actions for continuous integration. The workflow file is located at .github/workflows/dotnet.yml.
On every push to the master or main branch, GitHub Actions will:
Setup .NET 10
Restore dependencies for the API, MVC, and Test projects.
Build all projects in Release mode.
Run all 8 unit tests to ensure API integrity.

Successful CI Build:
Below is a screenshot of a successful green build with all tests passing:
https://docs/ci-screenshot.png

🎥 Video Presentation
A full walkthrough of the running application, explaining the MVC structure, Blob Storage integration, and the code architecture can be found below:

👉 Watch the Part 3 Video Presentation on YouTube: Add the link here


🛠️ Technologies Used
C# 12 / .NET 10
ASP.NET Core Web API & MVC
Entity Framework Core (Code-First / Database-First)
SQL Server (LocalDB)
Azure Blob Storage for image uploads (Event banners & Profile pictures)
Bootstrap 5 & Bootstrap Icons for UI design
xUnit & Moq for unit testing
GitHub Actions for CI/CD

📁 Project Structure
RaceDay-API/
├── RaceDay.API/           # The Backend API
│   ├── Controllers/       # Auth, Events, Categories, Enrolments, Participants
│   ├── Models/            # EF Core entities
│   ├── DTOs/              # Data Transfer Objects for JSON payloads
│   ├── Services/          # BlobStorageService for Azure
│   └── Program.cs         # API startup
│
├── RaceDay.MVC/           # The Frontend Web Application
│   ├── Controllers/       # OrganiserController, ParticipantController, AuthController
│   ├── Models/            # ViewModels for the UI
│   ├── Views/             # Razor Views (Dashboard, Events, Profile, etc.)
│   ├── Services/          # ApiService (HTTP client wrapper)
│   └── wwwroot/           # CSS, JS, Logo, and Images
│
├── RaceDay.Tests/         # Unit Tests for the API
│   ├── AuthControllerTests.cs
│   ├── EventsControllerTests.cs
│   └── ResultsControllerTests.cs
│
└── .github/workflows/     # CI/CD Pipeline (dotnet.yml)

📄 License
This project is submitted as part of the PROG6212 Portfolio of Evidence at The Independent Institute of Education. Not for commercial use.

✍️ Author
Lufuno Ramasuvha — ST10471198 — Group 3