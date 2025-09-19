---
applyTo: '**'
---
---
applyTo: '**'
---
# COPILOT\_INSTRUCTIONS.md

**Purpose:** Steer GitHub Copilot to generate production-grade C#/.NET code that is clean, testable, secure, observable, and performance-aware.
**Scope:** .NET 8+, ASP.NET Core, EF Core, minimal APIs or Controllers, Worker Services, Azure.

---

## 0) Global Directives

* Prefer clarity over cleverness. Generate code that is explicit, readable, and maintainable.
* Default to .NET 8 features. Enable `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`.
* Always compile with analyzers and warnings as errors. Treat analyzer messages as acceptance criteria.
* Every public type and method needs XML docs if part of a reusable package. Else add concise summary comments where helpful.
* Always show full namespaces in `using` directives, no wildcard usings.
* Avoid magic values. Extract to `const`, `static readonly`, options, or enums.
* All I/O, network, and DB calls are async. Provide `CancellationToken` opt-in parameters and pass them through.
* Prefer minimal external dependencies. Suggest BCL first.

---

## 1) Solution Layout and Naming

* Solution structure:

  * `src/Company.Project.Api`
  * `src/Company.Project.Application` (use-cases, CQRS handlers, ports)
  * `src/Company.Project.Domain` (entities, value objects, domain services, specifications)
  * `src/Company.Project.Infrastructure` (EF Core, implementations, messaging)
  * `tests/Company.Project.*.Tests` (unit), `tests/Company.Project.*.IntegrationTests` (integration)
* Names:

  * Projects and namespaces: `Company.Project.[Layer]`
  * Folders: `Controllers`, `Endpoints`, `Contracts`, `Entities`, `ValueObjects`, `Services`, `Repositories`, `Specifications`, `Policies`, `Options`, `Mappings`, `Configurations`.
  * Async suffix `Async` on methods that return `Task`/`Task<T>`.

---

## 2) Coding Style (use `.editorconfig`)

* Indentation: 4 spaces. Braces on new lines. File-scoped namespaces.
* Ordering: `using` directives sorted and outside namespace.
* Access modifiers explicit. Prefer `readonly` for fields. Use `sealed` where inheritance not intended.
* Avoid `var` when the type is not obvious. Use `var` for `new()` with explicit RHS type.
* Expression-bodied members allowed for one-liners only.
* Pattern matching over `as` + null checks. Prefer switch expressions for compact decision logic.
* Prefer `record` for immutable DTOs/VOs. Use `required` members where appropriate.

---

## 3) Nullability and Defensive Coding

* Enable nullable reference types. No `!` null-forgiving unless justified in comment.
* Validate public API parameters. Throw `ArgumentNullException.ThrowIfNull(param)` or return `ValidationProblem`.
* Avoid returning `null` collections. Return empty arrays/lists.
* Use value objects to encapsulate invariants instead of scattered validation.

---

## 4) Async/Await and Concurrency

* Use `await` end-to-end. No `Task.Result`/`.Wait()`. No `async void` except event handlers.
* Accept and pass `CancellationToken` down. Honor it in loops and long operations.
* Avoid unnecessary `ConfigureAwait(false)` in ASP.NET Core. Use it in libraries.
* Use `IAsyncEnumerable<T>` for streams. Buffer only when needed.
* For parallel work use `Parallel.ForEachAsync` or `Task.WhenAll` with bounded concurrency via `SemaphoreSlim`.
* Prefer channels or Dataflow for producer/consumer. Guard shared state with immutability or proper synchronization.

---

## 5) Exceptions and Error Handling

* Exceptions are exceptional. Control flow via results, not exceptions.
* Throw narrow exception types. Include context in `Message` and `Data`.
* Do not swallow exceptions. Catch only to add context, translate, or compensate.
* APIs return RFC 7807 ProblemDetails on errors. Include `traceId`.
* Map domain errors to 400/404/409. Unexpected → 500 with correlation id only.

---

## 6) Logging and Observability

