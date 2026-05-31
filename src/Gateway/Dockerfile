# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

# Copy shared properties first
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy only project files
COPY Gateway/Winterhaven.Gateway.Core.Domain/Winterhaven.Gateway.Core.Domain.csproj \
    Gateway/Winterhaven.Gateway.Core.Domain/

COPY Gateway/Winterhaven.Gateway.Core.Application/Winterhaven.Gateway.Core.Application.csproj \
    Gateway/Winterhaven.Gateway.Core.Application/

COPY Gateway/Winterhaven.Gateway.Infrastructure/Winterhaven.Gateway.Infrastructure.csproj \
    Gateway/Winterhaven.Gateway.Infrastructure/

COPY Gateway/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj \
    Gateway/Winterhaven.Gateway.Presentation/

COPY Gateway/Winterhaven.Gateway.Tests/Winterhaven.Gateway.Tests.csproj \
    Gateway/Winterhaven.Gateway.Tests/

COPY Gateway/Winterhaven.Gateway.Integrations/Winterhaven.Gateway.Integrations.csproj \
    Gateway/Winterhaven.Gateway.Integrations/

COPY Common/Winterhaven.Common.DTOs/Winterhaven.Common.DTOs.csproj \
    Common/Winterhaven.Common.DTOs/

COPY Common/Winterhaven.Common.Extensions/Winterhaven.Common.Extensions.csproj \
    Common/Winterhaven.Common.Extensions/

COPY appsettings.json Gateway/Winterhaven.Gateway.Presentation/

RUN dotnet restore Gateway/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj

COPY Gateway/ Gateway/
COPY Common/ Common/

RUN dotnet test Gateway/Winterhaven.Gateway.Tests/Winterhaven.Gateway.Tests.csproj -c Release --no-restore
RUN dotnet test Gateway/Winterhaven.Gateway.Integrations/Winterhaven.Gateway.Integrations.csproj -c Release --no-restore
RUN dotnet publish Gateway/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

# Create a new user (non-root) this fixes
# This is because running containers as a privileged user is security-sensitive
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

WORKDIR /app

COPY --from=build /out .

# Ensure the application files are owned by the non-root user
RUN chown -R appuser:appgroup /app

# Switch to the non-root user
USER appuser

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=true

ENTRYPOINT ["dotnet", "Winterhaven.Gateway.Presentation.dll"]

