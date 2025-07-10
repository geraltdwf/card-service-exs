FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV DOTNET_URLS=http://+:8017
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /.
COPY ["Card.Service/Card.Service.csproj", "Card.Service/"]
RUN dotnet restore "Card.Service/Card.Service.csproj"
COPY . .
WORKDIR "/Card.Service"
RUN dotnet build "Card.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Card.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Card.Service.dll"]
