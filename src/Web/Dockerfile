FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

COPY . .

RUN dotnet publish Winterhaven.Presentation/Winterhaven.Presentation.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

# Install ICU libraries for globalization support
# This is because azure SQL edge ships with globalization-invariant enabled, meaning only invariant culture is supported.
RUN apk add --no-cache curl icu icu-dev

WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080

# Required because we are using azure SQL edge which has globalization invariant enabled.
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Winterhaven.Presentation.dll"]
