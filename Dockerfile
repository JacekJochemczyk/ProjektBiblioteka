# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# kopiujemy pliki projektów osobno, żeby cache działał
COPY *.sln ./
COPY Biblioteka/Biblioteka.csproj Biblioteka/
COPY Biblioteka.Domain/Biblioteka.Domain.csproj Biblioteka.Domain/
COPY Biblioteka.Infrastructure/Biblioteka.Infrastructure.csproj Biblioteka.Infrastructure/

RUN dotnet restore

# kopiujemy resztę kodu
COPY . .

# publish głównego projektu
RUN dotnet publish Biblioteka/Biblioteka.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/App_Data \
    && chown -R app:app /app

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

USER app

EXPOSE 8080
ENTRYPOINT ["dotnet", "Biblioteka.dll"]
