# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

# Copy shared properties first
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy only project files
COPY Gateway/Core/Winterhaven.Gateway.Core.Domain/Winterhaven.Gateway.Core.Domain.csproj \
     Gateway/Core/Winterhaven.Gateway.Core.Domain/

COPY Gateway/Core/Winterhaven.Gateway.Core.Application/Winterhaven.Gateway.Core.Application.csproj \
     Gateway/Core/Winterhaven.Gateway.Core.Application/

COPY Gateway/Infrastructure/Winterhaven.Gateway.Infrastructure/Winterhaven.Gateway.Infrastructure.csproj \
     Gateway/Infrastructure/Winterhaven.Gateway.Infrastructure/

COPY Gateway/Presentation/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj \
     Gateway/Presentation/Winterhaven.Gateway.Presentation/

COPY Gateway/Tests/Winterhaven.Gateway.Tests/Winterhaven.Gateway.Tests.csproj \
     Gateway/Tests/Winterhaven.Gateway.Tests/

COPY Common/Winterhaven.Common.DTOs/Winterhaven.Common.DTOs.csproj \
     Common/Winterhaven.Common.DTOs/

COPY Common/Winterhaven.Common.Extensions/Winterhaven.Common.Extensions.csproj \
     Common/Winterhaven.Common.Extensions/

COPY Brokering/Winterhaven.Brokering/Winterhaven.Brokering.csproj \
     Brokering/Winterhaven.Brokering/

COPY appsettings.json Gateway/Presentation/Winterhaven.Gateway.Presentation/

RUN dotnet restore Gateway/Presentation/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj

COPY Gateway/ Gateway/
COPY Common/ Common/

RUN dotnet test Gateway/Tests/Winterhaven.Gateway.Tests/Winterhaven.Gateway.Tests.csproj -c Release --no-restore
RUN dotnet publish Gateway/Presentation/Winterhaven.Gateway.Presentation/Winterhaven.Gateway.Presentation.csproj -c Release -o /out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

RUN apk add --no-cache curl

WORKDIR /app

COPY --from=build /out .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=true

ENTRYPOINT ["dotnet", "Winterhaven.Gateway.Presentation.dll"]