* Use `ILogger<T>`. Structured logging with named properties: `logger.LogInformation("User {UserId} created order {OrderId}", userId, orderId);`
* No PII in logs. Hash or redact.
* Log levels: Debug (dev only), Information (state change), Warning (retriable/anomaly), Error (failed operation), Critical (system down).
* Correlate with `traceparent`/`RequestId`. Use OpenTelemetry for traces and metrics.
* Emit business metrics via `IMeter` (System.Diagnostics.Metrics). Prefix meters `Company.Project.*`.

---

## 7) Configuration and Options

* Use `IOptionsSnapshot<T>` for scoped services in web, `IOptionsMonitor<T>` for singletons.
* Validate options with `services.AddOptions<T>().Bind(...).ValidateDataAnnotations().Validate(...)`.
* No direct access to `IConfiguration` in business logic. Map to typed options.
* Store secrets in environment/KeyVault. Do not check them into source.

---

## 8) Dependency Injection and Lifetime

* Register concrete services with correct lifetimes: stateless services → `Singleton`, data access and domain services → `Scoped`, transient logic → `Transient`.
* No service locator. Inject required dependencies via constructor. Use `Guard Clause` for nulls.
* Split registration per feature in extension methods: `services.AddOrders()`.

---

## 9) Domain, DDD, and Boundaries

* Keep domain pure. No EF Core or HTTP types in Domain.
* Use value objects for concepts like `Money`, `Email`, `Quantity`. Make them immutable and validated.
* Aggregate roots enforce invariants. Mutations via methods, not property setters.
* CQRS: Commands mutate, Queries read. No query in command handler and vice versa.
* Use Specifications for complex querying logic in repositories.

---

## 10) API Design (REST/Minimal APIs/Controllers)

* Resource naming plural, kebab-case: `/api/v1/orders/{id}`
* Support pagination (`page`, `pageSize`), filtering, sorting. Return `Link` headers where useful.
* Idempotency keys for POST that creates resources. Upserts only when explicit.
* Consistent response envelope for lists: `{ items, total, page, pageSize }`
* Use validation responses with field errors. For invalid model state return `ValidationProblem()`.
* Version via URL (`/v1/`) or header. No breaking changes in minor versions.
* Use `ETag`/`If-Match` for concurrency where applicable.

---

## 11) Contracts and Mapping

* Define request/response DTOs in `Contracts` folder. No domain types leaking to API.
* Serialization: `System.Text.Json` with snake\_case or camelCase consistently. Configure required converters once.
* Mapping: Prefer manual mapping for critical paths. If AutoMapper used, profiles per feature. No implicit magic. Test maps.

---

## 12) Validation

* Use FluentValidation for DTOs and commands. Register validators in DI.
* Keep validation messages precise. Provide limits and expected format.
* Validate cross-field rules in validators, invariants in domain.

---

## 13) EF Core and Data Access

* Use DbContext per request scope. No long-lived contexts.
* Enable `AsNoTracking()` for read queries. Track only when modifying.
* Explicit includes or projection. Avoid N+1 by projecting to DTO.
* Transactions via `DbContext.Database.BeginTransactionAsync` or `ExecutionStrategy` with retries.
* Migrations checked in. No destructive changes without plan. Use idempotent scripts for CI/CD.
* Indexes and constraints defined via Fluent API. Avoid shadow state unless needed.
* Use `DateTimeOffset` for timestamps. Store in UTC. Add `rowversion` for optimistic concurrency.
* Connection resiliency and timeout configured. Parameterize queries. No raw SQL unless necessary.

---

## 14) Messaging, Outbox, and Idempotency

* Use outbox pattern for exactly-once publication. Store event with aggregate changes within same transaction.
* Messages include `MessageId`, `CorrelationId`, `CausationId`, `OccurredAt`.
* Consumers idempotent. Persist processed message ids.
* Retry with backoff and circuit breakers (Polly). Poison queue after max retries.

---

## 15) Resilience

* Wrap external calls in policies: timeout, retry with jitter, circuit breaker, fallback, bulkhead.
* Bound retries. Mark operations idempotent before retrying.
* Use `HttpClientFactory` with named/typed clients. Configure handler pipelines.

---

## 16) Security

