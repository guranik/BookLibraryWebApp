# BookLibraryClient/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BookLibraryClient/BookLibraryClient.csproj", "BookLibraryClient/"]
RUN dotnet restore "BookLibraryClient/BookLibraryClient.csproj"
COPY . .
WORKDIR "/src/BookLibraryClient"
RUN dotnet build "BookLibraryClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookLibraryClient.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BookLibraryClient.dll"]