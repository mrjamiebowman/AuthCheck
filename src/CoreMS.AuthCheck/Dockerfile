# base
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=8081
ENV ASPNETCORE_URLS "http://+:8080"

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CoreMS.AuthCheck/CoreMS.AuthCheck.csproj", "src/CoreMS.AuthCheck/"]
RUN dotnet restore "./src/CoreMS.AuthCheck/CoreMS.AuthCheck.csproj"
COPY . .
WORKDIR "/src/src/CoreMS.AuthCheck"
RUN dotnet build "./CoreMS.AuthCheck.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CoreMS.AuthCheck.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreMS.AuthCheck.dll"]