* Authentication via OpenIddict/IdentityServer/OAuth2 OIDC. Validate audience, issuer, lifetime.
* Authorization via policies and claims. No role checks in controllers. Prefer resource-based handlers.
* Validate all inputs. Use model binding + FluentValidation. Encode outputs by default.
* Secrets from environment or secret manager only. No secrets in logs or exceptions.
* For cryptography use `RandomNumberGenerator`, `AesGcm`, `RSA` from BCL. No homegrown crypto.

---

## 17) Caching

* Two-layer caching: in-memory and distributed (Redis) where needed.
* Cache keys versioned. Include tenant/user scoping where needed.
* Set sensible TTLs. Invalidate on writes or use cache-aside pattern.
* Serialize with `System.Text.Json`. Avoid large object graphs.

---

## 18) Time, Clocks, and Randomness

* Use `DateTimeOffset.UtcNow`. Abstract time via `IClock` for testability.
* Avoid `Thread.Sleep`. Use `Task.Delay` with cancellation.
* Inject RNG as an interface when deterministic behavior is needed.

---

## 19) Files, Streams, and Serialization

* Use async stream APIs. Avoid reading entire files into memory.
* Set `JsonSerializerOptions` centrally. Use `JsonSerializerContext` source generation for hot paths.
* For CSV/Excel prefer streaming writers. Validate input schema.

---

## 20) Performance Guidelines

* Prefer allocation-free patterns on hot paths. Consider `Span<T>`, `ReadOnlySpan<T>`, pooling with `ArrayPool<T>`.
* Avoid LINQ in tight loops. Favor simple loops when profiling shows impact.
* Precompute regex with `RegexOptions.Compiled`. Prefer `GeneratedRegex` in .NET 8.
* Use `Stopwatch.GetTimestamp()` for precise timing when needed.
* Benchmark critical code with BenchmarkDotNet before micro-optimizing.

---

## 21) Source Generators and AOT

* Prefer source generators for repetitive code (contracts, DI, mapping) when complexity pays off.
* Validate AOT readiness for worker apps. Avoid unsupported reflection patterns.

---

## 22) Testing Strategy

* Testing pyramid: unit > integration > E2E.
* Use xUnit + FluentAssertions. Name tests: `MethodName_Should_DoX_When_Y`.
* One assertion concept per test. Arrange-Act-Assert.
* Mock only external dependencies. Avoid over-mocking.
* Integration tests spin up minimal host with Testcontainers for DB and brokers.
* Contract tests for public APIs. Snapshot test only stable payloads.
* Seed minimal data. Clean state per test. Parallelize where safe.

---

## 23) Feature Toggles and Backward Compatibility

* Use typed feature flags. Default off for risky features. Gate behavior at edges.
* Maintain backward compatible DTOs. Add not remove fields. Use defaulting on deserialization.
* Blue/green or canary deploys for risky changes.

---

## 24) Build, CI/CD, and Quality Gates

* Pipeline steps: restore → build → test (unit, integration) → analyze → package → deploy.
* Treat warnings as errors. Failing tests block merges.
* Run `dotnet format`, StyleCop, Sonar or equivalent on PR.
* Produce SBOM. Sign artifacts. Supply chain policy enforced.

---

## 25) Git and PR Discipline

* Commit messages: `<scope>: <imperative summary>` with short body and issue link.
* Small PRs. Include design rationale in description. Link to ADR if architecture changes.
* PR checklist:

  * [ ] Tests added/updated
  * [ ] Logging correct level and structured
  * [ ] CancellationToken plumbed
  * [ ] Options validated
  * [ ] Nullability satisfied
  * [ ] Docs and samples updated

---

## 26) Documentation and ADRs

* Keep `README.md` per service with purpose, run instructions, env vars, ports.
* Use ADRs for significant decisions (pattern, library, data model).
* Keep API docs via OpenAPI. Examples for each endpoint.

---

## 27) Sample Templates

### 27.1 Minimal API Endpoint (with validation, logging, problem details)

