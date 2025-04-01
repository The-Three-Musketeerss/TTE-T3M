# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore projects
COPY TTE.API/TTE.API.csproj TTE.API/
COPY TTE.Application/TTE.Application.csproj TTE.Application/
COPY TTE.Infrastructure/TTE.Infrastructure.csproj TTE.Infrastructure/
COPY TTE.Test/TTE.Tests.csproj TTE.Test/
RUN dotnet restore TTE.API/TTE.API.csproj

# Copy remaining files and build
COPY . .
WORKDIR /src/TTE.API
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TTE.API.dll"]
