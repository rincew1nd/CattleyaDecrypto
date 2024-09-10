FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN apt-get update && apt-get install -y \
    software-properties-common \
    npm
RUN npm install npm@latest -g && \
    npm install n -g && \
    n latest
	
COPY ["CattleyaDecrypto.Server/CattleyaDecrypto.Server.csproj", "CattleyaDecrypto.Server/"]
RUN dotnet restore "./CattleyaDecrypto.Server/CattleyaDecrypto.Server.csproj"
COPY . .
WORKDIR "/src/CattleyaDecrypto.Server"
RUN dotnet build "./CattleyaDecrypto.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CattleyaDecrypto.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CattleyaDecrypto.Server.dll"]