# Используем базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Используем образ SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файл решения и файлы проектов
COPY ["TestWorkForModsen.sln", "."]
COPY ["docker-compose.dcproj", "."]
COPY ["TestWorkForModsen/TestWorkForModsen.csproj", "TestWorkForModsen/"]
COPY ["TestWorkForModsen.Data/TestWorkForModsen.Data.csproj", "TestWorkForModsen.Data/"]
COPY ["TestWorkForModsen.Services/TestWorkForModsen.Services.csproj", "TestWorkForModsen.Services/"]
COPY ["Test_TestWorkForModsen/Test_TestWorkForModsen.csproj", "Test_TestWorkForModsen/"]
# Восстанавливаем зависимости, игнорируя отсутствующие проекты (тесты и docker-compose)
RUN dotnet restore "TestWorkForModsen.sln" 

# Создаем директорию для сертификата
RUN mkdir -p /https

# Устанавливаем и экспортируем сертификат разработчика для HTTPS
RUN dotnet dev-certs https -v -ep /https/aspnetapp.pfx -p YourPassword123

# Копируем остальные файлы
COPY . .

# Собираем основной проект
WORKDIR "/src/TestWorkForModsen"
RUN dotnet build "./TestWorkForModsen.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикуем проект
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TestWorkForModsen.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app

# Копируем сертификат
COPY --from=build /https/aspnetapp.pfx /https/aspnetapp.pfx

# Копируем опубликованные файлы
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestWorkForModsen.dll"]