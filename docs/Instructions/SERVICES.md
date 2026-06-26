## Core Services

- **Client**
    - Type: Frontend
    - Technology: GameMaker LTS 2026.
    - Langauge: GameMaker Language (GML).
    - Purpose: User-facing game client.
    - Architecture: Loose but structured (no definitive model).
- **Gateway:**
    - Type: Backend
    - Technology: ASP.NET Core, .NET 10.0.x, WebSockets, NATS Client.
    - Language: Modern, C# 13.
    - Purpose: Relay server (via NATS and API), user session management, client connection lifecycle management.
    - Architecture: Clean Architecture (Domain, Application, Infrastructure, Presentation).
- **API:**
    - Type: Backend
    - Technology: ASP.NET Core, .NET 10.0.x, HTTP, JWT (not stateless).
    - Langauge: Modern, C# 13.
    - Purpose: Authentication & user management, data persistence.
    - Architecture: Clean Architecture (Domain, Application, Infrastructure, Presentation).
- **Room:**
    - Type: Backend
    - Technology: GameMaker LTS 2026, NATS Client.
    - Language: GameMaker Language (GML).
    - Purpose: Handles real-time game logic per region/room.
    - Architecture: Loose but structured (no definitive model).

## Other Services

- **Caddy:** Reverse proxy for Gateway with SSL/TLS termination.
- **Database:** MSSQL + EF Core (Docker container uses SQL Server Edge).
- **Orchestration:** Docker & Docker Compose (container management and service coordination).
- **Brokering:** NATS (**no** JetStream, _yet_) - handles communication between backend services.

## More Context

The system is composed of world gateways and room instances. A gateway represents a single world shard, owns a collection of room instances, and manages their lifecycle. Clients connect exclusively to the gateway, which routes messages and player state between room instances and connected clients.
