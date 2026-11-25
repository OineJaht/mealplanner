# MealPlanner

Web application for planning meals: recipes, meal plans, a pantry, shopping lists and ingredient-based recipe search.
Built as a TalTech course project (2025).

## Stack

- **Backend:** C#, ASP.NET Core, EF Core, PostgreSQL
- **API:** versioned REST API documented with Swagger, JWT authentication with refresh tokens, role-based access (Admin, Moderator, Premium)
- **Structure:** layered architecture (DAL / BLL), DTOs, AutoMapper, repository and unit-of-work pattern
- **Tests:** xUnit and Moq (`MealPlanner.Tests`)
- **Frontends:** two TypeScript clients on the same API, Vue 3 and React
- **Deployment:** Docker Compose, deployed to Azure

## Run locally

Requires Docker.

```sh
cp .env.example .env     # then edit the values
docker compose up --build
```

| Service        | URL                       |
|----------------|---------------------------|
| API            | http://localhost:5001     |
| Vue frontend   | http://localhost:3000     |
| React frontend | http://localhost:3001     |

The admin user `admin@mealplanner.com` is created on startup only when `ADMIN_PASSWORD` is set.

## Configuration

All secrets come from environment variables (see `.env.example`); nothing sensitive is stored in the repository.
