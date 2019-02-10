FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["GameBattleShip/GameBattleShip.csproj", "GameBattleShip/"]
RUN dotnet restore "GameBattleShip/GameBattleShip.csproj"
COPY . .
WORKDIR "/src/GameBattleShip"
RUN dotnet build "GameBattleShip.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GameBattleShip.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GameBattleShip.dll"]