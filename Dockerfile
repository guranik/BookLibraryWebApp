# Базовый образ для ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src
COPY ["BookLibraryAPI/BookLibraryAPI.csproj", "BookLibraryAPI/"]
RUN dotnet restore "BookLibraryAPI/BookLibraryAPI.csproj"
COPY . .
WORKDIR "/src/BookLibraryAPI"
RUN dotnet build "BookLibraryAPI.csproj" -c Release -o /app/build

# Этап публикации API
FROM build-api AS publish-api
RUN dotnet publish "BookLibraryAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап сборки клиента
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client
WORKDIR /src
COPY ["BookLibraryClient/BookLibraryClient.csproj", "BookLibraryClient/"]
RUN dotnet restore "BookLibraryClient/BookLibraryClient.csproj"
COPY . .
WORKDIR "/src/BookLibraryClient"
RUN dotnet build "BookLibraryClient.csproj" -c Release -o /app/build

# Этап публикации клиента
FROM build-client AS publish-client
RUN dotnet publish "BookLibraryClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап сборки тестов
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-tests
WORKDIR /src
COPY ["BookLibraryWebAPITests/BookLibraryWebAPITests.csproj", "BookLibraryWebAPITests/"]
RUN dotnet restore "BookLibraryWebAPITests/BookLibraryWebAPITests.csproj"
COPY . .
WORKDIR "/src/BookLibraryWebAPITests"
RUN dotnet build "BookLibraryWebAPITests.csproj" -c Release -o /app/build

# Этап выполнения тестов
FROM build-tests AS run-tests
WORKDIR /src/BookLibraryWebAPITests
CMD ["dotnet", "test", "--no-restore", "--verbosity=normal"]

# Финальный этап для API
FROM base AS final-api
WORKDIR /app
COPY --from=publish-api /app/publish .
ENTRYPOINT ["dotnet", "BookLibraryAPI.dll"]

# Финальный этап для клиента
FROM base AS final-client
WORKDIR /app
COPY --from=publish-client /app/publish .
ENTRYPOINT ["dotnet", "BookLibraryClient.dll"]