#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["YskConsole/YskConsole.csproj", "YskConsole/"]
RUN dotnet restore "YskConsole/YskConsole.csproj"
COPY . .
WORKDIR "/src/YskConsole"
RUN dotnet build "YskConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YskConsole.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YskConsole.dll"]