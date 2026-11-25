# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MealPlanner/MealPlanner.csproj", "MealPlanner/"]
RUN dotnet restore "MealPlanner/MealPlanner.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/MealPlanner"
RUN dotnet build "MealPlanner.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MealPlanner.csproj" -c Release -o /app/publish

# Use the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# Remove health check (curl not available)
# Remove user switching - let Azure handle permissions

# Run the application
ENTRYPOINT ["dotnet", "MealPlanner.dll"]