# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] - DD-MM-YYYY

### Added

- [#91] - Gateway Server (_by [@mathew-odwyer]_).
  - [#72] GML NATS Client (_by [@mathew-odwyer]_).
  - [#97] - Gateway Infrastructure Setup (_by [@mathew-odwyer]_).
  - [#94] - Gateway Registration Flow (_by [@mathew-odwyer]_).

### Fixed

- [#59] - Server Sends Error Messages to Client (_by [@mathew-odwyer]_).

## [v0.3.0] - 08-06-2026

### Added

- [#158] - Player Name Tags (_by [@mathew-odwyer]_).
- [#19] - Server CI/CD Pipeline (_by [@mathew-odwyer]_).

### Fixed

- [#69] - Client Crashes when Server Sends `player.reconcile` before Player Exists (_by [@mathew-odwyer]_).
- [#129] - Room Service Entry Point Causes Crash on Service Restart (_by [@mathew-odwyer]_).
- [#60] - Convert ASP.NET Identity Codes to Human Readable Strings (_by [@mathew-odwyer]_).

### Changed

- [#64] - Add Sprite to `obj_entity_clock` (_by [@mathew-odwyer]_).
- [#35] - Improve Performance of Remote Player Actions (_by [@mathew-odwyer]_).
- [#88] - Improve Performance of `obj_depth_sorter` (_by [@mathew-odwyer]_).

## [v0.2.0] - 05-12-2025

### Added

- [#11] - Global Web API Rate Limiting (_by [@mathew-odwyer]_).
- [#50] - Hole Punch Light Engine (_by [@mathew-odwyer]_).
- [#36] - Chat UI and Emotes (_by [@mathew-odwyer]_).

### Changed

- [#34] - Maps are Loaded from Web API end-point (_by [@mathew-odwyer]_).
- [#25] - Added Multiple Validation Error Support and updated UI (_by [@mathew-odwyer]_).

### Fixed

- [#41] - Fixed crash when a client disconnects before the server responds (_by [@mathew-odwyer]_).
- [#44] - Fixed Diagonal Movement (_by [@mathew-odwyer]_).

## [v0.1.0] - 21-11-2025

### Added

- Configured for Demo Environment (_by [@mathew-odwyer]_).
- Global Chat (_by [@mathew-odwyer]_).
- Player Movement (_by [@mathew-odwyer]_).
- Player Login and Sync (_by [@mathew-odwyer]_).
- User Login Flow (_by [@mathew-odwyer]_).
- User Registration Flow (_by [@mathew-odwyer]_).
- Web API Infrastructure (_by [@mathew-odwyer]_).

[Unreleased]: https://github.com/mathew-odwyer/Server/compare/v0.3.0...HEAD
[v0.3.0]: https://github.com/mathew-odwyer/Server/releases/tag/v0.3.0
[v0.2.0]: https://github.com/mathew-odwyer/Server/releases/tag/v0.2.0
[v0.1.0]: https://github.com/mathew-odwyer/Server/releases/tag/v0.0.1

[#94]: https://github.com/mathew-odwyer/Server/issues/94
[#97]: https://github.com/mathew-odwyer/Server/issues/97
[#91]: https://github.com/mathew-odwyer/Server/issues/91
[#72]: https://github.com/mathew-odwyer/Server/issues/72
[#158]: https://github.com/mathew-odwyer/Server/issues/158
[#69]: https://github.com/mathew-odwyer/Server/issues/69
[#129]: https://github.com/mathew-odwyer/Server/issues/129
[#64]: https://github.com/mathew-odwyer/Server/issues/64
[#35]: https://github.com/mathew-odwyer/Server/issues/35
[#88]: https://github.com/mathew-odwyer/Server/issues/88
[#60]: https://github.com/mathew-odwyer/Server/issues/60
[#19]: https://github.com/mathew-odwyer/Server/issues/19
[#34]: https://github.com/mathew-odwyer/Server/issues/34
[#25]: https://github.com/mathew-odwyer/Server/issues/25
[#41]: https://github.com/mathew-odwyer/Server/issues/41
[#50]: https://github.com/mathew-odwyer/Server/issues/50
[#44]: https://github.com/mathew-odwyer/Server/issues/44
[#36]: https://github.com/mathew-odwyer/Server/issues/36
[#11]: https://github.com/mathew-odwyer/Server/issues/11

[@mathew-odwyer]: https://github.com/mathew-odwyer
