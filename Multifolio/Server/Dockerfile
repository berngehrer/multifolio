#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Multifolio/Server/Multifolio.Server.csproj", "Multifolio/Server/"]
COPY ["Multifolio/Shared/Multifolio.Shared.csproj", "Multifolio/Shared/"]
COPY ["Multifolio/Client/Multifolio.Client.csproj", "Multifolio/Client/"]
RUN dotnet restore "Multifolio/Server/Multifolio.Server.csproj"
COPY . .
WORKDIR "/src/Multifolio/Server"
RUN dotnet build "Multifolio.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Multifolio.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Multifolio.Server.dll"]