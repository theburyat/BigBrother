FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BigBrotherApi/BigBrotherApi.csproj", "BigBrotherApi/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Utils/Utils.csproj", "Utils/"]
RUN dotnet restore "BigBrotherApi/BigBrotherApi.csproj"
COPY . .
WORKDIR "/src/BigBrotherApi"
RUN dotnet build "BigBrotherApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BigBrotherApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BigBrotherApi.dll"]
