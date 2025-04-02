# 🛒 Tech Trend Emporium - Backend

Backend RESTful API for Tech Trend Emporium, built with **ASP.NET Core**, using **Entity Framework Core**, **MySQL**, and secured with **JWT authentication**. 
📄 [Project Requirements](https://confluence.endava.com/spaces/DevDisc/pages/279422631/Tech+Trend+Emporium)

---


## 📦 Local Installation

Follow these steps to set up the project locally:

### 1. Clone the repository

```bash
git clone https://github.com/The-Three-Musketeerss/TTE-T3M.git
cd tech-trend-emporium 
```
### 1. Restore 
```bash
dotnet restore
```
---

## 🛠️ Tools & Libraries

| Tool / Library | Version | Description |
|----------------|---------|-------------|
| ASP.NET Core | 7.0 | Web framework used to build the RESTful backend APIs. |
| Entity Framework Core (Pomelo.MySQL) | 9.0.3 | ORM used for database access with MySQL. |
| AutoMapper | 14.0.0 | Maps DTOs to domain models and vice versa, reducing boilerplate. |
| JWT Authentication | 11.0.0 | Secures API endpoints using JSON Web Tokens. |
| Swashbuckle.AspNetCore (Swagger) | 6.6.2 | Generates interactive API documentation. |
| Docker | - | Containerizes the application for consistent deployment. |
| Bcrypt.Net | 4.0.3 | Hashes passwords securely before storing in DB. |
| xUnit | 2.9.3 | Unit testing framework for .NET. |
| Moq | 4.20.72 | Mocking library used to isolate unit tests. |
| Coverlet | 6.0.4 | Generates code coverage data from unit tests. |
| ReportGenerator | 5.4.5 | Converts coverage data into human-readable HTML reports. |
| AWS SDK | 3.7.402.31 | Access AWS services programmatically. |
| AWS Secrets Manager | 3.7.400.125 | Securely stores and retrieves secrets like DB credentials. |

---

## ✅ Unit Tests & Code Coverage

- All **API controllers** and **Service layer** have unit tests written using `xUnit` and `Moq`
- Coverage report generated using `Coverlet` + `ReportGenerator`
- **Coverage: ~80% line coverage** across API and Services

### Run tests and generate coverage:
```bash
cd TTE.Tests
dotnet test --collect:"XPlat Code Coverage"
```

### Generate HTML coverage report:
```bash
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
start coverage-report/index.html
```

---

## 🚀 How to Run the Application

### 1. Configure the connection string
Open `appsettings.json` in `TTE.API` and update:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;user=tte_user;password=upb123;database=tech_trend_emporium"
}
```

### 2. Run database migrations
```bash
dotnet ef migrations add InitialCreation --project TTE.Infrastructure --startup-project TTE.API
dotnet ef database update --project TTE.Infrastructure --startup-project TTE.API
```

### 3. Create a SuperAdmin manually in the DB
Insert a user into the `Users` table with a role of SuperAdmin (RoleId = 1).  
**Hash the password using Bcrypt** before inserting.

### 4. Build and run the project
```bash
dotnet build TTE.sln
dotnet run --project TTE.API
```

### 5. Open Swagger
```text
http://localhost:5006/swagger/index.html
```

---

## 🔐 Authentication

- Most endpoints require a valid **JWT token**
- Use `/api/auth/login` to obtain a token
- Use a **SuperAdmin** or **Employee** user depending on the required permission level

📘 Roles and permissions are documented in the [API Wiki](https://github.com/Medeteam/tech-trend-emporium)

---

## 📁 Project Structure

```
TTE.API             --> Controllers & Startup
TTE.Application     --> DTOs, Services, Interfaces
TTE.Infrastructure  --> EF Models, DbContext, Repositories
TTE.Tests           --> Unit tests (xUnit + Moq)
```

---

# Development Strategy
This project follows the trunk-based development strategy with the following rules:

- All contributions must be made via pull requests.
- Each pull request requires two approvals before being merged.
- Force pushes are not allowed on the main branch.
- After the merge, the branch is deleted.

## 📌 Notes

- This project is **backend only**
- When creating users manually in the DB, remember to hash passwords with Bcrypt

---

## Contribute
If you would like to contribute to this project:

- Frotk this repository.
- Create a new branch
```bash
    git checkout -b Feature/my-feature
```
- Commit you changes
```bash
    git commit -m "Add new feature"
```
- Push you Branch
```bash
    git push origin my-feature-branch
```
- Open a pull request on GitHub

---


