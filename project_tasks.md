# Project Tasks: Retro Gaming Consoles Management System

## Architectural Blueprint & Principles

### Clean Architecture Layers
```
┌────────────────────────────────────────────────────────┐
│             Presentation / UI Layer (Vue 3)            │
└───────────────────────────┬────────────────────────────┘
                            │ HTTP / REST
┌───────────────────────────▼────────────────────────────┐
│          API Layer (ASP.NET Core Web API)             │
│    - Controllers / Endpoints                           │
│    - Middleware (Exception Handling, Logging)         │
│    - Dependency Injection Registration                 │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                 Application Layer                      │
│    - Interfaces (Services / Repositories)             │
│    - DTOs (Request / Response Models)                  │
│    - Use Cases / Services & Business Logic Orchestration│
│    - Validation (FluentValidation)                     │
│    - Mappings                                          │
└─────────────────┬───────────────────┬──────────────────┘
                  │                   │
┌─────────────────▼─────────┐   ┌─────▼──────────────────┐
│       Domain Layer        │   │  Infrastructure Layer  │
│  - Entities & Value Objects│   │  - EF Core DbContext   │
│  - Domain Business Rules  │   │  - Entity Configurations│
│  - Domain Exceptions      │   │  - Migrations & Seeding│
│  (Zero dependencies)      │   │  - Repository Impls    │
└───────────────────────────┘   └────────────────────────┘
```

### SOLID Principles Alignment
- **Single Responsibility Principle (SRP):** Each class/component has a single purpose (e.g., Domain Entities hold state and domain invariants; Validators validate inputs; Repositories handle persistence; Controllers handle HTTP routing/status codes).
- **Open/Closed Principle (OCP):** Architecture allows extending behavior via interfaces, middleware, and domain rules without modifying core business pipelines.
- **Liskov Substitution Principle (LSP):** Concrete repository and service implementations seamlessly substitute their interfaces across layers.
- **Interface Segregation Principle (ISP):** Fine-grained interfaces for data access and business services (e.g., `IManufacturerRepository`, `ICallableMapLocationService`) rather than bloated generic interfaces.
- **Dependency Inversion Principle (DIP):** High-level modules (Application/API) depend on abstractions (interfaces), while low-level modules (Infrastructure/EF Core) implement them.

---

## Phase 1: Project Scaffolding & Solution Architecture

- [x] **1.1 Solution & Project Setup**
  - Create standard Clean Architecture .NET solution structure:
    - `RetroGaming.Domain` (Class Library - .NET 10 / netcore, no external dependencies)
    - `RetroGaming.UseCases` (Class Library - references Domain)
    - `RetroGaming.Infrastructure` (Class Library - references Application & Domain, EF Core packages)
    - `RetroGaming.Application` (Web API - references Application & Infrastructure)
    - `RetroGaming.Tests.Unit` & `RetroGaming.Tests.Integration` (xUnit / NUnit test projects)
- [x] **1.2 Frontend Setup**
  - Initialize Vue 3 project using Vite and TypeScript (`frontend/` or `RetroGaming.Client/`).
  - Configure Tailwind CSS (or clean UI framework like PrimeVue/Vuetify/custom modern CSS).
  - Setup Vue Router and Pinia for centralized state management.
  - Install mapping library: Leaflet (`leaflet` and `@types/leaflet`) or `vue-leaflet`.
- [x] **1.3 Database & Container Configuration**
  - Setup `docker-compose.yml` for SQL Server 2022 local container.
  - Configure local development connection strings and `.gitignore` entries.

---

## Phase 2: Domain Layer Modelling & Business Rules

- [x] **2.1 Manufacturer Entity & Invariants**
  - Properties: `Id` (Guid/int), `Name`, `HqCity`, `HqCountry`, `FoundedYear`, `Latitude`, `Longitude`, `Consoles` (collection).
  - Business rules:
    - Founded year must be reasonable (e.g., between 1800 and current year).
    - Coordinates must be within valid geo ranges (-90 to 90 lat, -180 to 180 lon).
- [x] **2.2 Console Entity & Invariants**
  - Properties: `Id`, `ManufacturerId`, `Name`, `ReleaseYear`, `Generation`, `UnitsSoldInMillions`, `IsDiscontinued`, `Manufacturer` (navigation).
  - Business rules:
    - Release year cannot predate the manufacturer's founding year.
    - Units sold must be non-negative ($\ge 0$).
    - Console name must be unique per manufacturer (case-insensitive check).
- [x] **2.3 Domain Exceptions & Invariant Enforcement**
  - Implement domain-specific exceptions (e.g., `InvalidDomainOperationException`, `EntityNotFoundException`, `DomainValidationException`).

---

## Phase 3: Infrastructure & Persistence Layer

- [x] **3.1 EF Core DbContext & Entity Configurations (Fluent API)**
  - Create `AppDbContext` adhering to `IAppDbContext` abstraction.
  - Configure `ManufacturerConfiguration` (table name, primary key, max lengths, index on Name).
  - Configure `ConsoleConfiguration` (foreign key with `DeleteBehavior.Restrict` to prevent cascade delete, composite unique index on `[ManufacturerId, Name]`).
- [x] **3.2 Database Migrations & Initial Schema**
  - Generate initial EF Core migration for SQL Server.
