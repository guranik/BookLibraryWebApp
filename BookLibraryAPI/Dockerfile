# BookLibraryAPI/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BookLibraryAPI/BookLibraryAPI.csproj", "BookLibraryAPI/"]
RUN dotnet restore "BookLibraryAPI/BookLibraryAPI.csproj"
COPY . .
WORKDIR "/src/BookLibraryAPI"
RUN dotnet build "BookLibraryAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookLibraryAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Создание директории для загрузок изображений
RUN mkdir -p /app/Uploads

# Установка прав доступа, если необходимо
RUN chmod -R 777 /app/Uploads

ENTRYPOINT ["dotnet", "BookLibraryAPI.dll"]