```csharp
app.MapPost("/api/v1/orders", async (
    CreateOrderRequest req,
    IValidator<CreateOrderRequest> validator,
    IMediator mediator,
    ILoggerFactory lf,
    CancellationToken ct) =>
{
    var log = lf.CreateLogger("Orders.Create");
    var vr = await validator.ValidateAsync(req, ct);
    if (!vr.IsValid) return Results.ValidationProblem(vr.ToDictionary());

    var cmd = req.ToCommand();
    var result = await mediator.Send(cmd, ct);

    log.LogInformation("Order {OrderId} created for {UserId}", result.OrderId, result.UserId);
    return Results.Created($"/api/v1/orders/{result.OrderId}", result);
})
.Produces<CreateOrderResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError);
```

### 27.2 Options Pattern with Validation

```csharp
builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection("Storage"))
    .ValidateDataAnnotations()
    .Validate(o => o.MaxItems > 0, "MaxItems must be positive")
    .ValidateOnStart();
```

### 27.3 Typed HttpClient with Resilience

```csharp
builder.Services.AddHttpClient<IWeatherClient, WeatherClient>(c =>
{
    c.BaseAddress = new Uri(cfg.Weather.BaseUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(PollyPolicies.RetryWithJitter)
.AddPolicyHandler(PollyPolicies.CircuitBreaker);
```

### 27.4 EF Core Query Best Practice

```csharp
var dto = await _db.Orders
    .AsNoTracking()
    .Where(o => o.UserId == userId)
    .OrderByDescending(o => o.CreatedAt)
    .Select(o => new OrderDto(o.Id, o.Total, o.Status))
    .ToListAsync(ct);
```

---

## 28) Analyzer and Ruleset Baseline

* Enable:

  * Microsoft.CodeAnalysis.NetAnalyzers
  * StyleCop.Analyzers
  * Meziantou.Analyzer
  * SecurityCodeScan
* Severity:

  * API surface and nullability: error
  * Async usage and blocking calls: error
  * Naming and style: warning or error on hot paths
* Block merges on analyzer violations.

---

## 29) Prohibited Patterns

* Blocking on async code.
* Static mutable state in web apps.
* Swallowing exceptions or returning raw exception messages.
* Business logic in controllers/endpoints.
* Leaking EF entities through API.
* Excessive inheritance. Prefer composition.
* Sharing DbContext across threads.
* Hard-coded connection strings or secrets.

---

## 30) Acceptance Checklist for Generated Code

* [ ] Compiles with warnings as errors
* [ ] Async and cancellation correctly implemented
* [ ] Logging is structured and non-PII
* [ ] Validation present and effective
* [ ] Nullability satisfied without `!`
* [ ] Tests exist and pass
* [ ] Options validated; config not read in core logic
* [ ] No prohibited patterns detected
* [ ] Observability integrated (tracing/metrics where relevant)
* [ ] Docs or summaries updated

---

## 31) Default Project SDK Settings (snippet)

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsAsErrors>CS4014;CS1998;CS8600;CS8602;CS8618</WarningsAsErrors>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Http.Polly" Version="8.*" />
  <PackageReference Include="FluentValidation" Version="11.*" />
  <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.*" />
  <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
</ItemGroup>
```

---

## 32) `.editorconfig` (snippet)

```
root = true

