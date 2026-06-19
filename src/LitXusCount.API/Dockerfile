# Build context is the repository root (see .do/app.yaml: source_dir "/")
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LitXusCount.slnx ./
COPY src/LitXusCount.Domain/LitXusCount.Domain.csproj src/LitXusCount.Domain/
COPY src/LitXusCount.Application/LitXusCount.Application.csproj src/LitXusCount.Application/
COPY src/LitXusCount.Infrastructure/LitXusCount.Infrastructure.csproj src/LitXusCount.Infrastructure/
COPY src/LitXusCount.API/LitXusCount.API.csproj src/LitXusCount.API/
RUN dotnet restore src/LitXusCount.API/LitXusCount.API.csproj

COPY src/ src/
RUN dotnet publish src/LitXusCount.API/LitXusCount.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LitXusCount.API.dll"]
