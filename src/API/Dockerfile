# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /app

# Copy shared properties first
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy only project files
COPY API/Core/Winterhaven.API.Core.Domain/Winterhaven.API.Core.Domain.csproj \
     API/Core/Winterhaven.API.Core.Domain/

COPY API/Core/Winterhaven.API.Core.Application/Winterhaven.API.Core.Application.csproj \
     API/Core/Winterhaven.API.Core.Application/

COPY API/Infrastructure/Winterhaven.API.Infrastructure/Winterhaven.API.Infrastructure.csproj \
     API/Infrastructure/Winterhaven.API.Infrastructure/

COPY API/Presentation/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj \
     API/Presentation/Winterhaven.API.Presentation/

COPY API/Tests/Winterhaven.API.Tests/Winterhaven.API.Tests.csproj \
     API/Tests/Winterhaven.API.Tests/

RUN dotnet restore API/Presentation/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj

COPY API/ API/

RUN dotnet test API/Tests/Winterhaven.API.Tests/Winterhaven.API.Tests.csproj -c Release --no-restore
RUN dotnet publish API/Presentation/Winterhaven.API.Presentation/Winterhaven.API.Presentation.csproj -c Release -o /out

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

ENTRYPOINT ["dotnet", "Winterhaven.API.Presentation.dll"]

