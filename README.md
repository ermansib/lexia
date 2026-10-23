# Lexia

Lexia helps authors research, create and publish books on Amazon KDP, from low-content puzzle books and journals to illustrated kids' stories and full-length fiction. It combines what LitPal (AI creation) and Book Bolt (research and design) do, in one guided flow: **Idea → Research → Create → Design → Publish**.

The full product requirements live in [REQUIREMENTS.md](REQUIREMENTS.md).

## What works today (Phase 1 foundation)

- **My books**: create book projects of any type and move them through the guided flow. Stages advance one step at a time, and a book can only be marked live once its print spec and listing are complete.
- **Puzzle book maker**: print-ready KDP interiors for word search, sudoku (every puzzle has exactly one solution) and mazes, with title page, instructions and answer key.
- **Journal maker**: lined, dot grid, graph and blank journals.
- **Cover calculator**: full-wrap cover size, spine width and pixel size for every KDP trim size and paper type.
- **KDP print checker**: catches the page size, margin, font, spine text and image resolution problems that get paperbacks rejected.
- **Amazon listing**: description and the 7 backend keywords, checked against KDP limits.

Next up: niche research with an AI opportunity score, the AI co-writer with a story bible, and AI illustration with consistent characters.

## Architecture

Microservices on .NET 10, each built with Clean Architecture (Domain, Application, Infrastructure, Api) and CQRS handlers without a paid mediator. Everything is free and open source.

| Part | Folder | What it owns | Storage |
|---|---|---|---|
| Gateway | `services/Gateway` | One entry point, routes `/api/v1/<area>` with YARP | none |
| Books | `services/Books` | Book projects, guided-flow stages, listing | PostgreSQL |
| Studio | `services/Studio` | KDP rules, puzzle generation, PDF interiors | none (stateless) |
| Web | `frontend` | React + TypeScript + Vite + Tailwind | |
| Shared | `services/BuildingBlocks` | Result type, handler interfaces, validation and logging decorators, auth and service defaults | |

Supporting services in `docker-compose.yml`: PostgreSQL, Redis, MongoDB (manuscripts and story bibles, from the AI co-writer onward), RabbitMQ (events between services) and Keycloak (sign-in).

## Run it

### Everything in Docker

```bash
docker compose up --build
```

Open http://localhost:3000 and sign in as `author` / `author`. Keycloak's admin console is at http://localhost:8080 (`admin` / `admin`). These are local development credentials only.

### Develop without Docker

Requirements: .NET 10 SDK, Node 22, PostgreSQL 16 or newer.

```bash
# Database (once)
createuser -s lexia && psql -c "ALTER USER lexia PASSWORD 'lexia'" && createdb -O lexia lexia_books

# Services (each in its own terminal). In Development they sign you in as a local author, so Keycloak is optional.
dotnet run --project services/Books/src/Lexia.Books.Api     # http://localhost:5101, migrates the database on start
dotnet run --project services/Studio/src/Lexia.Studio.Api   # http://localhost:5102
dotnet run --project services/Gateway/Lexia.Gateway         # http://localhost:5000

# Web app
cd frontend && npm install && npm run dev                    # http://localhost:5173
```

OpenAPI documents are served at `/openapi/v1.json` on each service in Development.

## Quality checks

```bash
cd services && dotnet build Lexia.slnx && dotnet format Lexia.slnx --verify-no-changes && dotnet test Lexia.slnx
cd frontend && npm run lint && npm run typecheck && npm test && npm run build
```

GitHub Actions runs the same checks on every pull request. Architecture tests fail the build if a layer depends on something it must not.

## Licences

Only packages that are free for commercial use are allowed. Some popular libraries changed licence in newer versions, so check before upgrading:

- **PDFsharp** stays on 6.2.x (MIT). 6.5 and later are AGPL.
- **MediatR**, **AutoMapper** 15+, **MassTransit** 9+ and **FluentAssertions** 8+ are commercial and are not used.
- **Redis** stays on 7.2 (BSD); Valkey is a drop-in alternative.
- The bundled DejaVu fonts use the Bitstream Vera licence (free to embed), see `services/Studio/src/Lexia.Studio.Infrastructure/Fonts/DEJAVU-LICENSE.txt`.
