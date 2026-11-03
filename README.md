# Retro Gaming Consoles Management System

A full-stack, enterprise-grade Clean Architecture system for managing retro gaming manufacturers and consoles, featuring geographic mapping with Leaflet, domain business invariant enforcement, RESTful API endpoints adhering to RFC 7807 ProblemDetails, and a reactive Vue 3 + TypeScript SPA.

---

## Table of Contents
1. [Quickstart Guide](#quickstart-guide)
   - [Prerequisites](#prerequisites)
   - [1. Database Setup (Docker SQL Server)](#1-database-setup-docker-sql-server)
   - [2. Backend Setup (.NET 10 Web API)](#2-backend-setup-net-10-web-api)
   - [3. Frontend Setup (Vue 3 + TypeScript)](#3-frontend-setup-vue-3--typescript)
   - [4. Running Automated Tests](#4-running-automated-tests)
2. [Architecture & System Design](#architecture--system-design)
   - [Clean Architecture Layers](#clean-architecture-layers)
   - [SOLID Principles Alignment](#solid-principles-alignment)
3. [Domain Rules & Invariants](#domain-rules--invariants)
4. [API Endpoints & Error Handling](#api-endpoints--error-handling)
5. [Frontend Architecture](#frontend-architecture)
6. [Phase-by-Phase Implementation Log](#phase-by-phase-implementation-log)
7. [AI Usage Disclosure Log](#ai-usage-disclosure-log)

---

## Quickstart Guide

### Prerequisites
- **.NET SDK**: .NET 10 SDK (or compatible .NET runtime)
- **Node.js**: Node.js 18+ and npm / pnpm / yarn
- **Docker & Docker Compose**: For running SQL Server 2022 locally

### 1. Database Setup (Docker SQL Server)
Start the local SQL Server 2022 container in the background:
```bash
docker compose up -d
```
*Note: SQL Server will be exposed on port `1433` with `sa` credentials specified in `docker-compose.yml` (`Password: YourStrong@Passw0rd!`).*

### 2. Backend Setup (.NET 10 Web API)
Apply database migrations and launch the ASP.NET Core Web API:
```bash
# Navigate to the project root and run the API project
dotnet run --project src/RetroGaming.Application
```
- API Base URL: `http://localhost:5193`
- Interactive Swagger UI: `http://localhost:5193/swagger`

The database schema and deterministic seed data (manufacturers with HQ coordinates and classic retro consoles) will be initialized automatically upon startup.

### 3. Frontend Setup (Vue 3 + TypeScript)
Launch the Vue 3 development server:
```bash
cd src/RetroGaming.Client
npm install
npm run dev
```
- Frontend UI: `http://localhost:5173`
- The frontend connects automatically to the backend API at `http://localhost:5193` with CORS preconfigured.

### 4. Running Automated Tests
Run the entire unit and integration test suite:
```bash
dotnet test
```
- **Unit Tests (`RetroGaming.Tests.Unit`)**: Domain rules, invariants, EF Core configurations, FluentValidation, and service logic.
- **Integration Tests (`RetroGaming.Tests.Integration`)**: RESTful API endpoints, HTTP status codes, and RFC 7807 ProblemDetails responses via `CustomWebApplicationFactory`.

---

## Architecture & System Design

### Clean Architecture Layers
> Project-name note: the runnable ASP.NET Core host/entry-point project is `RetroGaming.Application` (controllers, middleware, `Program.cs`), while the application/use-case layer (service interfaces, DTOs, validators, orchestration) lives in `RetroGaming.UseCases`.

```
┌────────────────────────────────────────────────────────┐
│             Presentation / UI Layer (Vue 3)            │
└───────────────────────────┬────────────────────────────┘
                            │ HTTP / REST
┌───────────────────────────▼────────────────────────────┐
│   API / Host Layer (ASP.NET Core Web API)             │
│   project: RetroGaming.Application                     │
│    - REST Controllers (Manufacturers, Consoles, Map)   │
│    - Global Exception Middleware (RFC 7807)           │
│    - Dependency Injection Registration                 │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│          Use Cases Layer                               │
│          project: RetroGaming.UseCases                 │
│    - Service Interfaces (IManufacturerService, etc.)   │
│    - DTOs (Request / Response Models)                  │
│    - Business Use Cases & Service Orchestration        │
│    - Validation Rules (FluentValidation)               │
└─────────────────┬───────────────────┬──────────────────┘
                  │                   │
┌─────────────────▼─────────┐   ┌─────▼──────────────────┐
│       Domain Layer        │   │  Infrastructure Layer  │
│  - Entities (Manufacturer,│   │  - AppDbContext        │
│    Console)               │   │  - Fluent Configurations│
│  - Domain Business Rules  │   │  - EF Core Migrations  │
│  - Domain Exceptions      │   │  - Deterministic Seed  │
│  (Zero dependencies)      │   │  - Repository Impls    │
└───────────────────────────┘   └────────────────────────┘
```

### SOLID Principles Alignment
- **Single Responsibility Principle (SRP)**: Each class has a single, well-defined responsibility. Domain entities manage state and domain invariants; FluentValidation validators enforce format/bounds constraints; Application services coordinate use cases; Controllers handle routing and HTTP status translation; Infrastructure handles persistence.
- **Open/Closed Principle (OCP)**: Business behaviors and validations can be extended without altering existing pipelines via interfaces, validation rules, and middleware.
- **Liskov Substitution Principle (LSP)**: Service abstractions (`IManufacturerService`, `IConsoleService`, `IMapService`, `IAppDbContext`) can be substituted cleanly (e.g., in-memory test doubles during integration testing).
- **Interface Segregation Principle (ISP)**: Focused, purpose-built interfaces rather than monolithic abstractions.
- **Dependency Inversion Principle (DIP)**: High-level Application and Domain layers do not depend on low-level Infrastructure details; Infrastructure implements abstractions declared in the Application layer.

---

## Domain Rules & Invariants

1. **Manufacturer Rules**:
   - `Name`: Required, non-empty, maximum 100 characters.
   - `FoundedYear`: Must be a reasonable historical year (between 1800 and the current calendar year).
   - `Geo-Coordinates`: Latitude must be between `-90.0` and `90.0`; Longitude must be between `-180.0` and `180.0`.
   - `Unique Console Names`: A manufacturer cannot have duplicate console names (case-insensitive).
   - `Cascade Deletion Protection`: A manufacturer with active consoles cannot be deleted (enforced both at domain service level and database level via `DeleteBehavior.Restrict`).

2. **Console Rules**:
   - `Name`: Required, non-empty, maximum 100 characters.
   - `ReleaseYear`: Must be $\ge 1970$ and **cannot predate the associated manufacturer's founding year**.
   - `Generation`: Must be $\ge 1$ (representing console hardware generations 1 through 9+).
   - `UnitsSoldInMillions`: Must be non-negative ($\ge 0.0$).
   - `Manufacturer Association`: Must reference an existing, valid manufacturer ID.

3. **Exception Hierarchy**:
   - `DomainException`: Abstract root exception.
   - `DomainValidationException`: Thrown when fields or inputs violate format/range constraints.
   - `EntityNotFoundException`: Thrown when queried entities do not exist.
   - `InvalidDomainOperationException`: Thrown when business rules or relational invariants are violated (e.g., duplicate names, release year before founding year, deleting manufacturer with consoles).

---

## API Endpoints & Error Handling

### RESTful Endpoints
| HTTP Method | Route | Description | Success Code |
|:---|:---|:---|:---|
| `GET` | `/api/manufacturers` | Retrieve all manufacturers with console counts | `200 OK` |
| `GET` | `/api/manufacturers/{id}` | Retrieve manufacturer details and hardware catalogue | `200 OK` |
| `POST` | `/api/manufacturers` | Create a new manufacturer | `201 Created` |
| `PUT` | `/api/manufacturers/{id}` | Update an existing manufacturer | `204 NoContent` |
| `DELETE` | `/api/manufacturers/{id}` | Delete a manufacturer (blocked if consoles exist) | `204 NoContent` |
| `GET` | `/api/consoles` | Search & sort consoles (query params: `searchTerm`, `manufacturerId`, `generation`, `isDiscontinued`, `sortBy`, `sortDescending`) | `200 OK` |
| `GET` | `/api/consoles/{id}` | Retrieve single console details | `200 OK` |
| `POST` | `/api/consoles` | Create a new console under a manufacturer | `201 Created` |
| `PUT` | `/api/consoles/{id}` | Update an existing console | `204 NoContent` |
| `DELETE` | `/api/consoles/{id}` | Delete a console | `204 NoContent` |
| `GET` | `/api/map/locations` | Retrieve manufacturers with coordinates and console counts for map markers | `200 OK` |

### RFC 7807 ProblemDetails Error Responses
All errors are normalized through `ExceptionHandlingMiddleware` into RFC 7807 compliant structures:
- `400 Bad Request`: Validation failures (`DomainValidationException`, argument errors).
- `404 Not Found`: Non-existent entity lookups (`EntityNotFoundException`).
- `409 Conflict`: Business invariant conflicts (`InvalidDomainOperationException` — duplicate names, release year < founding year, deletion with active consoles).
- `500 Internal Server Error`: Unhandled server exceptions.

---

## Frontend Architecture

- **Framework**: Vue 3 with TypeScript and Vite.
- **State Management**: Pinia stores (`useConsoleStore`, `useManufacturerStore`, `useMapStore`, `useToastStore`).
- **Styling**: Tailwind CSS with custom retro-cyberpunk styling (custom dark scrollbars, glowing neon accents, card elevations).
- **Key Views**:
  - **Consoles Management (`ConsolesView.vue`)**: Search by text, filter by manufacturer, generation, or discontinued status; interactive column sorting; responsive CRUD modals with client validation.
  - **Manufacturers Management (`ManufacturersView.vue`)**: Responsive card grid with HQ badges, coordinate tags, console count pills, hardware catalogue inspection modal, and deletion conflict error alerts.
  - **Interactive Map (`MapView.vue`)**: Leaflet dark-mode world map with custom glowing animated marker pins displaying console counts, smooth fly-to animations, and interactive popup cards with direct catalog navigation.
  - **Notifications (`ToastContainer.vue`)**: Centralized toast notification stack with auto-dismiss timers for instant user feedback.

---

## Phase-by-Phase Implementation Log

### Phase 1: Project Scaffolding & Solution Architecture
- Clean Architecture .NET solution structure (`RetroGaming.Domain`, `RetroGaming.UseCases`, `RetroGaming.Infrastructure`, `RetroGaming.Application`, `RetroGaming.Tests.Unit`, `RetroGaming.Tests.Integration`).
- Vue 3 + TypeScript + Vite + Tailwind CSS + Pinia frontend client scaffold.
- `docker-compose.yml` for SQL Server 2022 container.

### Phase 2: Domain Layer Modelling & Business Rules
- Domain exceptions (`DomainException`, `DomainValidationException`, `EntityNotFoundException`, `InvalidDomainOperationException`).
- `Manufacturer` and `Console` entities with domain invariants and business rules.
- 34 comprehensive domain unit tests with 100% pass rate.

### Phase 3: Infrastructure & Persistence Layer
- `IAppDbContext` interface and `AppDbContext` with EF Core Fluent API configurations (`ManufacturerConfiguration`, `ConsoleConfiguration`).
- Restrict delete foreign key behavior preventing cascading deletes.
- Deterministic seed data for 5 classic manufacturers (Nintendo, Sega, Sony, Atari, Microsoft) with geo-coordinates and 10 classic consoles.
- Initial EF Core migration (`InitialCreate`) for SQL Server.
- Persistence unit tests for configurations and seed data.

### Phase 4: Application Layer (Contracts, Use Cases & Validation)
- DTO contracts for Manufacturers, Consoles, and Map Locations.
- FluentValidation request validators (`CreateManufacturerRequestValidator`, etc.).
- Service implementations (`ManufacturerService`, `ConsoleService`, `MapService`) handling CRUD, search filtering, multi-field sorting, duplicate name prevention, founding vs release year checks, and cascade deletion guards.
- Application unit tests in `ValidatorTests` and `ServiceTests` (29 tests).

### Phase 5: RESTful API Layer & Cross-Cutting Concerns
- RESTful API controllers (`ManufacturersController`, `ConsolesController`, `MapController`).
- Centralized RFC 7807 `ExceptionHandlingMiddleware`.
- Swagger UI and CORS policy for local Vue 3 dev servers.
- End-to-end API integration tests (`CustomWebApplicationFactory`).

### Phase 6: Frontend Development (Vue 3 + TypeScript)
- Typed API client (`src/services/api.ts`) with RFC 7807 error parsing.
- Pinia state stores (`useConsoleStore`, `useManufacturerStore`, `useMapStore`, `useToastStore`).
- Consoles management view with dynamic search, multi-field sorting, and CRUD modals.
- Manufacturers management view with responsive grid and catalogue drawer.
- Leaflet interactive map with custom glowing pins, popups, and fly-to transitions.

### Phase 7: Automated Testing & Verification
- Unit test suite (`RetroGaming.Tests.Unit`): 68 tests covering Domain and Application logic.
- Integration test suite (`RetroGaming.Tests.Integration`): 18 tests covering REST endpoints, error mapping, and CRUD workflows.
- Full solution test suite: **85 meaningful tests passing (100% pass rate)** with NUnit 3 and FluentAssertions (an empty scaffolding placeholder test was removed during the Phase 8 cleanup).

### Phase 8: Documentation & Final Deliverables
- Comprehensive documentation in `README.md` including Quickstart guide, Clean Architecture breakdown, SOLID principles, domain rules, API endpoints table, and AI disclosure.
- **SOLID Review & Codebase Cleanup**:
  - Reviewed every layer and confirmed SOLID compliance (see [SOLID Principles Alignment](#solid-principles-alignment)): strict Clean Architecture dependency direction, `RetroGaming.Domain` with zero external dependencies, business invariants encapsulated in domain entities, focused service interfaces resolved through constructor injection, and cross-cutting concerns isolated in middleware.
  - Removed leftover auto-generated scaffolding (`UnitTest1.cs` empty placeholder test in `RetroGaming.Tests.Integration`) to keep the test suite meaningful and intentional.
  - Verified clean build with zero compiler errors and full test suite re-run after cleanup (**85 tests passing, 100% pass rate**).
- **Project Rename (Entry-Point Clarification)**:
  - Renamed the runnable ASP.NET Core host/entry-point project from `RetroGaming.Api` to `RetroGaming.Application` (controllers, middleware, `Program.cs`).
  - Renamed the former application/use-case class library from `RetroGaming.Application` to `RetroGaming.UseCases` to resolve the naming clash while keeping Clean Architecture dependency direction intact.
  - Updated all namespaces, project references, the solution file, and documentation accordingly; re-ran the full suite (**85 tests passing, 100% pass rate**).
- Complete task sign-off across all phases.

---

## Timebox
- **Timebox**: 3-5 hours total.
- **Used**
  - 10m Creating phased plan to feed into agentic AI.
  - 1h Walking AI through phased plan.
  - 30m Running through full build following README.md
  - 1h Identifying some defects and clarifications, resolving issues.
  - 30m Cleaning and updating README.md.
- **Total Time Spent**: 3h 10m~

---

## AI Usage Disclosure Log
- **Tools Used**: JetBrains Junie (Agentic AI powered by `claude-opus-4-8`).
- **Tasks Performed by AI**:
  - Scaffolding domain entities (`Manufacturer`, `Console`) and custom domain exception hierarchy.
  - Formulating domain invariant validation logic and relationships.
  - Implementing EF Core `AppDbContext`, Fluent API entity configurations, seed data, and migration generation.
  - Defining Application DTOs, FluentValidation validators, service contracts, and business logic implementations.
  - Writing API controllers, global exception handling middleware, CORS policy, and Swagger configuration.
  - Writing comprehensive unit and persistence tests in `RetroGaming.Tests.Unit`, integration tests in `RetroGaming.Tests.Integration`, and migrating test suites to NUnit 3 with FluentAssertions.
  - Building Vue 3 + TypeScript SPA frontend: API client with RFC 7807 parser, Pinia stores, Vue Router, retro Tailwind CSS layout, Toast notification system, Consoles table with multi-column sorting and filtering, Manufacturers cards grid, and Leaflet interactive map with custom pins and popups.
  - Authoring comprehensive architecture and operations documentation.
- **Verification & Adjustments**:
  - All unit and integration tests built and executed via .NET CLI (`dotnet test`) with 85 passing tests after the Phase 8 cleanup.
  - Zero-dependency rule on `RetroGaming.Domain` verified.
  - EF Core migration generated and verified against schema requirements.
  - Application service domain invariant validations and FluentValidation pipelines verified.
  - HTTP status codes (200, 201, 204, 400, 404, 409) and ProblemDetails verified via integration test suite.
  - Vue 3 frontend components, stores, routers, and Leaflet map integration structured and validated.

---

## Assumptions / Trade-offs
- **Assumptions**
    - The user is on a linux based system.
    - The user already has docker available to them, or they have available a pre-existing SQL database.
    - The user already has dotnet core 10 available to them.
    - The application will be hosted on a single server for simplicity.
    - The application is not intended to be highly scalable or distributed.
- **Trade-offs**
    - Because the application was not designed originally with scalability in mind, it will not be suitable for big data environments in its current state.
      - Horizontal scaling could be implemented with relative ease as the API layer is stateless, NGINX/HAProxy/Kubernetes Load Balancers could be used to distribute traffic across multiple instances of the application.
      - If the application was to be scaled or distributed, it would require changes to allow for pagination of the UI, the console list view is currently not paginated and same with the manufacturer list view.
      - Considerations could be made to allow the application to be scaled via caching. The database layer could rather easily be replaced with a distributed caching system such as Redis.
    - The database is currently the source of truth for the application and a single point of failure. If the db is unresponsive then the application ceases to function.
      - Considerations could be made for geo replication of the database to allow for failover, a small update to the application connection string would be required to allow for failover.