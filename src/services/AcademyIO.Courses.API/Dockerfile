# Stage 1: Build - Compiles and publishes the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all project files to leverage layer caching.
# This restore step will only be re-run if project files change.
COPY ["AcademyIO.sln", "."]
COPY ["src/services/AcademyIO.Auth.API/AcademyIO.Auth.API.csproj", "src/services/AcademyIO.Auth.API/"]
COPY ["src/services/AcademyIO.Courses.API/AcademyIO.Courses.API.csproj", "src/services/AcademyIO.Courses.API/"]
COPY ["src/services/AcademyIO.Students.API/AcademyIO.Students.API.csproj", "src/services/AcademyIO.Students.API/"]
COPY ["src/services/AcademyIO.Payments.API/AcademyIO.Payments.API.csproj", "src/services/AcademyIO.Payments.API/"]
COPY ["src/api-gateways/AcademyIO.Bff/AcademyIO.Bff.csproj", "src/api-gateways/AcademyIO.Bff/"]
COPY ["src/building-blocks/AcademyIO.Core/AcademyIO.Core.csproj", "src/building-blocks/AcademyIO.Core/"]
COPY ["src/building-blocks/AcademyIO.MessageBus/AcademyIO.MessageBus.csproj", "src/building-blocks/AcademyIO.MessageBus/"]
COPY ["src/building-blocks/AcademyIO.WebAPI.Core/AcademyIO.WebAPI.Core.csproj", "src/building-blocks/AcademyIO.WebAPI.Core/"]

# Restore dependencies for the entire solution
RUN dotnet restore "AcademyIO.sln"

# Copy the rest of the source code. .dockerignore will handle exclusions.
COPY . .

# Publish the application.
WORKDIR "/src/src/services/AcademyIO.Courses.API"
RUN dotnet publish "AcademyIO.Courses.API.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 2: Final - Creates the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Copy the published artifacts from the build stage
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AcademyIO.Courses.API.dll"]