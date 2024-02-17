FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5251

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["lab-1/Ais.ConcurrentQueue.Api/Ais.ConcurrentQueue.Api.csproj", "lab-1/Ais.ConcurrentQueue.Api/"]
COPY ["lab-1/Ais.ConcurrentQueue/Ais.ConcurrentQueue.csproj", "lab-1/Ais.ConcurrentQueue/"]
RUN dotnet restore "lab-1/Ais.ConcurrentQueue.Api/Ais.ConcurrentQueue.Api.csproj"
COPY . .
WORKDIR "/src/lab-1/Ais.ConcurrentQueue.Api"
RUN dotnet build "Ais.ConcurrentQueue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ais.ConcurrentQueue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ais.ConcurrentQueue.Api.dll"]
