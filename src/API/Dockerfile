# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

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

COPY API/Winterhaven.API.Tests/Winterhaven.API.Tests.csproj \
    API/Winterhaven.API.Tests/

COPY Common/Winterhaven.Common/Winterhaven.Common.csproj \
    Common/Winterhaven.Common/

COPY Common/Winterhaven.Common.DTOs/Winterhaven.Common.DTOs.csproj \
    Common/Winterhaven.Common.DTOs/

COPY Common/Winterhaven.Common.Extensions/Winterhaven.Common.Extensions.csproj \
    Common/Winterhaven.Common.Extensions/

COPY appsettings.json API/Winterhaven.API.Presentation/

RUN dotnet restore API/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj

COPY API/ API/
COPY Common/ Common/

RUN dotnet test API/Winterhaven.API.Tests/Winterhaven.API.Tests.csproj -c Release --no-restore
RUN dotnet publish API/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

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

