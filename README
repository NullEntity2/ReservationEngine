# ReservationEngine

A seat reservation system built with [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/). It provides a Blazor web frontend for browsing and reserving seats, backed by an API service that persists reservations in PostgreSQL.

## Solution structure

- **ReservationEngine.AppHost** — the Aspire orchestrator; wires up Postgres, Redis, and the two apps below for local development.
- **ReservationEngine.ApiService** — minimal API exposing seat availability and reservation endpoints, backed by EF Core over PostgreSQL.
- **ReservationEngine.Web** — Blazor Server frontend for viewing and reserving seats, using Redis for output caching.
- **ReservationEngine.ServiceDefaults** — shared Aspire service defaults (health checks, telemetry, resilience) referenced by both apps.
- **ReservationEngine.Tests** — integration tests built on the Aspire testing harness.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (or another OCI-compatible container runtime), used by Aspire to run Postgres and Redis locally

## Running the app

Start the whole solution via the AppHost, which provisions Postgres and Redis in containers and launches both apps:

```bash
dotnet run --project ReservationEngine.AppHost
```

This opens the Aspire dashboard, from which you can reach the web frontend and API service and inspect logs, traces, and metrics.

## API

- `GET /api/seats/reserved` — list currently reserved seat IDs.
- `POST /api/reservations` — reserve one or more seats (`{ "seatIds": ["A3", "B7"] }`); returns a conflict response listing any seats already taken.

## Running tests

```bash
dotnet test
```
