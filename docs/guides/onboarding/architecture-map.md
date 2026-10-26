# Architecture map

GameEngineAdapter separates engine-neutral contracts from engine-specific
implementations. The design allows game code to depend on the Core project
without importing a graphics, terminal, or platform SDK.

```text
Consumer / game code
        |
        v
GameEngineAdapter.Core
  IEngineAdapter
  EngineCapabilities
  provider interfaces and DTOs
        ^
        |
Adapter implementations
  GameEngineAdapter.Headless
  future engine-specific adapters
```

## Project responsibilities

| Project | Responsibility | Start here |
| --- | --- | --- |
| `GameEngineAdapter.Core` | Public contracts and data-transfer objects (DTOs). | `IEngineAdapter.cs` |
| `GameEngineAdapter.Headless` | In-memory adapter used to run deterministic scenarios without platform hardware. | `HeadlessAdapter.cs` |
| `GameEngineAdapter.UnitTests` | Contract and integration tests. | `CombatIntegrationTests.cs` |
| `GameEngineAdapter.Benchmarks` | Performance measurements for translation-oriented work. | `RenderTranslationBenchmarks.cs` |

## The adapter boundary

`IEngineAdapter` exposes a capability descriptor and providers for rendering,
input, UI, assets, and audio. It does not imply that every provider has the
same level of support in every adapter. Consumers should inspect
`EngineCapabilities` before relying on an optional feature.

The render lifecycle is defined by `IRenderProvider`:

```text
BeginFrame(camera)
  -> submit SpriteDrawDto / TextDrawDto / MeshDrawDto
  -> EndFrame()
  -> Present()
```

`FrameScope` only represents the active frame. Per `IRenderProvider`, callers
must explicitly invoke both `EndFrame` and `Present`.

## Headless testing path

`HeadlessAdapter` supplies in-memory providers. `DeterministicEngineRunner`
executes a fixed number of render frames with a supplied seed and fixed
timestep. `TestAdapter` wraps headless providers to record the application call
sequence for assertions.

For examples and determinism constraints, read
[Deterministic integration testing](../deterministic-integration-testing.md).
