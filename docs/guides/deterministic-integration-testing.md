# Deterministic integration testing

## Overview

The `GameEngineAdapter.Headless` assembly supports deterministic, CI-clean integration tests
that exercise multiple providers together (render + audio + input) without any GPU, audio
hardware, or platform SDK calls. The pieces are `HeadlessAdapter`, `TestAdapter`, and
`DeterministicEngineRunner` with its `onTick` callback. By convention, these tests live in
`src/GameEngineAdapter.UnitTests/` and use the file-name suffix `*IntegrationTests.cs` to
distinguish them from pure unit tests.

## When to write an integration test

- **Cross-provider scenarios** — when a single test must observe how render, audio, input,
  and asset providers interact in order. A pure unit test isolates one class behind mocks
  and cannot show ordering across providers.
- **End-to-end determinism guarantees** — when the contract under test is "running the same
  scenario twice with the same seed produces byte-for-byte identical recorded output".
- **Exercising the runner's tick callback** — when the behaviour is per-step simulation logic
  driven by `DeterministicEngineRunner.Run`, not an isolated method on a single class.

For tests that target the behaviour of one class (for example, a single provider
implementation), prefer a normal unit test in the same project.

## The pieces

The public API a test author touches lives in `JohnLudlow.GameEngineAdapter.Headless`:

- **`HeadlessAdapter`** — an in-memory `IEngineAdapter` whose render provider records every
  submission. Cast `adapter.RenderProvider` to `HeadlessRenderProvider` to read back
  `RecordedSprites`, `RecordedTexts`, and `RecordedMeshes`.

- **`TestAdapter`** — wraps a `HeadlessAdapter` and decorates every provider with a
  recording wrapper. Useful when the assertion is on the *sequence* of cross-provider calls
  rather than the individual outputs of one provider. It exposes:
  - `Inner` — the wrapped `HeadlessAdapter`.
  - `RecordedCalls` — the ordered `IReadOnlyList<RecordedCall>` of every call routed
    through one of the recording decorators.
  - `RenderProvider`, `InputProvider`, `AssetProvider`, `AudioPlayer` — the recording
    decorators. Calls made through these properties are appended to `RecordedCalls`.

- **`DeterministicEngineRunner(HeadlessAdapter adapter, int seed, TimeSpan fixedTimestep, CameraDescriptor camera = default)`**
  — a fixed-timestep loop with a seeded `Random`. The optional `camera` defaults to
  `default(CameraDescriptor)`, which is safe for headless runs because
  `HeadlessRenderProvider.BeginFrame` ignores camera configuration.

  Public properties:
  - `Adapter` — the `HeadlessAdapter` driven by this runner.
  - `Random` — the seeded `System.Random` instance.
  - `SimulationTime` — accumulated simulated time, starting at `TimeSpan.Zero`.

- **`DeterministicEngineRunner.Run(int steps, Action<DeterministicTickContext>? onTick = null)`**
  — runs `steps` simulation steps. The `onTick` callback, if provided, is invoked on each
  step between `BeginFrame` and `EndFrame`, so submissions made inside it are enclosed in a
  valid frame. After the callback, the runner calls `EndFrame()`, then `Present()`, then
  advances `SimulationTime` by `fixedTimestep`.

- **`DeterministicTickContext`** — a `readonly record struct` with three members:
  `(TimeSpan SimulationTime, Random Random, HeadlessAdapter Adapter)`. The `Random`
  reference is the same instance as `runner.Random`; calls inside the callback consume
  the runner's RNG sequence.

## A minimal example

```csharp
using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

var config = new EngineConfig("Headless", null, null);
using var adapter = new HeadlessAdapter(config);
var runner = new DeterministicEngineRunner(adapter, seed: 42, TimeSpan.FromMilliseconds(16));
var render = (HeadlessRenderProvider)adapter.RenderProvider;

var material = new MaterialDto("default", new Dictionary<string, object>(), []);

runner.Run(10, ctx =>
{
    var x = ctx.Random.NextSingle() * 100f;
    var transform = new TransformDto(x, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
    adapter.RenderProvider.SubmitSprite(new SpriteDrawDto("npc", transform, material, 0));
});

IReadOnlyList<SpriteDrawDto> recorded = render.RecordedSprites;
```

