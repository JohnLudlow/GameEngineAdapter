# Integration test suite for AI, combat, and map generation scenarios

## Overview

This plan covers the creation of a deterministic integration test suite for the headless adapter.
No integration tests currently exist for the adapter in end-to-end simulation scenarios. The tests
validate that the adapter correctly supports the patterns required for AI/NPC decision loops, combat
audio/render sequences, and map generation batches — without relying on any native platform
dependencies.

Each scenario uses `DeterministicEngineRunner` with a fixed seed and either `HeadlessAdapter` or
`TestAdapter` to record provider calls. Running the same scenario twice with the same seed must
produce byte-for-byte identical output, making failures easy to diagnose.

Before the integration tests can be written, `DeterministicEngineRunner` must be extended with a
tick callback, `SimulationTime` tracking, and a configurable camera. Phase 0 covers those runner
enhancements; Phases 1–3 cover the three domain scenarios.

## Table of contents

- [Integration test suite for AI, combat, and map generation scenarios](#integration-test-suite-for-ai-combat-and-map-generation-scenarios)
  - [Overview](#overview)
  - [Table of contents](#table-of-contents)
  - [Plan issue](#plan-issue)
  - [Plan status](#plan-status)
  - [Definition of terms](#definition-of-terms)
  - [Architectural considerations and constraints](#architectural-considerations-and-constraints)
  - [Implementation guide](#implementation-guide)
    - [Plan requirements](#plan-requirements)
    - [Phase 0 — DeterministicEngineRunner enhancements](#phase-0--deterministicenginerunner-enhancements)
      - [Objective](#objective)
      - [Technical details](#technical-details)
      - [Phase requirements](#phase-requirements)
      - [Examples](#examples)
    - [Phase 1 — AI/NPC decision-loop scenario](#phase-1--ainpc-decision-loop-scenario)
      - [Objective](#objective-1)
      - [Technical details](#technical-details-1)
      - [Phase requirements](#phase-requirements-1)
      - [Examples](#examples-1)
    - [Phase 2 — Combat audio/render sequence scenario](#phase-2--combat-audiorender-sequence-scenario)
      - [Objective](#objective-2)
      - [Technical details](#technical-details-2)
      - [Phase requirements](#phase-requirements-2)
      - [Examples](#examples-2)
    - [Phase 3 — Map generation batch scenario](#phase-3--map-generation-batch-scenario)
      - [Objective](#objective-3)
      - [Technical details](#technical-details-3)
      - [Phase requirements](#phase-requirements-3)
      - [Examples](#examples-3)
  - [See also](#see-also)
  - [References](#references)

## Plan issue

- [#16](https://github.com/JohnLudlow/GameEngineAdapter/issues/16)

## Plan status

Complete

Implemented on branch `16-integration-test-suite-for-ai-combat-and-map-generation-scenarios-no-plan-yet` across commits `1d97689`, `b38948b`, `375d5f6`, `a81a687`, with the `Random`-rename refinements in `25caca2` and `f936c1a`. All 132 tests in `GameEngineAdapter.UnitTests` pass.

## Definition of terms

| Term | Meaning | Reference |
| ---- | ------- | --------- |
| Deterministic scenario | A simulation run whose output is fully determined by the seed and scripted inputs, with no external or random variation. | |
| Fixed timestep | A simulation approach where each update advances time by a constant interval, removing variable frame-rate effects. | |
| Integration test | A test that exercises multiple components (adapter, runner, providers) working together end-to-end, as opposed to a unit test that isolates a single class. | |
| Scripted input | Pre-defined key or mouse-button presses injected via `HeadlessInputProvider.ScriptKeyDown` / `ScriptKeyUp`. | |
| Seeded RNG | A `System.Random` instance initialised with a specific seed so it produces a reproducible sequence across runs. | |
| Tick callback | A delegate invoked by `DeterministicEngineRunner.Run` on each simulation step, between `BeginFrame` and `EndFrame`, allowing test code to drive per-frame simulation logic. | |

## Architectural considerations and constraints

- **Prerequisites**: Phase 0 (runner enhancements) must be complete before any integration-test phase
  can be written or pass. Phases 1–3 are independent of each other once Phase 0 is done.
- **No native dependencies**: All scenarios run headless. No GPU, audio hardware, or platform SDK
  calls are permitted. Tests must pass in GitHub Actions without additional setup.
- **Determinism contract**: Every scenario is run twice with the same seed. Both runs must produce
  identical recorded output. Any non-determinism is a bug in the adapter or the test itself.
- **Test project location**: All integration tests live in the existing
  `src/GameEngineAdapter.UnitTests/` project. No new project is needed.
- **File naming convention**: Integration test files use the suffix `*IntegrationTests.cs` to
  distinguish them from pure unit tests.
- **Self-contained scenarios**: Each scenario constructs its own adapter and runner. No shared state
  between test methods or test classes.
- **Tick callback position**: The `onTick` callback is called after `BeginFrame` and before
  `EndFrame`, so submissions inside the callback are enclosed within a valid frame.
- **`SimulationTime` increment timing**: `SimulationTime` increments after `Present()`. The callback
  for step 0 therefore sees `SimulationTime == TimeSpan.Zero`; step 1 sees 16 ms; step 2 sees
  32 ms, and so on.
- **Combat test adapter wiring**: For the combat scenario, `DeterministicEngineRunner` is
  constructed with `testAdapter.Inner` as the `HeadlessAdapter`, but all provider calls
  (render, audio) are made through the `testAdapter` providers (the recording decorators), so
  `RecordedCalls` captures the full cross-provider sequence.
- **`RecordingRenderProvider` access**: `RecordingRenderProvider` is `public sealed`, so test code
  can reach `testAdapter.Inner.RenderProvider` for direct `HeadlessRenderProvider` access when
  required.

### Dependency diagram

```text
Phase 0  ──► Phase 1 (AI)
         ──► Phase 2 (Combat)
         ──► Phase 3 (Map generation)
```

## Implementation guide

### Plan requirements

- (***Complete***) AI scenario produces stable, assertable render output
  - GIVEN a `DeterministicEngineRunner` with seed 42 and a 16 ms timestep
  - WHEN an NPC decision loop runs for 60 steps via the tick callback
  - THEN the recorded sprite submissions are identical between two runs with the same seed

- (***Complete***) Combat scenario audio event sequence is deterministic
  - GIVEN a `TestAdapter` with `DeterministicEngineRunner` seed 1337
  - WHEN a combat event script runs for 10 steps
  - THEN `RecordedCalls` contains the expected audio and render entries in the correct order

- (***Complete***) Map generation scenario produces stable mesh output
  - GIVEN a `DeterministicEngineRunner` with seed 0 and 100 steps
  - WHEN 10 mesh draw commands are submitted per tick
  - THEN the total recorded mesh count is 1000 and the content is identical between two runs

- (***Complete***) All integration scenarios run in CI without native dependencies
  - GIVEN the GitHub Actions workflow
  - WHEN `dotnet test` runs the `GameEngineAdapter.UnitTests` project
  - THEN all `*IntegrationTests.cs` scenarios pass with no platform-specific setup

### Phase 0 — DeterministicEngineRunner enhancements

***Complete***

#### Objective

Extend `DeterministicEngineRunner` with:

1. An optional `CameraDescriptor camera` constructor parameter (4th parameter, defaults to
   `default`) so test code can supply a custom camera without changing existing call sites.
2. A public `Random` property exposing the internal `Random` instance.
3. A public `SimulationTime` property of type `TimeSpan` that starts at `TimeSpan.Zero` and
   advances by `_fixedTimestep` after each step's `Present()` call.
4. An `onTick` parameter on `Run` (`Action<DeterministicTickContext>? onTick = null`) called
   between `BeginFrame` and `EndFrame` on every step.

Success criterion: all six existing tests in `DeterministicEngineRunnerTests.cs` continue to
pass, and nine new unit tests (see Phase requirements) all pass.

#### Technical details

The updated `Run` loop executes in this order for each step:

```text
BeginFrame(camera)
  → onTick(new DeterministicTickContext(SimulationTime, _random, _adapter))
EndFrame()
Present()
SimulationTime += _fixedTimestep
```

`SimulationTime` is incremented *after* `Present()`, so the callback for step 0 receives
`TimeSpan.Zero`, step 1 receives `_fixedTimestep`, step 2 receives `2 × _fixedTimestep`, and
so on. This matches the expectation of consuming code that treats `SimulationTime` as the elapsed
time at the *start* of the current tick.

The `CameraDescriptor camera` parameter replaces the hard-coded `ProjectionType.Orthographic`
camera that currently lives inside `Run`. When `camera` equals `default`, all camera fields are
zero-initialised. The headless provider ignores camera configuration entirely (its `BeginFrame`
is a no-op), so existing tests pass unchanged. Callers targeting a real renderer must supply an
explicit camera, as the zero-initialised defaults differ from the current hard-coded values
(`OrthographicSize = 10f`, `AspectRatio ≈ 1.778`, `NearPlane = 0.1f`, `FarPlane = 100f`).

All nine new unit tests belong in `DeterministicEngineRunnerTests.cs`:

| Test | Assertion |
| ---- | --------- |
| `SimulationTime_IsZeroAfterConstruction` | `runner.SimulationTime == TimeSpan.Zero` |
| `SimulationTime_AdvancesPerStep` | 5 steps × 16 ms → `SimulationTime == 80 ms` |
| `SimulationTime_AccumulatesAcrossMultipleRunCalls` | Two `Run(5)` calls → `SimulationTime == 160 ms` (see note) |
| `TickCallback_ReceivesSequentialSimulationTimes` | 3-step run collects times 0 ms, 16 ms, 32 ms |
| `TickCallback_RandomIsSameReferenceAsRunnerRandom` | `ctx.Random` is `ReferenceEquals` to `runner.Random` |
| `TickCallback_AdapterIsSameReferenceAsRunnerAdapter` | `ReferenceEquals(ctx.Adapter, runner.Adapter)` is true |
| `TickCallback_RandomStatePersistsAcrossInvocations` | Two consecutive steps receive different RNG values; re-run with the same seed produces the identical pair |
| `Run_WithNullCallback_DoesNotThrow` | `runner.Run(1)` with no callback completes without exception |
| `Constructor_CameraParameterIsOptional` | Three-argument construction (no `camera`) compiles and runs without error |

> Note: Issue #15 specifies `Run(3)` then `Run(2)` → 80 ms for the accumulation test. This plan
> uses two `Run(5)` calls → 160 ms. Both parameterisations validate that `SimulationTime` persists
> across calls; the values differ by design for clarity, not due to a specification conflict.

#### Phase requirements

- (***Complete***) `SimulationTime` starts at zero
  - GIVEN a newly constructed `DeterministicEngineRunner`
  - WHEN `SimulationTime` is read before `Run` is called
  - THEN it equals `TimeSpan.Zero`

- (***Complete***) `SimulationTime` advances by one timestep per step
  - GIVEN a runner with a 16 ms timestep
  - WHEN `Run(5)` is called
  - THEN `SimulationTime` equals `TimeSpan.FromMilliseconds(80)`

- (***Complete***) `SimulationTime` accumulates across multiple `Run` calls
  - GIVEN a runner with a 16 ms timestep
  - WHEN `Run(5)` is called twice
  - THEN `SimulationTime` equals `TimeSpan.FromMilliseconds(160)`

- (***Complete***) Tick callback receives sequential `SimulationTime` values
  - GIVEN a runner with a 16 ms timestep and a 3-step run
  - WHEN the tick callback records `ctx.SimulationTime` on each invocation
  - THEN the recorded sequence is `[0 ms, 16 ms, 32 ms]`

- (***Complete***) `ctx.Random` is the same reference as `runner.Random`
  - GIVEN a tick callback that captures `ctx.Random`
  - WHEN compared with `runner.Random` after the run
  - THEN `ReferenceEquals(ctx.Random, runner.Random)` is true

- (***Complete***) `Run` with no callback does not throw
  - GIVEN a runner with a valid adapter
  - WHEN `Run(1)` is called without providing an `onTick` delegate
  - THEN no exception is thrown

- (***Complete***) `ctx.Adapter` is the same reference as the runner's adapter
  - GIVEN a tick callback that captures `ctx.Adapter`
  - WHEN compared with `runner.Adapter` after the run
  - THEN `ReferenceEquals(ctx.Adapter, runner.Adapter)` is true

- (***Complete***) RNG state persists across callback invocations within the same `Run` call
  - GIVEN a runner with seed 42 and a callback that records one RNG value per step
  - WHEN `Run(2)` is called
  - THEN the two recorded values differ (demonstrating state persistence), and a second
    identical run with seed 42 produces the same pair of values

- (***Complete***) Optional camera constructor parameter does not break existing call sites
  - GIVEN the updated `DeterministicEngineRunner` constructor with an optional 4th parameter
  - WHEN constructed with three arguments (no `camera` parameter)
  - THEN the code compiles and the runner executes without error

- (***Complete***) All existing `DeterministicEngineRunnerTests` still pass
  - GIVEN the updated `DeterministicEngineRunner` implementation
  - WHEN the existing six tests are run
  - THEN all six pass without modification

#### Examples

```csharp
// src/GameEngineAdapter.Headless/DeterministicEngineRunner.cs

private readonly CameraDescriptor _camera;

/// <summary>Gets the underlying headless adapter driven by this runner.</summary>
public HeadlessAdapter Adapter => _adapter;

/// <summary>Gets the seeded random number generator used by this runner.</summary>
public Random Random => _random;

/// <summary>
/// Gets the total simulated time elapsed since the runner was constructed.
/// Advances by one <c>fixedTimestep</c> after each step's <c>Present()</c> call.
/// </summary>
public TimeSpan SimulationTime { get; private set; } = TimeSpan.Zero;

/// <summary>
/// Initializes a new deterministic engine runner.
/// </summary>
/// <param name="adapter">The headless adapter to drive.</param>
/// <param name="seed">RNG seed for reproducibility.</param>
/// <param name="fixedTimestep">Time interval per simulation step.</param>
/// <param name="camera">
/// Camera descriptor used for each frame. Defaults to <c>default(CameraDescriptor)</c>
/// (all fields zero-initialised) when not specified. The headless provider ignores
/// camera configuration, so this parameter matters only for production renderers.
/// </param>
public DeterministicEngineRunner(
    HeadlessAdapter adapter,
    int seed,
    TimeSpan fixedTimestep,
    CameraDescriptor camera = default)
{
    _adapter = adapter;
    _random = new Random(seed);
    _fixedTimestep = fixedTimestep;
    _camera = camera;
    SimulationTime = TimeSpan.Zero;
}

/// <summary>
/// Runs a fixed number of simulation steps deterministically.
/// </summary>
/// <param name="steps">Number of steps to simulate.</param>
/// <param name="onTick">
/// Optional callback invoked each step between <c>BeginFrame</c> and <c>EndFrame</c>.
/// Receives a <see cref="DeterministicTickContext"/> containing the current
/// <see cref="SimulationTime"/>, the seeded <see cref="Random"/>, and the adapter.
/// </param>
public void Run(int steps, Action<DeterministicTickContext>? onTick = null)
{
    var renderProvider = _adapter.RenderProvider;
    for (var i = 0; i < steps; i++)
    {
        using var scope = renderProvider.BeginFrame(in _camera);
        onTick?.Invoke(new DeterministicTickContext(SimulationTime, _random, _adapter));
        renderProvider.EndFrame();
        renderProvider.Present();
        SimulationTime += _fixedTimestep;
    }
}
```

```csharp
// src/GameEngineAdapter.UnitTests/DeterministicEngineRunnerTests.cs (new tests to add)
// Add these [Fact] methods to the existing DeterministicEngineRunnerTests class.
// Existing namespace, using directives, and helper method MakeConfig() are already in place.

[Fact]
public void SimulationTime_IsZeroAfterConstruction()
{
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

    Assert.Equal(TimeSpan.Zero, runner.SimulationTime);
}

[Fact]
public void SimulationTime_AdvancesPerStep()
{
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

    runner.Run(5);

    Assert.Equal(TimeSpan.FromMilliseconds(80), runner.SimulationTime);
}

[Fact]
public void TickCallback_ReceivesSequentialSimulationTimes()
{
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));
    var times = new List<TimeSpan>();

    runner.Run(3, ctx => times.Add(ctx.SimulationTime));

    Assert.Equal(
        [TimeSpan.Zero, TimeSpan.FromMilliseconds(16), TimeSpan.FromMilliseconds(32)],
        times);
}

[Fact]
public void TickCallback_AdapterIsSameReferenceAsRunnerAdapter()
{
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));
    HeadlessAdapter? captured = null;

    runner.Run(1, ctx => captured = ctx.Adapter);

    Assert.True(ReferenceEquals(runner.Adapter, captured));
}

[Fact]
public void TickCallback_RandomStatePersistsAcrossInvocations()
{
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner1 = new DeterministicEngineRunner(adapter, 42, TimeSpan.FromMilliseconds(16));
    var values1 = new List<float>();

    runner1.Run(2, ctx => values1.Add(ctx.Random.NextSingle()));

    // Two steps must produce different values (state persists across invocations)
    Assert.NotEqual(values1[0], values1[1]);

    // Re-run with the same seed must produce the identical sequence
    using var adapter2 = new HeadlessAdapter(new EngineConfig("Headless", null, null));
    var runner2 = new DeterministicEngineRunner(adapter2, 42, TimeSpan.FromMilliseconds(16));
    var values2 = new List<float>();
    runner2.Run(2, ctx => values2.Add(ctx.Random.NextSingle()));

    Assert.Equal(values1, values2);
}

[Fact]
public void Constructor_CameraParameterIsOptional()
{
    // Three-argument construction (no camera) must compile and run without error.
    var config = new EngineConfig("Headless", null, null);
    using var adapter = new HeadlessAdapter(config);
    var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

    runner.Run(1);
}
```

### Phase 1 — AI/NPC decision-loop scenario

***Complete***

#### Objective

Write `AiIntegrationTests.cs`. The scenario simulates an NPC that uses the seeded RNG each tick to
produce a deterministic world-space position, then submits a sprite at that position.

Run with seed 42 for 60 steps at a 16 ms timestep, collect `render.RecordedSprites`, then run again
with the same seed and assert both collections are equal element-by-element. A mismatch indicates
non-determinism in the runner or the RNG wiring.

#### Technical details

- The test helper `RunNpcScenario(int seed)` constructs a fresh `HeadlessAdapter` and
  `DeterministicEngineRunner` each time it is called, ensuring no shared state.
- `ctx.Random.NextSingle()` is called twice per tick (for X and Y) in a fixed order; because
  `Random` with the same seed produces the same sequence, both runs produce identical positions.
- `HeadlessRenderProvider` is cast from `adapter.RenderProvider` to access `RecordedSprites`
  directly. No `TestAdapter` is needed here because the assertion targets render output only.
- `DefaultMaterial` is declared as a `static readonly` field to avoid per-test allocation.

#### Phase requirements

- (***Complete***) NPC sprite positions are identical between runs with the same seed
  - GIVEN two `DeterministicEngineRunner` instances with seed 42, 60 steps, and a 16 ms timestep
  - WHEN both are run with an equivalent NPC tick callback
  - THEN `run1` and `run2` are equal element-by-element (`Assert.Equal(run1, run2)`)

#### Examples

```csharp
// src/GameEngineAdapter.UnitTests/AiIntegrationTests.cs

namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class AiIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<SpriteDrawDto> RunNpcScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, seed, TimeSpan.FromMilliseconds(16));
        var render = (HeadlessRenderProvider)adapter.RenderProvider;

        runner.Run(60, ctx =>
        {
            var x = ctx.Random.NextSingle() * 100f;
            var y = ctx.Random.NextSingle() * 100f;
            var transform = new TransformDto(x, y, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
            adapter.RenderProvider.SubmitSprite(new SpriteDrawDto("npc", transform, DefaultMaterial, 0));
        });

        return render.RecordedSprites;
    }

    [Fact]
    public void NpcScenario_IsDeterministic()
    {
        var run1 = RunNpcScenario(42);
        var run2 = RunNpcScenario(42);
        Assert.Equal(run1, run2);
    }
}
```

### Phase 2 — Combat audio/render sequence scenario

***Complete***

#### Objective

Write `CombatIntegrationTests.cs`. Script 10 combat steps: every step submits a character sprite;
on even steps (step % 2 == 0) also play a weapon sound. Use `TestAdapter` so that both render and
audio calls appear together in `RecordedCalls`, then assert the combined sequence is correct and
stable across two runs.

#### Technical details

- `DeterministicEngineRunner` is constructed with `testAdapter.Inner` (the underlying
  `HeadlessAdapter`) so the runner drives the frame lifecycle.
- All provider submissions go through `testAdapter.RenderProvider` and `testAdapter.AudioPlayer`
  (the recording decorators), so every call is captured in `testAdapter.RecordedCalls`.
- The expected `RecordedCalls` sequence for 10 steps is:
  - Step 0: `SubmitSprite`, `StartPlayback("weapon_fire")`
  - Step 1: `SubmitSprite`
  - Step 2: `SubmitSprite`, `StartPlayback("weapon_fire")`
  - … (alternating)

  Total: 15 entries (10 sprite submissions + 5 audio calls on even steps).
  Note: `DeterministicEngineRunner` is wired to `testAdapter.Inner` (the `HeadlessAdapter`),
  so `BeginFrame`, `EndFrame`, and `Present` are dispatched directly to `HeadlessRenderProvider`,
  bypassing the `RecordingRenderProvider` decorator. Only explicit calls made through
  `testAdapter.RenderProvider` and `testAdapter.AudioPlayer` appear in `RecordedCalls`.
- Two identical runs with seed 1337 must produce equal `RecordedCalls` sequences.

#### Phase requirements

- (***Complete***) Combat call sequence is deterministic and correctly ordered
  - GIVEN two `TestAdapter` instances driven by `DeterministicEngineRunner` with seed 1337 and
    10 steps
  - WHEN the combat tick callback submits a sprite each step and audio on even steps
  - THEN both `RecordedCalls` sequences are equal element-by-element

- (***Complete***) Audio events occur only on even steps
  - GIVEN a single 10-step combat run
  - WHEN `RecordedCalls` is filtered to audio entries
  - THEN exactly 5 audio entries are present, each recording `StartPlayback("weapon_fire")`

#### Examples

```csharp
// src/GameEngineAdapter.UnitTests/CombatIntegrationTests.cs

namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using System.Linq;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class CombatIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<RecordedCall> RunCombatScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var testAdapter = new TestAdapter(config);
        var runner = new DeterministicEngineRunner(testAdapter.Inner, seed, TimeSpan.FromMilliseconds(16));

        var step = 0;
        runner.Run(10, ctx =>
        {
            var transform = new TransformDto(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
            testAdapter.RenderProvider.SubmitSprite(
                new SpriteDrawDto("character", transform, DefaultMaterial, 0));

            if (step % 2 == 0)
                testAdapter.AudioPlayer.StartPlayback("weapon_fire");

            step++;
        });

        return testAdapter.RecordedCalls;
    }

    [Fact]
    public void CombatScenario_IsDeterministic()
    {
        var run1 = RunCombatScenario(1337);
        var run2 = RunCombatScenario(1337);
        // RecordedCall.Arguments uses reference equality on its IReadOnlyList<object?> field.
        // Compare the call sequence by ProviderName and MethodName to assert ordering stability.
        Assert.Equal(
            run1.Select(c => (c.ProviderName, c.MethodName)),
            run2.Select(c => (c.ProviderName, c.MethodName)));
    }

    [Fact]
    public void CombatScenario_AudioOnEvenStepsOnly()
    {
        var calls = RunCombatScenario(1337);
        var audioCalls = calls.Where(c => c.ProviderName == "Audio").ToList();
        Assert.Equal(5, audioCalls.Count);
        Assert.All(audioCalls, c => Assert.Equal("weapon_fire", (string?)c.Arguments[0]));
    }
}
```

### Phase 3 — Map generation batch scenario

***Complete***

#### Objective

Write `MapGenerationIntegrationTests.cs`. Simulate 100 steps; each step submits 10 mesh draw
commands whose geometry (position/scale) is derived from the seeded RNG. Assert that the total
mesh count is 1000 and that all mesh entries are identical between two runs with seed 0.

#### Technical details

- Uses `HeadlessAdapter` directly (no `TestAdapter` needed — the assertion is on mesh count and
  content, not on cross-provider call order).
- `render.RecordedMeshes` provides direct access to the accumulated `MeshDrawDto` list.
- 100 steps × 10 meshes per step = 1000 total mesh entries; this is asserted as a count before
  the per-element equality check to give a clearer failure message if the count is wrong.
- `ctx.Random.NextSingle()` is called six times per mesh (X, Y, Z, ScaleX, ScaleY, ScaleZ) in a
  fixed order inside the callback. The same seed produces the same sequence both times.

#### Phase requirements

- (***Complete***) Map generation produces exactly 1000 mesh entries
  - GIVEN a `DeterministicEngineRunner` with seed 0, 100 steps, and 10 meshes per tick
  - WHEN `Run` completes
  - THEN `render.RecordedMeshes.Count == 1000`

- (***Complete***) Map generation output is identical between runs with the same seed
  - GIVEN two runs with seed 0
  - WHEN mesh entries are compared element-by-element
  - THEN all 1000 entries are equal

#### Examples

```csharp
// src/GameEngineAdapter.UnitTests/MapGenerationIntegrationTests.cs

namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class MapGenerationIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<MeshDrawDto> RunMapScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, seed, TimeSpan.FromMilliseconds(16));
        var render = (HeadlessRenderProvider)adapter.RenderProvider;

        runner.Run(100, ctx =>
        {
            for (var i = 0; i < 10; i++)
            {
                var x = ctx.Random.NextSingle() * 512f;
                var y = ctx.Random.NextSingle() * 512f;
                var z = ctx.Random.NextSingle() * 10f;
                var scaleX = ctx.Random.NextSingle() * 4f + 1f;
                var scaleY = ctx.Random.NextSingle() * 4f + 1f;
                var scaleZ = ctx.Random.NextSingle() * 4f + 1f;
                var transform = new TransformDto(x, y, z, 0f, 0f, 0f, scaleX, scaleY, scaleZ);
                adapter.RenderProvider.SubmitMesh(
                    new MeshDrawDto($"tile_{i}", transform, DefaultMaterial, 0));
            }
        });

        return render.RecordedMeshes;
    }

    [Fact]
    public void MapScenario_Produces1000Meshes()
    {
        var meshes = RunMapScenario(0);
        Assert.Equal(1000, meshes.Count);
    }

    [Fact]
    public void MapScenario_IsDeterministic()
    {
        var run1 = RunMapScenario(0);
        var run2 = RunMapScenario(0);
        Assert.Equal(run1, run2);
    }
}
```

## See also

- [Phase 2 — Headless adapter development](./phase-2-headless-adapter-development.md) — parent plan
  that this integration suite closes out
- [Issue #14](https://github.com/JohnLudlow/GameEngineAdapter/issues/14) — recording decorators
  (prerequisite, complete)
- [Issue #15](https://github.com/JohnLudlow/GameEngineAdapter/issues/15) — `SimulationTime` + tick
  callback (prerequisite, partially complete — Phase 0 finishes it)

## References

- `src/GameEngineAdapter.Headless/DeterministicEngineRunner.cs` — runner to be extended in Phase 0
- `src/GameEngineAdapter.Headless/DeterministicTickContext.cs` — tick context struct (already
  exists)
- `src/GameEngineAdapter.Headless/HeadlessRenderProvider.cs` — exposes `RecordedSprites` and
  `RecordedMeshes`
- `src/GameEngineAdapter.Headless/TestAdapter.cs` — exposes `RecordedCalls`, `Inner`, and
  recording provider properties
- `src/GameEngineAdapter.UnitTests/DeterministicEngineRunnerTests.cs` — existing runner tests
  (must all remain green after Phase 0)
- xUnit docs: <https://xunit.net/>
