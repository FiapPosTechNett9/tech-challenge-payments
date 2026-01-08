FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FIAP.CloudGames.Users.sln ./
COPY src/FIAP.CloudGames.Users.API/FIAP.CloudGames.Users.API.csproj src/FIAP.CloudGames.Users.API/
COPY src/FIAP.CloudGames.Users.Application/FIAP.CloudGames.Users.Application.csproj src/FIAP.CloudGames.Users.Application/
COPY src/FIAP.CloudGames.Users.Domain/FIAP.CloudGames.Users.Domain.csproj src/FIAP.CloudGames.Users.Domain/
COPY src/FIAP.CloudGames.Users.Infrastructure/FIAP.CloudGames.Users.Infrastructure.csproj src/FIAP.CloudGames.Users.Infrastructure/
COPY . .

RUN dotnet restore src/FIAP.CloudGames.Users.API/FIAP.CloudGames.Users.API.csproj

RUN dotnet publish src/FIAP.CloudGames.Users.API/FIAP.CloudGames.Users.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FIAP.CloudGames.Users.API.dll"]