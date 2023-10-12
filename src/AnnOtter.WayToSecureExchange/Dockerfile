#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /src
COPY ["AnnOtter.WayToSecureExchange.csproj", "."]
RUN dotnet restore "./AnnOtter.WayToSecureExchange.csproj" --use-current-runtime
COPY . .
WORKDIR "/src/."
RUN dotnet build "AnnOtter.WayToSecureExchange.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnnOtter.WayToSecureExchange.csproj" -c Release -o /app/publish /p:UseAppHost=false --use-current-runtime --self-contained false

FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0-jammy-chiseled AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnnOtter.WayToSecureExchange.dll"]