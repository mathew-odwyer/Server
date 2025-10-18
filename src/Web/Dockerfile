FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY . .

RUN dotnet publish Web.Presentation/Web.Presentation.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Web.Presentation.dll"]
