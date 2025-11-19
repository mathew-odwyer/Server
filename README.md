<div align="center">

# Winterhaven

[Play Winterhaven MVP]()

[![Built with: ASP.NET](https://img.shields.io/badge/Built%20with-ASP.NET-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![Made with: GameMaker](https://img.shields.io/badge/Made%20with-GameMaker-000000)](https://gamemaker.io/)
[![Built with: Node.js](https://img.shields.io/badge/Built%20with-Node.js-339933?logo=node.js&logoColor=white)](https://nodejs.org/)

[![Release](https://img.shields.io/github/v/release/mathew-odwyer/Server?include_prereleases)](https://github.com/mathew-odwyer/Server/releases)
![License: Proprietary](https://img.shields.io/badge/License-Proprietary-red.svg)

</div>

## Overview

Winterhaven is a work-in-progress 2D top-down online role-playing game set in a medieval fantasy world. Experience real-time multiplayer gameplay with smooth movement and secure networking infrastructure.

> 🎮 This project showcases full-stack game server development capabilities, demonstrating expertise in distributed systems, real-time networking, and secure architecture.

<img width="1282" height="752" alt="Screenshot 2025-09-27 002412" src="https://github.com/user-attachments/assets/b08b891f-cfc9-417f-b13c-85ccf00f8964" />

## ✨ Key Features

### Current Implementation

- **Security Infrastructure**
  - ASP.NET Identity authentication.
  - JWT-based authorization.
  - Single-session enforcement.
  - SSL/TLS encryption via Caddy reverse proxy.

- **Technical Architecture**
  - Containerized microservices using Docker.
  - MSSQL database with Entity Framework Core.
  - Real-time WebSocket communication.
  - Clean architecture by design.

- **Multiplayer Experience**
  - Register and login via the game client.
  - Player movement with client-side prediction and server reconciliation.
  - Communicate with other players on the same server.

## 🏗️ Architecture

### Core Services

| Service | Technology Stack | Purpose |
|---------|-----------------|----------|
| Web API | C#, ASP.NET Identity, EF Core | User authentication and data management. |
| Database | MSSQL | Persistent data storage with code-first approach. |
| Server | GameMaker Language (GML) | Real-time game logic processing. |
| Infrastructure | Caddy, Redis, NATS | Security, communication and caching. |

## 🚀 Getting Started

> [!NOTE]  
> Are you an employer or just hoping to test out the game as quickly as possible? I've hosted it online in a test environment [here](). Just make an account and login.

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [GameMaker](https://gamemaker.io/en/download) (Latest Stable, not LTS)

### Installation

1. Download latest release
   - Get the latest release from [GitHub Releases](https://github.com/mathew-odwyer/Server/releases).

2. Configure environment
   - There's a fair bit to get setup and running locally, feel free to message me directly if you're looking to get started quickly (_@softwareantics_ on Discord).
   - I have plans in the future to automate through process through a CLI tool if my project gains enough traction.

3. Build and run services
   ```bash
   docker-compose up --build
   ```

4. Set up SSL certificate
   ```bash
   certutil -addstore "Root" "%APPDATA%\Caddy\root.pem"
   ```

5. Launch the client through GameMaker IDE or Stitch, create an account and have fun!

## 📝 Changelog

See [CHANGELOG.md](./CHANGELOG.md) for version history.

## 📄 License

Copyright © 2025 Mathew O'Dwyer. All rights reserved.

This software is proprietary and may not be redistributed, modified, or used commercially without explicit permission from Mathew O'Dwyer.