[*.cs]
indent_style = space
indent_size = 4
csharp_new_line_before_open_brace = all
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
dotnet_style_qualification_for_field = true:suggestion
dotnet_style_qualification_for_property = true:suggestion
dotnet_style_require_accessibility_modifiers = always:warning
dotnet_diagnostic.CA1062.severity = error
dotnet_diagnostic.CA2007.severity = error
dotnet_diagnostic.SA0001.severity = warning
```

---

## 33) Final Guidance to Copilot

* If uncertain, generate the simplest correct version with hooks for extension.
* Prefer small, composable functions over large monoliths.
* Include TODOs only with exact follow-ups and owners.
* Always provide tests or test scaffolds with new logic.
* Propose refactors only with measurable benefits or reduced risk.

---

## 34) Testing — Dedicated Section

### 34.1 General Principles

* Tests are first-class citizens. Same coding standards as production code.
* Tests must be **deterministic**, isolated, and idempotent. No flaky behavior tolerated.
* Follow the pyramid: **70% unit, 20% integration, 10% end-to-end**.
* Each test documents behavior. Treat failing tests as broken functionality, not “just a test issue.”

### 34.2 Structure and Naming

* Projects in `tests/Company.Project.[Layer].Tests`.
* Mirror production namespaces and folder structures in test projects.
* Class naming: `[ClassName]Tests` or `[Feature]Scenarios`.
* Method naming convention:
  `MethodName_Should_ExpectedBehavior_When_Precondition`.
  Example: `CreateOrder_Should_ReturnOrderId_When_ValidRequest`.

### 34.3 Unit Tests

* Test smallest unit of logic (domain entity, value object, service method).
* No external resources (DB, network, file). Use pure memory.
* Assertions with **FluentAssertions** for readability.
* Use parameterized `[Theory]` for combinatorial cases.
* Mocks/Stubs only for true dependencies. Prefer fakes for simple contracts.
* Cover:

  * Happy path
  * Boundary conditions
  * Exceptional/invalid input cases

### 34.4 Integration Tests

* Spin up service with `WebApplicationFactory<T>` or `MinimalApiFactory`.
* Use **Testcontainers** for DB, queues, external services. No reliance on developer machine state.
* Reset environment between runs. Migrations applied automatically.
* Verify real EF Core queries, DI registration, and serialization.
* Use actual HTTP calls against in-memory server.
  Example: `await _client.PostAsJsonAsync("/api/v1/orders", request);`

### 34.5 End-to-End Tests

* Minimal set for core flows: user signup, login, order lifecycle.
* Use dedicated test environments, not production.
* Automate in CI/CD but allow smoke-only mode for fast pipelines.
* Prefer Playwright/Selenium for UI + API orchestration.

### 34.6 Performance/Load Testing

* Benchmark critical methods with BenchmarkDotNet. Keep results versioned.
* For APIs, use k6 or Locust for realistic load.
* Establish baseline SLAs (latency, throughput, error rate). Fail builds if exceeded.

### 34.7 Code Coverage

* Minimum **80% line coverage**, but never chase % at cost of quality.
* Critical domain and infrastructure code must be fully covered.
* Track coverage via Coverlet integrated with CI.
* Do not exclude files unless explicitly documented.

### 34.8 Test Data

* Deterministic, minimal, explicit. No random values unless seeded.
* Use Builders or ObjectMothers for readability.
  Example: `new OrderBuilder().WithCustomer("CUST-123").Build();`
* Avoid deep graphs unless required. Keep focus on behavior.

### 34.9 Assertions

* One conceptual assertion per test. Use FluentAssertions chaining if multiple checks are related.
* Always assert the expected outcome, not just absence of exception.
* For async code: `await act.Should().ThrowAsync<...>()`.

### 34.10 CI/CD Requirements

* All tests run in CI. No skipping without documented reason.
* Fast feedback: unit tests under 1 second each. Integration suite under 5 minutes.
* Flaky tests automatically quarantined and tracked until resolved.

### 34.11 Test Smells to Avoid

* Overuse of mocks → brittle tests.
* Asserting implementation details (e.g., number of calls). Prefer outcome-based tests.
* Large “god tests” with multiple scenarios. Split per case.
* Sleeping or arbitrary delays. Use proper async waits.
* Hidden dependencies (global state, static classes).

### 34.12 Golden Rules

* A test that doesn’t fail is not a test. Verify it fails when code is wrong.
* A test without assertion is useless.
* Every bug fixed must have a regression test.
* Every new feature merged must have accompanying tests.

---

## 35) API Contracts, Versioning, and Deprecation

* OpenAPI required. Every endpoint documented with request/response examples.
* Version via URI `/v{n}`. Backward compatible within major. Add fields; do not remove.
* Deprecation policy: mark in OpenAPI, add `Deprecation` header, provide EOL date, migration notes.
* SDKs generated from OpenAPI. Regenerate on contract change. Version SDKs semantically.

## 36) Error Catalog

* Standard ProblemDetails fields. `type`, `title`, `status`, `traceId`, `errors`.
* Organization-wide error codes: `DOMAIN-CATEGORY-NNN`. Single source of truth in repo.
* Map: validation→400, auth→401/403, not found→404, conflict→409, upstream timeout→504.
* Never leak stack traces or connection info.

## 37) Pagination, Sorting, Filtering

* Query params: `page`, `pageSize` (1–200 default 50), `sort` (`field:asc|desc`), `filter` (RSQL or explicit keys).
* List envelope: `{ items, total, page, pageSize }`. Include `Link` headers for nav.

## 38) Rate Limiting and Throttling

* Token bucket per API key/user/IP. Default 100 req/min. Configurable per route.
* Return `429` with `Retry-After`. Log with dimension `LimitName`.

## 39) Security Hardening

* HTTPS only. HSTS. SameSite=Lax cookies. `X-Content-Type-Options: nosniff`.
* JWT validation: issuer, audience, signature, expiry, clock skew ≤ 2m.
* Input validation on all external boundaries. Output encoding by default.
* Keys in KeyVault/Secret Manager. Rotation policy documented. Short TTL access tokens.
* Audit trail for authz decisions and sensitive mutations.

## 40) Privacy, PII, and Data Retention

* Tag fields as `PII` or `Sensitive`. Mask in logs, traces, and non-prod data.
* Data retention per entity. Scheduled purges. Right to erasure supported.
* Export data in portable JSON/CSV upon request. Include schema.

## 41) Multi-Tenancy

* Tenant resolution order: header `X-Tenant-Id` → token claim → subdomain.
* All persistence scoped by tenant key. Enforce via global query filter/specification.
* Cache keys and metrics labeled with tenant. No cross-tenant joins.

## 42) Observability SLOs and Alerts

* SLIs: latency p95, error rate, saturation, queue depth.
* SLOs: define per service (e.g., `POST /orders p95 ≤ 200ms, error ≤ 0.5%`).
* Alerts: page on SLO burn rate > 2x, queue lag > 5m, circuit open > 1m.
* Trace attributes: `tenantId`, `userId`(hashed), `correlationId`, `feature`.

## 43) Health, Readiness, and Diagnostics

* `/health/live` no deps. `/health/ready` checks DB, cache, brokers.
* Diagnostics mode flag to enable extended self-checks; disabled in prod by default.
* Startup/Shutdown hooks log reasons and durations.

## 44) Background Work, Sagas, and Scheduling

* Use durable storage for jobs (Hangfire/Quartz). Idempotent handlers. Pass `CancellationToken`.
* Sagas: model state explicitly. Compensation steps defined and tested.
* Cron jobs defined in code with feature flags. Time zone = UTC. Missed runs handled.

## 45) Messaging Contracts and Versioning

* Message schema immutable once published. Add new fields as optional.
* Include `messageId`, `correlationId`, `causationId`, `occurredAt`.
* Use schema registry (Avro/Proto/JSON Schema). Validate on publish/consume.
* Dead-letter with reason. Quarantine analyzer runs daily.

## 46) Data Access Advanced

* Connection pool limits set. Command timeout defaults: reads 15s, writes 30s.
* Use `rowversion` or concurrency tokens. Conflict returns 409 with recovery guidance.
* Soft deletes only with explicit requirement. Filtered indexes for soft-deleted data.

## 47) Performance Budgets

* Endpoint budgets: CPU ≤ X ms p95, allocations ≤ Y KB per request.
* Hot paths avoid LINQ allocations; prefer projections. Use `GeneratedRegex`, `Span<T>`.
* Add BenchmarkDotNet projects for hot components. Track baselines in CI.

## 48) Docker and Runtime Standards

* Distroless or `mcr.microsoft.com/dotnet/aspnet:8.0` runtime. Multi-stage build. Non-root user.
* Expose only required ports. Health endpoints wired to k8s probes.
* Resource requests/limits set. Fail fast on OOM. Graceful shutdown ≤ 30s.

## 49) Kubernetes, Helm, and Config

* One chart per service. Values for env overrides only. No logic in templates.
* Config from env vars. No k8s secrets in ConfigMap. Use Secret objects + CSI.
* PodDisruptionBudget, HPA with CPU+RPS or custom metric.

## 50) Blue/Green, Canary, and Rollback

* Traffic shifting via header or percentage. Automated rollback on error budget burn.
* Database changes forward-compatible. Two-phase deploy for breaking schema changes.

## 51) Environments and Parity

* Env tiers: `dev`, `test`, `staging`, `prod`. Feature flags consistent across tiers.
* Non-prod uses masked data. No prod secrets in non-prod. Same build artifact across tiers.

## 52) Contract and Consumer-Driven Tests

* Pact/contract tests for public APIs and event streams.
* Build fails if provider breaks consumer contracts. Contracts versioned with code.

## 53) Seed, Fixtures, and Demo Data

* Deterministic seeds behind a flag. Never auto-seed in prod.
* Demo datasets separate from migrations. Clearly labeled.

## 54) Tooling and Static Analysis

* Analyzers: NetAnalyzers, StyleCop, Meziantou, SecurityCodeScan, Nullable Ref Types.
* Treat selected warnings as errors: blocking on async, nullability, disposed objects, insecure crypto.
* Secret scanning enabled. SBOM generated. SCA enforced.

## 55) Source Control and Branch Protection

* `main` protected: required reviews, passing checks, linear history. No direct pushes.
* Conventional commits: `feat:`, `fix:`, `refactor:`, `perf:`, `docs:`, `test:`, `build:`, `ci:`, `chore:`.
* CODEOWNERS for critical areas. PR template with risk and rollback plan.

## 56) Packaging and Versioning

* Semantic Versioning for services and NuGet packages.
* Package metadata includes repository URL, license, description, source link.
* Sign packages. Publish from CI only.

## 57) Documentation and Runbooks

* Each service has: purpose, endpoints, env vars, dependencies, limits, SLOs, on-call runbook.
* Runbook includes common failures, dashboards, remediation steps, rollback.

## 58) Internationalization and Localization

* Resource files for user-facing text. No hard-coded strings.
* Culture from `Accept-Language` with fallback. Use `DateTimeOffset` and invariant formatting.

## 59) Accessibility (if UI present)

* WCAG 2.1 AA targets. Keyboard nav, ARIA roles, contrast checks.
* UI tests enforce color contrast and focus order.

## 60) Copilot Prompting Rules (Meta)

* Always request context: target layer, feature, DTOs, constraints.
* Prefer vertical slice scaffolds: request, validator, handler, endpoint, tests.
* Emit tests with every new feature by default. Include edge cases and failure paths.
* If uncertain API shape, propose two alternatives with trade-offs.

## 61) Sample PR Checklist (Extended)

* [ ] Contract updated and backward compatible
* [ ] Error codes added to catalog
* [ ] Telemetry added: logs, traces, metrics with cardinality control
* [ ] Rate limits and timeouts configured
* [ ] Docs, runbook, and examples updated
* [ ] Feature flag and migration plan included
* [ ] Rollback plan verified
* [ ] Benchmarks or perf notes for hot paths

## 62) Timeouts, Retries, and Budgets

* Default outbound timeouts: HTTP 10s, DB 30s, cache 3s, broker 5s.
* Retry only idempotent operations. Exponential backoff with jitter. Retry budget ≤ 20% of RPS.
* Circuit breaker on error rate and latency spikes. Bulkhead for high-risk deps.

## 63) Thread Pool and Async Guidelines

* No sync-over-async. No blocking calls on request threads.
* For CPU-bound work, use `Task.Run` off request path and measure saturation.
* Cap parallelism with `SemaphoreSlim` or channels. Never unbounded `Task.WhenAll` on user input.

## 64) Logging Quality Gates

* Each log has intent: audit, diagnostic, or business metric.
* Use structured properties. Limit high-cardinality keys. No arrays of large objects.
* Event IDs standardized per feature. Redact PII.

## 65) Example Endpoint Contract Snippet

```yaml
# OpenAPI excerpt pattern
responses:
  "200":
    description: OK
    headers:
      ETag:
        schema: { type: string }
    content:
      application/json:
        schema: { $ref: "#/components/schemas/Order" }
        examples:
          default:
            value: { id: "ord_123", total: 120.50, currency: "EUR", status: "paid" }
```

Use this file as the contract. Generate code that satisfies each section by default.