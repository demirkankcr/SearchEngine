# src/projects/SearchEngine/SearchEngine.UI/Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore dependencies
COPY ["src/corepackages/Core.Domain/Core.Domain.csproj", "src/corepackages/Core.Domain/"]
COPY ["src/projects/SearchEngine/SearchEngine.UI/SearchEngine.UI.csproj", "src/projects/SearchEngine/SearchEngine.UI/"]

RUN dotnet restore "src/projects/SearchEngine/SearchEngine.UI/SearchEngine.UI.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
WORKDIR "/src/src/projects/SearchEngine/SearchEngine.UI"
RUN dotnet build "SearchEngine.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SearchEngine.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchEngine.UI.dll"]

