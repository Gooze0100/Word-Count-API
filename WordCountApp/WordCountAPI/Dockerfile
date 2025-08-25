FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5103
EXPOSE 5104

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WordCountAPI/WordCountAPI.csproj", "WordCountAPI/"]
RUN dotnet restore "WordCountAPI/WordCountAPI.csproj"
COPY . .
WORKDIR "/src/WordCountAPI"
RUN dotnet build "./WordCountAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS api-publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WordCountAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS api
WORKDIR /app
COPY --from=api-publish /app/publish .
ENTRYPOINT ["dotnet", "WordCountAPI.dll"]
