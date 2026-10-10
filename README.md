# MiniERP

ASP.NET Core Razor Pages application targeting .NET 10.

## Run locally

Install the .NET 10 SDK, then run from this directory:

```powershell
dotnet run --project src/MiniERP.Web/MiniERP.Web.csproj --launch-profile https
```

Open http://localhost:5048.
Open https://localhost:7017.

For HTTPS, use `--launch-profile https`. Trust the local development certificate with `dotnet dev-certs https --trust` if needed.

The solution is `../MiniERP.Web.slnx`. Open this solution in Visual Studio,
set MiniERP.Web as the startup project and select the http or https profile.
There is one ASP.NET Core Web project: `src/MiniERP.Web/MiniERP.Web.csproj`.

## Architecture: a simple modular monolith

The application is one deployment, one Web project, and one SQL Server database.
Business code is grouped by feature. Departments is the first implemented
feature; Accounts, Positions, Groups, Questions, and Exams are future work.

```text
src/MiniERP.Web/
  Features/
    Departments/
      DepartmentSummary.cs          # data returned to callers
      IDepartmentQueries.cs         # feature's public contract
      DepartmentQueries.cs          # read use case
      IDepartmentCommands.cs        # write contract
      DepartmentCommands.cs         # create/update/soft delete
      DepartmentModule.cs           # feature registration
  Domain/Entities/
    Department.cs                   # entity, normalized name
  Infrastructure/Persistence/
    AppDbContext.cs                 # shared EF Core context
    Configurations/
      DepartmentConfiguration.cs    # mapping to the existing SQL schema
  Extensions/
    DependencyInjectionExtensions.cs
  Pages/Departments/                # Razor UI; calls the feature contract
  Pages/Shared/                     # shared layout
  wwwroot/                          # browser assets
```

Domain holds entities; Infrastructure holds persistence mappings. Features
own use cases, contracts and returned DTOs. Keep entities independent of EF Core.
Pages handle HTTP input and rendering, without SQL queries or business rules.
Other features call a feature's public contracts and receive DTOs, rather than
querying its entities directly. DbContext belongs to Infrastructure and is used
by feature implementations; each implementation queries only its own entities.
Common should contain code genuinely shared by implemented features, rather
than empty exception handlers, pagination classes or speculative abstractions.

These boundaries are conventions inside one assembly; `internal` hides
implementation from external assemblies but cannot enforce isolation between
features in this project. Keep cross-feature dependencies explicit and review
them as new use cases are added. Separate module projects can enforce stricter
boundaries later if the application needs them.

Departments uses EF Core SQL Server and maps to `dbo.Department`, including
the filtered unique index and a query filter for `DeletedAt`. The list is
ordered by ID. SQL scripts currently own schema initialization;
the app does not run EnsureCreated, migrations or seeding on startup. When
introducing EF migrations, first establish a baseline for the existing schema
so migrations do not try to recreate these tables.

### Department CRUD

Visit `/Departments` for the list, then use Create, Details, Edit and Delete.
Names are required, trimmed and limited to 30 characters. Active names must
be unique using the database collation. Both a precheck and the unique index
protect against duplicate names, including concurrent submissions.
Successful POSTs redirect to avoid resubmitting the write on refresh. Razor
forms enforce antiforgery tokens, and input models bind only editable fields.
Unknown or deleted IDs return 404. Delete is confirmed by POST and sets
`DeletedAt` in UTC, preserving Account foreign keys; deleted names can be reused.
Edits update only active rows, so stale forms cannot revive a deleted department.
Concurrent name edits currently use last-write-wins; no rowversion is present
in the existing schema. This development application does not yet implement
authentication or permissions.

HTTP regression checks are in `tests/department-crud.ps1`. Run them against
a separate, disposable database with the same Department table/index and a
fixture named `CRUD existing` (ID 1) referenced by another table. Configure a
test Web process to use that database, then run:

```powershell
./tests/department-crud.ps1 -BaseUri http://localhost:5059 -IsolatedDatabase
```

The script writes data and soft-deletes its fixture, so do not point it at
your normal TestingSystem database. It checks validation, CRUD, duplicate and
concurrent submissions, antiforgery, missing IDs, stale edits and name reuse.

## SQL Server with Docker Compose

Start Docker Desktop with Linux containers. From this directory:

```powershell
Copy-Item .env.example .env
# Edit .env and replace MSSQL_SA_PASSWORD with your own strong password.
docker compose up -d
docker compose logs -f database-init
```

SQL Server 2022 Developer starts first. Once healthy, `database-init` runs
`database/SQLServer/01_init_data.sql` followed by `02_insert_data.sql` to create
and seed `TestingSystem`. Successful initialization ends with exit code 0.
Check both services with `docker compose ps -a`.

Both services use the SQL Server image directly; no Dockerfile or image build
is required. Initialization commands are embedded in Compose, which mounts
the database SQL files.

The schema and seed run in one transaction. If a script fails, inspect the logs,
fix the script, then retry with `docker compose run --rm database-init`.
After success, a database marker prevents rerunning the scripts and inserting
duplicate data on later starts. Changes to these scripts are not migrations
and are not automatically applied to an initialized database.

Connect from SSMS or another local client using:

- Server: `localhost,1433` (or the port configured in `.env`)
- Authentication: SQL Server Authentication
- User: `sa`
- Password: `MSSQL_SA_PASSWORD` from `.env`
- Database: `TestingSystem`
- Enable Trust Server Certificate for this local development instance.

Example .NET connection string (replace the password):

```text
Server=tcp:127.0.0.1,1433;Database=TestingSystem;User Id=sa;Password=<your-password>;Encrypt=True;TrustServerCertificate=True;
```

## Connect the Web application

Compose reads `.env` for container configuration. ASP.NET Core does not load
this file automatically. Store its own connection string using user secrets
(replace the password and port to match `.env`):

```powershell
dotnet user-secrets set "ConnectionStrings:TestingSystem" "Server=tcp:127.0.0.1,1433;Database=TestingSystem;User Id=sa;Password=<your-password>;Encrypt=True;TrustServerCertificate=True;" --project src/MiniERP.Web/MiniERP.Web.csproj
```

Alternatively, set `ConnectionStrings__TestingSystem` in the environment of
the Web process. Do not commit the real password to appsettings or source code.
User secrets are loaded in Development, including the Visual Studio profiles.
Use the explicit IPv4 address above because Compose publishes SQL Server only
on `127.0.0.1`; `localhost` can resolve to IPv6 on some hosts.
In Development, the Web application also normalizes a SQL Server host of
`localhost` to `tcp:127.0.0.1`, preserving its configured port. Restart the Web
process after changing connection settings so the current process uses them.

Start the database, check `database-init` completed, then run the Web project.
Visit `/Departments` to read the seeded departments through the feature and
EF Core. Home and Privacy can run without a configured database; Departments
requires the connection string and a reachable SQL Server.

`docker compose down` stops the services and preserves data in the named volume.
To intentionally delete all database data and initialize from scratch:

```powershell
docker compose down -v
docker compose up -d
```

Do not commit `.env`. The checked-in `.env.example` contains only an example
development password. The published port is restricted to localhost.
