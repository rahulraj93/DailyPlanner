# DailyPlanner

One app to plan every part of life: time, study, gym, meals, money, family, travel and events, with an AI coach that reshapes the plan around the user's goals. Web app first; desktop and mobile later. Built by one developer (C# / SQL Server background), so keep things simple and boring.

A working HTML prototype (v0.3) exists and defines the features and data shapes. Treat it as the spec for behaviour, not as code to port.

## Architecture decisions (settled)

- **Modular monolith.** One ASP.NET Core app, one container, one Azure SQL database. No microservices for now.
- **Five modules** with hard boundaries: `Planner`, `Life`, `Finance`, `Notify`, `AiCoach`.
- **Web client:** Blazor WebAssembly, installable (PWA), hosted by the same ASP.NET Core app.
- **Database:** Azure SQL (SQL Server locally). One schema per module: `planner`, `life`, `finance`, `notify`, `ai`.
- **Hosting target:** Azure Container Apps (single app). Not needed until the beta.
- **.NET:** latest LTS (.NET 10). Nullable enabled, warnings as errors.

## Solution layout

```
DailyPlanner.sln
Directory.Build.props            # nullable, implicit usings, warnings as errors, LangVersion latest
Directory.Packages.props         # central package management
src/
  DailyPlanner.Api/              # host: composes modules, auth, Minimal API endpoints, serves the Blazor app
  DailyPlanner.Web/              # Blazor WebAssembly client (PWA)
  DailyPlanner.SharedKernel/     # Entity base, Result type, IDomainEvent, IEventBus, IClock, ICurrentUser
  Modules/
    Planner/
      DailyPlanner.Planner/            # internal: domain, application, EF Core (PlannerDbContext), endpoints
      DailyPlanner.Planner.Contracts/  # public: interfaces, DTOs, integration events
    Life/      (same two-project shape)
    Finance/   (same two-project shape)
    Notify/    (same two-project shape)
    AiCoach/   (same two-project shape)
tests/
  DailyPlanner.ArchitectureTests/  # enforces the module rules below
  DailyPlanner.Planner.Tests/      # and one test project per module as it grows
```

Each module exposes one registration method, e.g. `services.AddPlannerModule(config)` and `app.MapPlannerEndpoints()`, called from `DailyPlanner.Api`.

## Module rules (enforced by architecture tests)

1. A module may reference another module's `*.Contracts` project only, never its internal project.
2. A module never reads or writes another module's tables or `DbContext`.
3. Cross-module communication = a call through a Contracts interface, or an in-process integration event published on `IEventBus`.
4. Each module has its own `DbContext`, `HasDefaultSchema("<schema>")`, and its own migrations history table in that schema.
5. Endpoints live in the module and are grouped under `/api/<module>/...`.
6. Internal types are `internal`; only Contracts types are `public`.

Do not add MediatR or MassTransit. A small hand-written `IEventBus` (in-memory, dispatches to `IIntegrationEventHandler<T>` from DI) is enough.

## Data conventions (ready for local-first sync later)

- Primary keys: `Guid` (version 7, time-ordered).
- Every user-owned row has `UserId`, `CreatedAt`, `UpdatedAt`, and `DeletedAt` (soft delete). Sync will rely on `UpdatedAt` and tombstones later.
- All queries are filtered to the current user (global query filter via `ICurrentUser`).
- Times: store dates as `DateOnly`, times of day as `TimeOnly`, instants as UTC `DateTimeOffset`. A block ending at midnight is stored as `00:00` and means end of day.
- Money: `decimal(18,2)` plus a currency code; finance amounts and body-weight values will be encrypted at field level later, so keep them in clearly named columns.
- Never store: card numbers, bank logins, plain passwords, full AI chat transcripts.

## Domain model by module (from the prototype)

**Planner** (`planner` schema)
- `LifeArea`: id, name, colour, isEnabled, sortOrder. Seeded: Routine, Work, Study, Gym & fitness, Meals & diet, Entertainment, Finance, Family time, Travel & trips, Events.
- `RoutineBlock`: the user's usual week. dayOfWeek, start, end, title, lifeAreaId, note.
- `WeekPlan`: a dated override of the routine for one week (weekStart = Monday). Has many `PlannedBlock` (same fields as RoutineBlock plus dayOfWeek).
- `BlockCompletion`: date + blockId ticked done.
- `Goal`: title, lifeAreaId, dueDate, progress 0 to 100, why.
- `Habit` + `HabitLog` (habitId, date).
- `FocusArea`: area, lifeAreaId, current, target, unit, lowerIsBetter, note.

**Life** (`life` schema)
- Meals: `MealSlot` (name, time), `MealPlanEntry` (dayOfWeek, slotId, text), `WaterLog` (date, glasses, goal), `GroceryItem` (text, isDone).
- Gym: `Workout` (name) with `Exercise` (name, sets, reps text), `GymSplit` (dayOfWeek, workoutId nullable), `WorkoutSession` (date, workoutId) with `SessionEntry` (exerciseId, weightKg), `BodyWeightLog` (date, kg).
- Family: `FamilyIdea` (title, type home|outdoor|trip, cost $|$$|$$$, status idea|planned|done, date).
- Travel: `Trip` (name, destination, start, end, budget, status idea|booked|done, itinerary text) with `TripChecklistItem` (kind packing|booking, text, isDone).
- Events: `Event` (title, date, time, type exam|family|bill|holiday|birthday|appointment|other, repeat none|weekly|monthly|yearly, remindDaysBefore). Publishes `EventDue`.

**Finance** (`finance` schema)
- `FinanceSettings` (currency, monthlyIncome), `BudgetCategory` (name, monthlyAmount), `Expense` (date, amount, budgetCategoryId, note), `SavingsGoal` (name, target, saved). Publishes `ExpenseAdded`, `BudgetExceeded`.

**Notify** (`notify` schema)
- `ReminderSchedule`, `NotificationLog`. Listens to `EventDue`, `BudgetExceeded`. Background work runs as a hosted service.

**AiCoach** (`ai` schema)
- `AiUsage` (userId, date, requestCount) for daily caps. Builds prompts from Contracts queries of other modules; calls the AI provider through one `IAiClient` interface. Stores results only when the user applies them, never transcripts.

## Auth

Start with a development auth handler that signs in a fixed test user, behind `ICurrentUser`. Real sign-in will be an external provider (JWT) later; nothing outside `DailyPlanner.Api` should know which one.

## Build order

1. **Skeleton**: solution, all projects, references, Directory.*.props, empty module registrations, architecture tests passing, `dotnet build` and `dotnet test` green.
2. **Planner module end to end**: LifeArea, RoutineBlock, WeekPlan, Goal, Habit, FocusArea; EF Core migrations; Minimal API endpoints; integration tests against SQL Server (Testcontainers).
3. **Blazor client**: Today and Week screens against the Planner API.
4. **Life module**, then **Finance**, **Events/Notify**, then **AiCoach**.
5. Real auth, deploy to Azure Container Apps, invite beta testers.

## Working agreements

- Small, reviewable steps. Build and run tests after every step and report results.
- Ask before adding any new NuGet package or external service.
- No business logic in `DailyPlanner.Api` beyond composition, auth and middleware.
- Prefer plain, readable C# over clever abstractions. No repository-over-EF wrappers.
- Out of scope for now: microservices, mobile/desktop apps, payments, offline sync.
