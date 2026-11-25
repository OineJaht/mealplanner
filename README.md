# MealPlanner

Web application for planning meals: recipes, meal plans, a pantry, shopping lists and ingredient-based recipe search. Built as a TalTech course project (2025).

## Stack

- **Backend:** C#, ASP.NET Core 8, EF Core, PostgreSQL
- **API:** versioned REST API documented with Swagger, JWT authentication with refresh tokens, role-based access (Admin, Moderator, Premium)
- **Structure:** layered architecture (DAL / BLL), DTOs, AutoMapper, repository and unit-of-work pattern
- **Tests:** xUnit and Moq (`MealPlanner.Tests`)
- **Frontends:** two TypeScript clients on the same API: Vue 3 (`meal-planner-client`) and React (`React`)
- **Deployment:** Docker Compose; the project was deployed to Azure during the course

The server-rendered MVC views in the backend are an earlier iteration; the Vue and React clients are the primary UI.

## Run with Docker

Requires Docker.

```bash
cp .env.example .env     # PowerShell: Copy-Item .env.example .env; then edit the values
docker compose up --build
```

| Service | URL |
|---|---|
| API (Swagger at `/swagger`) | http://localhost:5001 |
| Vue frontend | http://localhost:3000 |
| React frontend | http://localhost:3001 |

## Run without Docker

Requires .NET 8 SDK, PostgreSQL and Node.js 18+.

Create `MealPlanner/.env` (it is git-ignored):

```
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=mealplanner;Username=postgres;Password=<your-password>
JwtSettings__SecretKey=<random string, at least 32 characters>
ADMIN_PASSWORD=<optional, creates admin@mealplanner.com>
```

Then:

```bash
# Backend (the database is created and migrated on startup)
cd MealPlanner
dotnet run                          # http://localhost:5001/swagger

# Vue client (in another terminal)
cd meal-planner-client
cp .env.example .env                # PowerShell: Copy-Item .env.example .env
npm ci && npm run dev               # http://localhost:3000
```

Register a user via `POST /api/Account/register` (or the client's sign-up page) and log in via `POST /api/Account/login`. The admin user is created on startup only when `ADMIN_PASSWORD` is set; it must satisfy the ASP.NET Identity password rules.

## Tests

```bash
dotnet test MealPlanner.Tests
```

13 tests are currently skipped: they were written against an earlier service implementation and need rework (the EF Core in-memory provider does not support transactions).

## Configuration

All secrets come from environment variables (see `.env.example`); nothing sensitive is stored in the repository.
