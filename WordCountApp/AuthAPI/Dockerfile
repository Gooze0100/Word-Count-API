FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5081
EXPOSE 5082

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthAPI/AuthAPI.csproj", "AuthAPI/"]
RUN dotnet restore "AuthAPI/AuthAPI.csproj"
COPY . .
WORKDIR "/src/AuthAPI"
RUN dotnet build "./AuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS auth-publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS auth
WORKDIR /app
COPY --from=auth-publish /app/publish .
ENTRYPOINT ["dotnet", "AuthAPI.dll"]
