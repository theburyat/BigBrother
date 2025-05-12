FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BigBrother.WebApp/BigBrother.WebApp.csproj", "BigBrother.WebApp/"]
COPY ["BigBrother.Domain/BigBrother.Domain.csproj", "BigBrother.Domain/"]
COPY ["BigBrother.Repository/BigBrother.Repository.csproj", "BigBrother.Repository/"]
RUN dotnet restore "BigBrother.WebApp/BigBrother.WebApp.csproj"
COPY . .
WORKDIR "/src/BigBrother.WebApp"
RUN dotnet build "./BigBrother.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BigBrother.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BigBrother.WebApp.dll"]
