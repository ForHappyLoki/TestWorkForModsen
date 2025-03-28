# Используем базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Используем образ SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN mkdir -p /https && \
    dotnet dev-certs https -v -ep /https/aspnetapp.pfx -p YourPassword123 && \
    chmod 644 /https/aspnetapp.pfx  

# Копируем файлы проектов
COPY ["TestWorkForModsen.sln", "."]
COPY ["docker-compose.dcproj", "."]
COPY ["TestWorkForModsen/TestWorkForModsen.csproj", "TestWorkForModsen/"]
COPY ["TestWorkForModsen.Data/TestWorkForModsen.Data.csproj", "TestWorkForModsen.Data/"]
COPY ["TestWorkForModsen.Services/TestWorkForModsen.Services.csproj", "TestWorkForModsen.Services/"]
COPY ["Test_TestWorkForModsen/Test_TestWorkForModsen.csproj", "Test_TestWorkForModsen/"]

# Восстанавливаем зависимости
RUN dotnet restore "TestWorkForModsen.sln"

# Генерируем и экспортируем сертификат разработчика
RUN dotnet dev-certs https -v -ep /https/aspnetapp.pfx -p YourPassword123 \
    && chmod 644 /https/aspnetapp.pfx

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

RUN mkdir -p /https
COPY --from=build /https/aspnetapp.pfx /https/aspnetapp.pfx
RUN chmod 644 /https/aspnetapp.pfx

RUN ls -la /https/aspnetapp.pfx || echo "Файл сертификата не найден!"

ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword123
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080

# Копируем опубликованные файлы
COPY --from=publish /app/publish .

# Устанавливаем переменные окружения для Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword123
ENV ASPNETCORE_URLS=https://+:8081;http://+:8080

ENTRYPOINT ["dotnet", "TestWorkForModsen.dll"]