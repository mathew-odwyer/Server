<div align="center">

# Mantanimus

[![Built with: ASP.NET](https://img.shields.io/badge/Built%20with-ASP.NET-512BD4)](https://dotnet.microsoft.com/apps/aspnet)
[![Made with: GameMaker](https://img.shields.io/badge/Made%20with-GameMaker-000000)](https://gamemaker.io/)
[![Built with: Node.js](https://img.shields.io/badge/Built%20with-Node.js-339933?logo=node.js&logoColor=white)](https://nodejs.org/)

[![Tests](https://img.shields.io/badge/tests-passing-brightgreen)](https://github.com/mathew-odwyer/Server/actions)
[![Release](https://img.shields.io/github/v/release/mathew-odwyer/Server?include_prereleases)](https://github.com/mathew-odwyer/Server/releases)

[![License: Proprietary](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE)

</div>

## Overview

Mantanimus is a work-in-progress 2D top-down online role-playing game set in a medieval fantasy world. Experience real-time multiplayer gameplay with smooth movement and secure networking infrastructure.

> 🎮 This project showcases full-stack game server development capabilities, demonstrating expertise in distributed systems, real-time networking, and secure architecture.

## ✨ Key Features

### Current Implementation

- **Multiplayer Experience**
  - Register and login via the game client.
  - Player movement with client-side prediction and server reconciliation.
  - Global and private chat system with visual effects.

- **Security Infrastructure**
  - ASP.NET Identity authentication.
  - JWT-based authorization.
  - Single-session enforcement.
  - SSL/TLS encryption via Caddy reverse proxy.

- **Technical Architecture**
  - Containerized microservices using Docker.
  - Distributed caching with Redis.
  - Cross-service communication with NATS.
  - MSSQL database with Entity Framework Core.
  - Real-time WebSocket communication.

- **Coming Soon**
  - Real-time combat system with strategic mechanics
  - Secure banking and inventory management
  - Comprehensive skills progression system
  - Player-driven economy with trading features

## 🏗️ Architecture

### Core Services

| Service | Technology Stack | Purpose |
|---------|-----------------|----------|
| Web API | C#, ASP.NET Identity, EF Core | User authentication and data management. |
| Database | MSSQL | Persistent data storage with code-first approach. |
| Gateway | NodeJS | Client connection management and routing. |
| Game Server | GameMaker Language (GML) | Real-time game logic processing. |
| Infrastructure | Caddy, Redis, NATS | Security, communication and caching. |

## 🚀 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [GameMaker](https://gamemaker.io/en/download) (Latest Stable, not LTS)

### Installation

1. Download latest release
   - Get the latest release from [GitHub Releases](https://github.com/mathew-odwyer/Server/releases).
   > [!NOTE]
   > Don't clone the repository as the client is not open-source due to external licensing requirements. Ensure you download the [latest release](https://github.com/mathew-odwyer/Server/releases) if you wish to play the game.

2. Configure environment
   - Copy `.env.example` to `.env`.
   - Copy `.appsettings.Example.json` to `appsettings.json`.

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