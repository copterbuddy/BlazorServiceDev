#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorAppDev/Server/Service.csproj", "BlazorAppDev/Server/"]
COPY ["BlazorAppDev/Client/BlazorAppDev.Client.csproj", "BlazorAppDev/Client/"]
COPY ["BlazorAppDev/Shared/BlazorAppDev.Shared.csproj", "BlazorAppDev/Shared/"]
RUN dotnet restore "BlazorAppDev/Server/Service.csproj"
COPY . .
WORKDIR "/src/BlazorAppDev/Server"
RUN dotnet build "Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]