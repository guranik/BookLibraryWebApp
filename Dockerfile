# Базовый образ для ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проектов
COPY ["BookLibraryAPI/BookLibraryAPI.csproj", "BookLibraryAPI/"]
COPY ["BookLibraryClient/BookLibraryClient.csproj", "BookLibraryClient/"]
COPY ["BookLibraryWebAPITests/BookLibraryWebAPITests.csproj", "BookLibraryWebAPITests/"]
COPY ["BookLibraryDataAccessClassLibrary/BookLibraryDataAccessClassLibrary.csproj", "BookLibraryDataAccessClassLibrary/"]
COPY ["BookLibraryBusinessLogicClassLibrary/BookLibraryBusinessLogicClassLibrary.csproj", "BookLibraryBusinessLogicClassLibrary/"]

# Восстанавливаем зависимости
RUN dotnet restore "BookLibraryAPI/BookLibraryAPI.csproj" && \
    dotnet restore "BookLibraryClient/BookLibraryClient.csproj" && \
    dotnet restore "BookLibraryWebAPITests/BookLibraryWebAPITests.csproj" && \
    dotnet restore "BookLibraryDataAccessClassLibrary/BookLibraryDataAccessClassLibrary.csproj" && \
    dotnet restore "BookLibraryBusinessLogicClassLibrary/BookLibraryBusinessLogicClassLibrary.csproj"

# Копируем все остальные файлы
COPY . .

# Сборка и публикация API
WORKDIR "/src/BookLibraryAPI"
RUN dotnet publish "BookLibraryAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Сборка и публикация клиента
WORKDIR "/src/BookLibraryClient"
RUN dotnet publish "BookLibraryClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап для API
FROM base AS final-api
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookLibraryAPI.dll"]

# Финальный этап для клиента
FROM base AS final-client
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookLibraryClient.dll"]