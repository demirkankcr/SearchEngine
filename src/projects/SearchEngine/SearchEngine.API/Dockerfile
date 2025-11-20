# src/projects/SearchEngine/SearchEngine.API/Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies
# Copy csproj files specifically to allow better caching
COPY ["src/corepackages/Core.Domain/Core.Domain.csproj", "src/corepackages/Core.Domain/"]
COPY ["src/corepackages/Core.Persistence/Core.Persistence.csproj", "src/corepackages/Core.Persistence/"]
COPY ["src/corepackages/Core.Application/Core.Application.csproj", "src/corepackages/Core.Application/"]
COPY ["src/corepackages/Core.CrossCuttingConcerns/Core.CrossCuttingConcerns.csproj", "src/corepackages/Core.CrossCuttingConcerns/"]
COPY ["src/projects/SearchEngine/SearchEngine.Application/SearchEngine.Application.csproj", "src/projects/SearchEngine/SearchEngine.Application/"]
COPY ["src/projects/SearchEngine/SearchEngine.Infrastructure/SearchEngine.Infrastructure.csproj", "src/projects/SearchEngine/SearchEngine.Infrastructure/"]
COPY ["src/projects/SearchEngine/SearchEngine.API/SearchEngine.API.csproj", "src/projects/SearchEngine/SearchEngine.API/"]

RUN dotnet restore "src/projects/SearchEngine/SearchEngine.API/SearchEngine.API.csproj"

# Copy the entire source code
COPY . .

# Build the project
WORKDIR "/src/src/projects/SearchEngine/SearchEngine.API"
RUN dotnet build "SearchEngine.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SearchEngine.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchEngine.API.dll"]

