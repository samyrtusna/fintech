# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy the entire repository
COPY . .

# Restore dependencies
RUN dotnet restore "fintech.API.csproj"

# Publish the application
RUN dotnet publish "fintech.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Render will provide the PORT environment variable.

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=10000

ENTRYPOINT ["dotnet", "fintech.API.dll"]