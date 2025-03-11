FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY prueba/prueba.csproj prueba/
RUN dotnet restore prueba/prueba.csproj

COPY prueba/ prueba/
WORKDIR /src/prueba

RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "prueba.dll"]

