#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Sample.Console/Sample.Console.csproj", "src/Sample.Console/"]
COPY ["src/EFCore.MongoDB/EFCore.MongoDB.csproj", "src/EFCore.MongoDB/"]
RUN dotnet restore "src/Sample.Console/Sample.Console.csproj"
COPY . .
WORKDIR "/src/src/Sample.Console"
RUN dotnet build "Sample.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Console.dll"]