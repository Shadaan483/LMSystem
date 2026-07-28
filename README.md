# 📖LMSystem — Library Management System

A full-stack ASP.NET Core MVC application for managing a library's books, periodicals, borrowers, and staff — with real authentication, role-based access control, and a custom visual theme.

## Tech Stack

- **ASP.NET Core MVC** (.NET 8)
- **Entity Framework Core** (SQL Server) — Books, Borrow Records, Publications
- **Raw ADO.NET** (`SqlConnection` / `SqlCommand`) — Students, Librarians, Dashboard
- **ASP.NET Core Identity** — Authentication, roles (Administrator / Librarian / Member)
- **xUnit + EF Core InMemory** — automated tests (`LMSystem.Tests`)
- **SQL Server** (Express or full edition)

## Features

- **Book Catalog** — full CRUD, search by title/author/ISBN, pagination, cover art via Open Library
- **Borrowing System** — borrow/return workflow with guard views for edge cases
- **Publications** — separate Newspaper / Magazine catalog with its own CRUD
- **Student & Librarian Management** — built directly on ADO.NET with parameterized queries
- **Dashboard** — live summary stats (books, students, librarians, active borrows)
- **Authentication & Roles** — Register, Login, Forgot/Reset Password, Change Password, Access Denied, role-based `[Authorize]`
- **Custom UI theme** — navy / brass / burgundy "reading room" design

## Project Structure

```
LMSystem/
├── LMSystem.sln
├── LMSystem/                      # Main web app
│   ├── Controllers/                # Books, Borrow, Publications, Students, Librarians, Dashboard, Account, Login, Home
│   ├── Models/                     # EF Core entities + ADO.NET models
│   ├── ViewModels/                 # Register/Login/Borrow/etc. view models
│   ├── Views/                      # Razor views, grouped by controller
│   ├── Data/                       # ApplicationDbContext, SeedData
│   ├── wwwroot/                    # CSS, images, static assets
│   ├── Program.cs
│   ├── appsettings.json
│   └── setup_day3_day4.sql         # Manual SQL for the raw ADO.NET tables
└── LMSystem.Tests/                 # Unit tests (EF Core InMemory)
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express edition works fine) + SQL Server Management Studio (optional, for running the setup script)

### Setup
1. Clone the repo:
   ```bash
   git clone https://github.com/<your-username>/<repo-name>.git
   cd <repo-name>
   ```
2. Update the connection string in `LMSystem/appsettings.json` to match your SQL Server instance, e.g.:
   ```json
   "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=LMS;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
   ```
3. Apply EF Core migrations:
   ```bash
   dotnet ef database update --context LibraryContext --project LMSystem
   dotnet ef database update --context ApplicationDbContext --project LMSystem
   ```
4. Run `setup_day3_day4.sql` in SQL Server Management Studio against the `LMS` database to create the Students/Librarians tables.
5. Run the app:
   ```bash
   cd LMSystem
   dotnet run
   ```
6. Open the URL shown in the terminal (e.g. `https://localhost:5001`).

### Seeded Demo Accounts

| Username | Password | Role |
|---|---|---|
| `admin` | `12345` | Administrator |
| `mycodingproject` | `myc546` | Librarian |
| `my` | `myc` | Member |

## Running Tests

```bash
dotnet test
```

## License

This project was built as part of MP Online Internship project.
