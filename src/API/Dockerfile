# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /app

# TODO: Remove this, hack fix for this issue: https://github.com/reactiveui/refit/issues/2114
# Stops NuGet from going online to check revocation status
ENV NUGET_CERT_REVOCATION_MODE=offline

# Copy shared properties first
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy only project files
COPY API/Winterhaven.API.Core.Domain/Winterhaven.API.Core.Domain.csproj \
    API/Winterhaven.API.Core.Domain/

COPY API/Winterhaven.API.Core.Application/Winterhaven.API.Core.Application.csproj \
    API/Winterhaven.API.Core.Application/

COPY API/Winterhaven.API.Infrastructure/Winterhaven.API.Infrastructure.csproj \
    API/Winterhaven.API.Infrastructure/

COPY API/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj \
    API/Winterhaven.API.Presentation/

COPY Common/Winterhaven.Common/Winterhaven.Common.csproj \
    Common/Winterhaven.Common/

COPY API/Winterhaven.API.Presentation/appsettings.json API/Winterhaven.API.Presentation/

RUN dotnet restore API/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj

COPY API/ API/
COPY Common/ Common/

RUN dotnet publish API/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final

# Install ICU libraries for globalization support
# This is because azure SQL edge ships with globalization-invariant enabled, meaning only invariant culture is supported.
RUN apk add --no-cache curl icu icu-dev

WORKDIR /app

COPY --from=build /out .

EXPOSE 8080

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=true

ENTRYPOINT ["dotnet", "Winterhaven.API.Presentation.dll"]