Running this snippet twice with the same seed produces identical contents in `RecordedSprites`.

## Recording cross-provider call sequences

Use `TestAdapter` instead of `HeadlessAdapter` directly when the assertion is on the order
in which different providers are invoked — for example, "every even step submits a sprite
*and* starts an audio playback".

There is one wiring rule worth stating up front. The runner's frame lifecycle calls
(`BeginFrame`, `EndFrame`, `Present`) are dispatched against whichever `HeadlessAdapter` you
hand it. If you construct the runner with `testAdapter.Inner`, those lifecycle calls go
straight to the inner `HeadlessRenderProvider` and **bypass** the `RecordingRenderProvider`
decorator. Only the explicit calls your test code makes through `testAdapter.RenderProvider`,
`testAdapter.AudioPlayer`, and the other recording decorators land in `RecordedCalls`.

This is usually what you want — the recorded sequence then contains exactly the application
events your test produced, with no per-frame lifecycle noise. See
`src/GameEngineAdapter.UnitTests/CombatIntegrationTests.cs` for a worked example.

## Determinism contract

- **Same seed + same scripted inputs ⇒ identical recorded output.** Two runs of the same
  scenario must be byte-for-byte equal. Any difference is a bug in the adapter, the runner,
  or the test itself.
- **Single-source RNG.** Use only `ctx.Random` (or, equivalently, `runner.Random`) for any
  randomised value. Do not call `Guid.NewGuid()`, `DateTime.Now`, `Stopwatch.GetTimestamp()`,
  `Environment.TickCount`, or any other unseeded source from inside the callback or the
  scenario helper.
- **`SimulationTime` advances after `Present()`.** Step 0's callback observes
  `ctx.SimulationTime == TimeSpan.Zero`. Step 1's callback observes `fixedTimestep`.
  Step 2's callback observes `2 × fixedTimestep`. Tests that script behaviour by elapsed
  time should rely on this ordering rather than on `Stopwatch`-style measurement.
- **Equality on recorded DTO lists** uses record-struct value equality. `SpriteDrawDto`,
  `TextDrawDto`, and `MeshDrawDto` are records, so `Assert.Equal(run1, run2)` over
  `IReadOnlyList<SpriteDrawDto>` (and the others) compares element-by-element by value.
  However, `MaterialDto` contains reference-typed `IReadOnlyDictionary` and
  `IReadOnlyList` fields and falls back to reference equality on those, so two materials
  constructed independently with the same contents are *not* equal. The pattern in all
  three existing scenarios is to declare a single `static readonly MaterialDto DefaultMaterial`
  on the test class and reuse it across runs.
- **Equality on `RecordedCall` sequences.** `RecordedCall.Arguments` is
  `IReadOnlyList<object?>` and uses reference equality, so a literal `Assert.Equal` on two
  `RecordedCalls` lists will usually fail even when the runs are deterministic. Project to
  a stable shape first — for example
  `calls.Select(c => (c.ProviderName, c.MethodName))` — and assert equality on that. See
  `CombatIntegrationTests.cs` for the idiom.

## File location and naming

Integration tests live alongside unit tests in `src/GameEngineAdapter.UnitTests/`. They use
the file-name suffix `*IntegrationTests.cs` to make their scope visible at a glance.
Existing examples are `AiIntegrationTests.cs`, `CombatIntegrationTests.cs`, and
`MapGenerationIntegrationTests.cs`.

## See also

- [Integration test suite plan](../plans/integration-test-suite.md) — the plan that delivered this capability.
- [`DeterministicEngineRunner.cs`](../../src/GameEngineAdapter.Headless/DeterministicEngineRunner.cs)
- [`DeterministicTickContext.cs`](../../src/GameEngineAdapter.Headless/DeterministicTickContext.cs)
- [`TestAdapter.cs`](../../src/GameEngineAdapter.Headless/TestAdapter.cs)
- [`AiIntegrationTests.cs`](../../src/GameEngineAdapter.UnitTests/AiIntegrationTests.cs)
- [`CombatIntegrationTests.cs`](../../src/GameEngineAdapter.UnitTests/CombatIntegrationTests.cs)
- [`MapGenerationIntegrationTests.cs`](../../src/GameEngineAdapter.UnitTests/MapGenerationIntegrationTests.cs)
