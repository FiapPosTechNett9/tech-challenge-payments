FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FIAP.CloudGames.Payments.sln ./
COPY src/FIAP.CloudGames.Payments.API/FIAP.CloudGames.Payments.API.csproj src/FIAP.CloudGames.Payments.API/
COPY src/FIAP.CloudGames.Payments.Application/FIAP.CloudGames.Payments.Application.csproj src/FIAP.CloudGames.Payments.Application/
COPY src/FIAP.CloudGames.Payments.Domain/FIAP.CloudGames.Payments.Domain.csproj src/FIAP.CloudGames.Payments.Domain/
COPY src/FIAP.CloudGames.Payments.Infrastructure/FIAP.CloudGames.Payments.Infrastructure.csproj src/FIAP.CloudGames.Payments.Infrastructure/
COPY . .

RUN dotnet restore src/FIAP.CloudGames.Payments.API/FIAP.CloudGames.Payments.API.csproj

RUN dotnet publish src/FIAP.CloudGames.Payments.API/FIAP.CloudGames.Payments.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FIAP.CloudGames.Payments.API.dll"]