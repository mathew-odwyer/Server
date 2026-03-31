# Winterhaven Local Setup Guide

TODO: Work on automating as much of this possible.
    - Provide defaults where possible
    - Maybe create a small utility script that will generate the required files for local use

## Docker

Create `docker-compose.override.yml` and add:
    - Create a `.env` file in the source folder and any required services, including the variables listed in the [Required Environment Variables](#required-environment-variables) section
    - Add volume to Caddy to use the `Development.Caddyfile`
    - Add volume to Caddy to store the _caddy data_ locally so that certificates can be trusted
    - Expose any ports that may be required for testing, specifying which service uses which port:
        - 8080: Web API
        - 1433: SQL Server
        - 22: GM Builder (SSH)
        - Add additional ports as needed for other services
    - Ensure that the hosts file contains an entry for Winterhaven (ie; `127.0.0.1 winterhaven.com.au`)

Don't forget about adding in GM Builder:

```yml
  builder:
    container_name: winterhaven-builder
    image: winterhaven-builder
    build:
# The following settings are required if you are running Docker Desktop on Windows and need to perform privileged operations (such as mounting certain filesystems or using specific device nodes):
#    cap_add:
#      - SYS_ADMIN
#      - MKNOD
#    security_opt:
#      - seccomp:unconfined
#      - SYS_ADMIN
#      - MKNOD
#    security_opt:
#      - seccomp:unconfined
  
```

## Web

Create `appsettings.json` and add:
    - `ConnectionStrings:Docker` or `ConnectionStrings:Default`
    - Ensure the connection string will trust the server certificate (only for local dev)
    - `ApiOptions`
    - `JwtOptions`
    - `RateLimitOptions` (_optional_)
    - `Logging` (_optional_)

## Database

- Ensure that the system administrator can login via the Web API and SSMS
- Ensure that a Winterhaven database has been created
- Ensure the Web API `api/Health` endpoint is working
- Ensure the Web API `api/UserAccount/Register` endpoint is working

## Room Server

- Open up GameMaker and test the connection to GM Builder
- Ensure that the server can be built using GM Builder
- Run the server and ensure it can start up properly

## Client

- Ensure all core services are running
- Run an instance of the client in debug mode

# Required Environment Variables

```
ASPNETCORE_ENVIRONMENT
ACCEPT_EULA
SA_PASSWORD
API_URL (internal connection within docker to the Web API; normally http://web:8080)
API_KEY (internal connection within docker to the Web API; normally http://web:8080)
MAP_NAME

ApiOptions
JwtOptions
RateLimitOptions
Logging
```
