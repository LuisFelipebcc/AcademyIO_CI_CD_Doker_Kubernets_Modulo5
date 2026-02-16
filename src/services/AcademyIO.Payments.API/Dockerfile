# =========================
# Stage 1 - Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os arquivos de projeto necessários (Payments.API + suas dependências)
# Decoupled from Courses, Auth, Students, Bff e Tests
COPY ["src/services/AcademyIO.Payments.API/AcademyIO.Payments.API.csproj", "src/services/AcademyIO.Payments.API/"]
COPY ["src/building-blocks/AcademyIO.Core/AcademyIO.Core.csproj", "src/building-blocks/AcademyIO.Core/"]
COPY ["src/building-blocks/AcademyIO.WebAPI.Core/AcademyIO.WebAPI.Core.csproj", "src/building-blocks/AcademyIO.WebAPI.Core/"]
COPY ["src/building-blocks/AcademyIO.MessageBus/AcademyIO.MessageBus.csproj", "src/building-blocks/AcademyIO.MessageBus/"]

# Restore apenas do projeto do serviço
RUN dotnet restore "src/services/AcademyIO.Payments.API/AcademyIO.Payments.API.csproj"

# Copia o restante do código
COPY . .

WORKDIR "/src/src/services/AcademyIO.Payments.API"

# Publica aplicação
RUN dotnet publish "AcademyIO.Payments.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# =========================
# Stage 2 - Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AcademyIO.Payments.API.dll"]