- [x] **3.3 Seed Data Generation**
  - Seed classic manufacturers with accurate HQ coordinates:
    - *Nintendo* (Kyoto, Japan - Founded 1889, Lat: 34.9699, Lon: 135.7563)
    - *Sega* (Tokyo, Japan - Founded 1960, Lat: 35.6264, Lon: 139.7402)
    - *Sony Interactive Entertainment* (Tokyo/San Mateo - Founded 1993, Lat: 37.5485, Lon: -122.3131)
    - *Atari* (Sunnyvale, USA - Founded 1972, Lat: 37.3688, Lon: -122.0363)
    - *Microsoft* (Redmond, USA - Founded 1975, Lat: 47.6423, Lon: -122.1391)
  - Seed classic consoles:
    - NES / Famicom (1983, Gen 3), Super Nintendo (1990, Gen 4), N64 (1996, Gen 5)
    - Master System (1985, Gen 3), Mega Drive / Genesis (1988, Gen 4), Dreamcast (1998, Gen 6)
    - PlayStation (1994, Gen 5), PlayStation 2 (2000, Gen 6)
    - Atari 2600 (1977, Gen 2)
    - Xbox (2001, Gen 6)

---

## Phase 4: Application Layer (Contracts, Use Cases & Validation)

- [x] **4.1 Data Transfer Objects (DTOs)**
  - Manufacturer DTOs: `ManufacturerDto`, `ManufacturerDetailDto`, `CreateManufacturerRequest`, `UpdateManufacturerRequest`.
  - Console DTOs: `ConsoleDto`, `CreateConsoleRequest`, `UpdateConsoleRequest`.
  - Map Location DTO: `ManufacturerMapLocationDto` (Id, Name, LocationString, Latitude, Longitude, ConsoleCount).
- [x] **4.2 Input Validation (FluentValidation)**
  - `CreateManufacturerRequestValidator` / `UpdateManufacturerRequestValidator` (required fields, year bounds, coordinate ranges).
  - `CreateConsoleRequestValidator` / `UpdateConsoleRequestValidator` (required fields, units sold non-negative, valid generation).
- [x] **4.3 Service Contracts & Implementation**
  - `IManufacturerService`: CRUD, deletion check (block deletion if active consoles exist).
  - `IConsoleService`: CRUD, domain rule checks (founding year vs release year, unique name under manufacturer).
  - `IMapService`: query optimized projection for map markers.

---

## Phase 5: RESTful API Layer & Cross-Cutting Concerns

- [x] **5.1 REST Endpoints**
  - `/api/manufacturers` (GET, GET by Id, POST, PUT, DELETE)
  - `/api/consoles` (GET with search & sorting params, GET by Id, POST, PUT, DELETE)
  - `/api/map/locations` (GET manufacturers with geo-coordinates and console counts)
- [x] **5.2 Error Handling & HTTP Status Standards**
  - Global Exception Handling Middleware / `ProblemDetails` (RFC 7807).
  - 200 OK / 201 Created / 204 No Content for successful flows.
  - 400 Bad Request for validation errors.
  - 404 Not Found for non-existent entities.
  - 409 Conflict / 422 Unprocessable Entity for domain rule violations (e.g. Deleting manufacturer with consoles, duplicate console name).
- [x] **5.3 OpenAPI / Swagger & CORS Configuration**
  - Configure Swagger UI for testing endpoints.
  - Enable CORS policy for local Vue 3 dev server.

---

## Phase 6: Frontend Development (Vue 3 + TypeScript)

- [x] **6.1 Layout, Navigation & Design System**
  - Responsive retro-themed or sleek modern dashboard layout with header navigation (Consoles, Manufacturers, Map View).
  - Toast/notification system for user feedback (success, errors, confirmation dialogs).
- [x] **6.2 API Client & Pinia Store Services**
  - Axios or Fetch wrapper with base URL, error interceptor, and typed responses.
  - Stores: `useConsoleStore`, `useManufacturerStore`, `useMapStore`.
- [x] **6.3 Consoles Management View**
  - Interactive table displaying console list with columns: Name, Manufacturer, Release Year, Generation, Units Sold, Status, Actions.
  - Server-side or client-side search by console/manufacturer name.
  - Column sorting (by Release Year, Name, Units Sold).
  - Create / Edit Console Modal / Form with client validation.
  - Delete Console with confirmation prompt.
- [x] **6.4 Manufacturers Management View**
  - List/Card view of manufacturers showing HQ and Console count.
  - Create / Edit Manufacturer Modal with coordinates input.
  - Delete Manufacturer with error handling display when consoles still exist.
- [x] **6.5 Interactive Map View (Option A)**
  - Integrate Leaflet map centering on global view with custom retro/marker pins.
  - Fetch `/api/map/locations` from backend.
  - Interactive popups showing:
    - Manufacturer Name
    - Location (City, Country)
    - Total Consoles Count
    - Quick link or list of consoles by that manufacturer.

---

## Phase 7: Automated Testing & Verification

- [x] **7.1 Unit Tests (Domain & Application)**
  - Validate domain business rules:
    - Rejection of console release year earlier than manufacturer founding year.
    - Rejection of negative units sold.
    - Blocking deletion of manufacturer when consoles exist.
  - Test FluentValidation rules with valid and invalid payloads.
- [x] **7.2 Integration Tests (API & Database)**
  - Use `WebApplicationFactory` with in-memory or Testcontainers SQL Server.
  - Test complete CRUD flows and correct HTTP status code returns.
- [x] **7.3 Frontend Sanity & E2E/Component Verification**
  - Verify form validations, table search/sort reactivity, and map marker click interactions.

---

## Phase 8: Documentation & Final Deliverables

- [x] **8.1 Comprehensive README.md**
  - Quickstart guide (prerequisites, how to run backend `.NET`, how to run `Vue 3` frontend, Docker SQL Server start).
  - Architecture overview and design decisions (Clean Architecture, SOLID choices).
  - Domain rules and trade-offs documented.
  - AI Usage disclosure section:
    - Tools used
    - What AI was used for (scaffolding, task planning, boilerplate)
    - What was manually verified and adjusted.
- [x] **8.2 Clean Code & Repository Review**
  - Check for no leftover temporary code, clean commit history, linting, and formatting.
