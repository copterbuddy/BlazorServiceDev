#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "Service/Service.csproj"
COPY . .
WORKDIR "/src/Service"
RUN dotnet build "Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://+:80
#ENV ASPNETCORE_URLS=https://+:443;http://+:80
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]