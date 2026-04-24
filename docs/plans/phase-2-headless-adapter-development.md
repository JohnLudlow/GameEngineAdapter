# Phase 2 — Headless adapter development

## Overview

Phase 2 implements a minimal, dependency-free headless adapter for the GameEngineAdapter project. This adapter is housed in a separate assembly (`GameEngineAdapter.Headless`) under the `JohnLudlow.GameEngineAdapter.Headless` namespace, referencing the core contracts from `JohnLudlow.GameEngineAdapter.Core`. It enables deterministic, reproducible integration testing and CI runs by simulating rendering, input, and audio operations without any native platform dependencies. The HeadlessAdapter records render commands, simulates input via scripted events, and provides deterministic execution modes (fixed timestep, seeded RNG) to ensure stable test results. A TestAdapter is also provided for CI environments to record and assert adapter calls.

## Table of contents

- [Phase 2 — Headless adapter development](#phase-2--headless-adapter-development)
  - [Overview](#overview)
  - [Table of contents](#table-of-contents)
  - [Plan issue](#plan-issue)
  - [Plan status](#plan-status)
  - [Definition of terms](#definition-of-terms)
  - [Architectural considerations and constraints](#architectural-considerations-and-constraints)
  - [Implementation guide](#implementation-guide)
    - [Plan requirements](#plan-requirements)
    - [Phase 1 — HeadlessAdapter core](#phase-1--headlessadapter-core)
      - [Objective](#objective)
      - [Technical details](#technical-details)
      - [Phase requirements](#phase-requirements)
      - [Examples](#examples)
    - [Phase 2 — TestAdapter for CI](#phase-2--testadapter-for-ci)
      - [Objective](#objective-1)
      - [Technical details](#technical-details-1)
      - [Phase requirements](#phase-requirements-1)
      - [Examples](#examples-1)
    - [Phase 3 — Deterministic execution](#phase-3--deterministic-execution)
      - [Objective](#objective-2)
      - [Technical details](#technical-details-2)
      - [Phase requirements](#phase-requirements-2)
      - [Examples](#examples-2)
    - [Testing and compatibility](#testing-and-compatibility)
    - [Performance targets](#performance-targets)
  - [Known issues and design concerns](#known-issues-and-design-concerns)
  - [See also](#see-also)
  - [References](#references)

## Plan issue

- [#5](https://github.com/JohnLudlow/GameEngineAdapter/issues/5)

## Plan status

Partially complete

## Definition of terms

| Term | Meaning | Reference |
| ---- | ------- | --------- |
| Adapter | A class that implements the Phase 1 interfaces to abstract platform-specific functionality for a given engine or runtime. | |
| Deterministic tick | A simulation step that produces identical results given the same initial state and inputs. | |
| Fixed timestep | A simulation approach where each update advances time by a constant interval, avoiding variable frame rate effects. | |
| HeadlessAdapter | An adapter implementation that simulates engine operations without platform or GPU dependencies. | |
| No-op | An operation or method that performs no action (no operation), used to satisfy interface contracts without side effects. | |
| Seeded RNG | A random number generator initialized with a specific seed to produce reproducible sequences across runs. | |
| TestAdapter | An adapter implementation that records calls for verification and assertion in automated tests (currently adapter lifecycle calls). | |

## Architectural considerations and constraints

- **Separate assembly**: The headless adapter lives in a dedicated assembly (`GameEngineAdapter.Headless`) under the `JohnLudlow.GameEngineAdapter.Headless` namespace. This isolates the headless implementation from the core contracts defined in `JohnLudlow.GameEngineAdapter.Core`, ensuring engine-specific adapters do not depend on each other. The headless assembly references the core assembly via a project reference.
- **Dependency on Phase 1 interfaces**: The HeadlessAdapter and TestAdapter must implement all interfaces defined in Phase 1 (`IEngineAdapter`, `IRenderProvider`, `IInputProvider`, `IUserInterfaceProvider`, `IAssetProvider`, `IAudioPlayer`, `IAssetLoader`). Implementation cannot begin until Phase 1 interfaces are finalized.
- **No native platform dependencies**: The implementation must not use any native APIs, GPU, audio hardware, or OS-specific features. All adapters must run on any platform supported by .NET 10.
- **Deterministic execution**: The goal is reproducible behaviour given the same scenario, seed, and scripted inputs. The current implementation provides deterministic building blocks (scriptable input state and a deterministic runner skeleton).
- **Call recording architecture**: Headless providers record their own interactions (e.g., render commands and audio calls). `TestAdapter` currently records adapter lifecycle calls only; provider call recording is not yet implemented.
- **Separation of concerns**: Each provider (render, input, UI, asset, audio) is independently testable and replaceable.
- **CI compatibility**: The adapters must run in CI environments (GitHub Actions, Azure DevOps, etc.) without requiring graphics or audio hardware.

## Implementation guide

### Plan requirements

- (***Not complete***) Headless adapter passes integration scenarios for AI, combat and map generation.
  - GIVEN deterministic seeds and scenario scripts
  - WHEN CI runs the integration suite
  - THEN results are stable and asserted by tests.
  - Note: the repository currently has unit tests for the headless components, but no AI/combat/map-generation integration scenario suite.

### Phase 1 — HeadlessAdapter core

***Complete***

#### Objective

Implement a `HeadlessAdapter` and its providers that simulate all engine operations without any platform dependencies, recording render and input operations for verification. Success criteria: the adapter implements all Phase 1 contracts and can run a complete simulation loop without native dependencies.

#### Technical details

- **HeadlessAdapter**: Implements `IEngineAdapter`. Composes headless providers for rendering, input, UI, assets, and audio. Currently returns `EngineCapabilities` (including `ContractVersion`) from the `Capabilities` property.
- **HeadlessRenderProvider**: Implements `IRenderProvider`. Records all render commands (`SubmitSprite`, `SubmitText`, `SubmitMesh`) to separate typed lists per draw type (`RecordedSprites`, `RecordedTexts`, `RecordedMeshes`) for later inspection. `BeginFrame` returns a `FrameScope`; `EndFrame` and `Present` are no-ops.
- **HeadlessInputProvider**: Implements `IInputProvider`. Provides deterministic, scriptable input state via methods such as `ScriptKeyDown`, `ScriptKeyUp`, and `ScriptMousePosition`.
- **HeadlessUserInterfaceProvider**: Implements `IUserInterfaceProvider`. Currently empty (pending final UI provider contract).
- **HeadlessAssetProvider**: Implements `IAssetProvider`. Manages asset cache and lifecycle state in memory without actual file or resource access. Composes a `HeadlessAssetLoader` (implementing `IAssetLoader`) that returns stub assets for any requested identifier.
- **HeadlessAudioPlayer**: Implements `IAudioPlayer`. Records calls (`StartPlayback`, `StopPlayback`, `SetVolume`) for test assertion.

#### Phase requirements

- (***Complete***) HeadlessAdapter implements all Phase 1 contracts
  - GIVEN the Phase 1 interfaces and DTOs are defined
  - WHEN HeadlessAdapter is instantiated with an `EngineConfig`
  - THEN all provider accessors return functional headless implementations.

- (***Complete***) Render command recording
  - GIVEN a HeadlessRenderProvider
  - WHEN sprites, text, and meshes are submitted
  - THEN all commands are recorded in order and retrievable for assertion.

- (***Complete***) Scripted input playback
  - GIVEN a HeadlessInputProvider initialized with scripted events
  - WHEN input is polled
  - THEN events are returned in the scripted order.

#### Examples

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Simulates engine operations for headless, deterministic testing.
/// Exposes provider instances; individual providers may record their own calls for verification.
/// </summary>
public sealed class HeadlessAdapter : IEngineAdapter
{
    /// <summary>Gets the headless engine capabilities.</summary>
    public EngineCapabilities Capabilities { get; }

    /// <summary>Gets the configuration used to initialize this adapter.</summary>
    public EngineConfig Config { get; }

    /// <inheritdoc />
    public IRenderProvider RenderProvider { get; }

    /// <inheritdoc />
    public IInputProvider InputProvider { get; }

    /// <inheritdoc />
    public IUserInterfaceProvider UserInterfaceProvider { get; }

    /// <inheritdoc />
    public IAssetProvider AssetProvider { get; }

    /// <inheritdoc />
    public IAudioPlayer AudioPlayer { get; }

    /// <summary>
    /// Initializes a new headless adapter with the specified configuration.
    /// </summary>
    /// <param name="config">Adapter configuration.</param>
    public HeadlessAdapter(EngineConfig config)
    {
        Config = config;
        Capabilities = new EngineCapabilities(
            Supports2D: true,
            Supports3D: false,
            SupportsShaders: false,
            SupportsAudio: false,
            SupportsRichUI: false,
            ContractVersion: "1.0.0",
            SupportedTextureFormats: [],
            MaxTextureSize: 0,
            MaxAudioChannels: 0);
        RenderProvider = new HeadlessRenderProvider();
        InputProvider = new HeadlessInputProvider();
        UserInterfaceProvider = new HeadlessUserInterfaceProvider();
        AssetProvider = new HeadlessAssetProvider();
        AudioPlayer = new HeadlessAudioPlayer();
    }

    /// <inheritdoc />
    public Task InitializeAsync(EngineConfig config, CancellationToken ct = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task ShutdownAsync(CancellationToken ct = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose() { /* No resources to release */ }
}
```

```csharp
// src/GameEngineAdapter.Headless/HeadlessRenderProvider.cs

public sealed class HeadlessRenderProvider : IRenderProvider
{
    private readonly List<SpriteDrawDto> _recordedSprites = [];
    private readonly List<TextDrawDto> _recordedTexts = [];
    private readonly List<MeshDrawDto> _recordedMeshes = [];

    public IReadOnlyList<SpriteDrawDto> RecordedSprites => _recordedSprites;
    public IReadOnlyList<TextDrawDto> RecordedTexts => _recordedTexts;
    public IReadOnlyList<MeshDrawDto> RecordedMeshes => _recordedMeshes;

    public FrameScope BeginFrame(in CameraDescriptor camera) => new();
    public void SubmitSprite(in SpriteDrawDto dto) => _recordedSprites.Add(dto);
    public void SubmitText(in TextDrawDto dto) => _recordedTexts.Add(dto);
    public void SubmitMesh(in MeshDrawDto dto) => _recordedMeshes.Add(dto);
    public void EndFrame() { }
    public void Present() { }

    public void Clear()
    {
        _recordedSprites.Clear();
        _recordedTexts.Clear();
        _recordedMeshes.Clear();
    }
}
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Simulates input by maintaining scripted state (keys held, buttons held, mouse position)
/// for deterministic testing.
/// </summary>
public sealed class HeadlessInputProvider : IInputProvider
{
    private readonly HashSet<string> _keysDown = [];
    private readonly HashSet<int> _buttonsDown = [];
    private (float X, float Y) _mousePosition;

    /// <summary>
    /// Sets the specified key as held down.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    public void ScriptKeyDown(string key) => _keysDown.Add(key);

    /// <summary>
    /// Releases the specified key.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    public void ScriptKeyUp(string key) => _keysDown.Remove(key);

    /// <summary>
    /// Sets the specified mouse button as held down.
    /// </summary>
    /// <param name="button">The mouse button index.</param>
    public void ScriptMouseButtonDown(int button) => _buttonsDown.Add(button);

    /// <summary>
    /// Releases the specified mouse button.
    /// </summary>
    /// <param name="button">The mouse button index.</param>
    public void ScriptMouseButtonUp(int button) => _buttonsDown.Remove(button);

    /// <summary>
    /// Sets the scripted mouse position.
    /// </summary>
    /// <param name="x">X coordinate in screen space.</param>
    /// <param name="y">Y coordinate in screen space.</param>
    public void ScriptMousePosition(float x, float y) => _mousePosition = (x, y);

    /// <inheritdoc />
    public bool IsKeyDown(string key) => _keysDown.Contains(key);

    /// <inheritdoc />
    public bool IsMouseButtonDown(int button) => _buttonsDown.Contains(button);

    /// <inheritdoc />
    public (float X, float Y) GetMousePosition() => _mousePosition;

    /// <summary>Resets all scripted input state.</summary>
    public void Reset()
    {
        _keysDown.Clear();
        _buttonsDown.Clear();
        _mousePosition = (0f, 0f);
    }
}
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Headless implementation of <see cref="IUserInterfaceProvider"/>.
/// Executes UI layout and hit-testing logic without rendering any visuals.
/// </summary>
public sealed class HeadlessUserInterfaceProvider : IUserInterfaceProvider
{
    // Members to be defined when IUserInterfaceProvider is finalized.
}
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Manages asset cache and lifecycle state in memory without actual file or resource access.
/// Composes a <see cref="HeadlessAssetLoader"/> for stub asset loading.
/// </summary>
public sealed class HeadlessAssetProvider : IAssetProvider
{
    private readonly Dictionary<string, object> _cache = [];

    /// <summary>
    /// Initializes a new <see cref="HeadlessAssetProvider"/> with a default
    /// <see cref="HeadlessAssetLoader"/>.
    /// </summary>
    public HeadlessAssetProvider()
    {
        Loader = new HeadlessAssetLoader();
    }

    /// <inheritdoc />
    public IAssetLoader Loader { get; }

    /// <inheritdoc />
    public object? GetAsset(string assetId) =>
        _cache.TryGetValue(assetId, out var asset) ? asset : null;

    /// <inheritdoc />
    public void UnloadAsset(string assetId) => _cache.Remove(assetId);

    /// <inheritdoc />
    public bool IsAssetLoaded(string assetId) => _cache.ContainsKey(assetId);

    /// <inheritdoc />
    public void ClearCache() => _cache.Clear();

    /// <summary>
    /// Loads an asset via the loader and caches it.
    /// </summary>
    /// <param name="assetId">Asset identifier to load and cache.</param>
    public void LoadAndCache(string assetId)
    {
        if (!_cache.ContainsKey(assetId))
        {
            _cache[assetId] = Loader.Load(assetId);
        }
    }
}
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Stub asset loader that returns placeholder objects for any requested identifier.
/// </summary>
public sealed class HeadlessAssetLoader : IAssetLoader
{
    /// <inheritdoc />
    public Task<object> LoadAsync(string assetId, CancellationToken ct = default) =>
        Task.FromResult<object>(new object());

    /// <inheritdoc />
    public object Load(string assetId) => new object();
}
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Headless audio player that records all calls for verification.
/// All playback methods are no-ops; calls are recorded for test assertion.
/// </summary>
public sealed class HeadlessAudioPlayer : IAudioPlayer
{
    private readonly List<(string Method, string AudioAssetId, object? Arg)> _recordedCalls = [];

    /// <summary>Gets the list of recorded audio calls for test assertion.</summary>
    public IReadOnlyList<(string Method, string AudioAssetId, object? Arg)> RecordedCalls =>
        _recordedCalls;

    /// <inheritdoc />
    public void StartPlayback(string audioAssetId, bool loopPlayback = false) =>
        _recordedCalls.Add(("StartPlayback", audioAssetId, loopPlayback));

    /// <inheritdoc />
    public void StopPlayback(string audioAssetId) =>
        _recordedCalls.Add(("StopPlayback", audioAssetId, null));

    /// <inheritdoc />
    public void SetVolume(string audioAssetId, float volume) =>
        _recordedCalls.Add(("SetVolume", audioAssetId, volume));

    /// <summary>Clears all recorded calls.</summary>
    public void Clear() => _recordedCalls.Clear();
}
```

### Phase 2 — TestAdapter for CI

***Partially complete***

#### Objective

Provide a `TestAdapter` that wraps the HeadlessAdapter and records calls for assertion in CI environments.

#### Technical details

- **TestAdapter**: Implements `IEngineAdapter`. Composes a `HeadlessAdapter` internally.
- **Call recording**: Currently records adapter lifecycle calls (`InitializeAsync`, `ShutdownAsync`) to `RecordedCalls`.
- **Provider recording**: Provider calls are currently delegated directly to the inner providers (no recording decorators yet).

#### Phase requirements

- (***Complete***) TestAdapter records adapter lifecycle calls
  - GIVEN a TestAdapter wrapping a HeadlessAdapter
  - WHEN `InitializeAsync` and `ShutdownAsync` are called
  - THEN the calls and arguments are recorded for assertion.

- (***Not complete***) TestAdapter records provider method calls
  - GIVEN a TestAdapter wrapping a HeadlessAdapter
  - WHEN any provider method is called
  - THEN the call and its arguments are recorded for assertion.

- (***Complete***) TestAdapter supports call assertion (for recorded calls)
  - GIVEN expected sequences of recorded calls
  - WHEN the test completes
  - THEN recorded calls can be queried and asserted for correctness.

#### Examples

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

/// <summary>
/// DTO recording a single adapter call for test assertion.
/// </summary>
/// <param name="ProviderName">Name of the provider (e.g. "Render", "Input").</param>
/// <param name="MethodName">Name of the method called.</param>
/// <param name="Arguments">Arguments passed to the method.</param>
public readonly record struct RecordedCall(
    string ProviderName,
    string MethodName,
    IReadOnlyList<object?> Arguments);
```

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Adapter for CI that wraps HeadlessAdapter and records all calls for assertion.
/// </summary>
public sealed class TestAdapter : IEngineAdapter
{
    private readonly HeadlessAdapter _inner;
    private readonly List<RecordedCall> _recordedCalls = [];

    /// <summary>Gets the recorded calls for assertion.</summary>
    public IReadOnlyList<RecordedCall> RecordedCalls => _recordedCalls;

    /// <inheritdoc />
    public EngineCapabilities Capabilities => _inner.Capabilities;

    /// <inheritdoc />
    public IRenderProvider RenderProvider => _inner.RenderProvider;

    /// <inheritdoc />
    public IInputProvider InputProvider => _inner.InputProvider;

    /// <inheritdoc />
    public IUserInterfaceProvider UserInterfaceProvider => _inner.UserInterfaceProvider;

    /// <inheritdoc />
    public IAssetProvider AssetProvider => _inner.AssetProvider;

    /// <inheritdoc />
    public IAudioPlayer AudioPlayer => _inner.AudioPlayer;

    /// <summary>
    /// Initializes a new test adapter wrapping a headless adapter.
    /// </summary>
    /// <param name="config">Adapter configuration.</param>
    public TestAdapter(EngineConfig config)
    {
        _inner = new HeadlessAdapter(config);
    }

    /// <inheritdoc />
    public Task InitializeAsync(EngineConfig config, CancellationToken ct = default)
    {
        _recordedCalls.Add(new RecordedCall("Adapter", "InitializeAsync", [config]));
        return _inner.InitializeAsync(config, ct);
    }

    /// <inheritdoc />
    public Task ShutdownAsync(CancellationToken ct = default)
    {
        _recordedCalls.Add(new RecordedCall("Adapter", "ShutdownAsync", []));
        return _inner.ShutdownAsync(ct);
    }

    /// <inheritdoc />
    public void Dispose() => _inner.Dispose();
}
```

### Phase 3 — Deterministic execution

***Partially complete***

#### Objective

Provide a deterministic execution harness for headless simulations (fixed timestep configuration and seeded RNG) to enable reproducible test runs.

#### Technical details

- **Fixed timestep**: The deterministic runner currently stores a fixed timestep value. It does not yet advance or expose a simulation clock based on this timestep.
- **Seeded RNG**: The deterministic runner initializes an RNG with the provided seed. The RNG is not yet used to drive any simulation outputs.
- **Scenario scripting**: Scripted input exists as explicit setter methods on `HeadlessInputProvider`, but there is no scenario/queue playback format yet.

#### Phase requirements

- (***Partially complete***) Fixed timestep execution
  - GIVEN a configured fixed timestep interval
  - WHEN the simulation runs N steps
  - THEN the runner executes N steps (timestep is currently stored, not applied to a simulation clock).

- (***Partially complete***) Seeded RNG reproducibility
  - GIVEN a fixed RNG seed
  - WHEN the simulation runs twice with identical inputs
  - THEN the runner completes deterministically (no non-deterministic outputs are currently produced by RNG).

#### Examples

```csharp
namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Runs a headless simulation with deterministic fixed timestep and seeded RNG.
/// </summary>
public sealed class DeterministicEngineRunner
{
    private readonly HeadlessAdapter _adapter;
    private readonly Random _rng;
    private readonly TimeSpan _fixedTimestep;

    /// <summary>
    /// Initializes a new deterministic engine runner.
    /// </summary>
    /// <param name="adapter">The headless adapter to drive.</param>
    /// <param name="seed">RNG seed for reproducibility.</param>
    /// <param name="fixedTimestep">Time interval per simulation step.</param>
    public DeterministicEngineRunner(HeadlessAdapter adapter, int seed, TimeSpan fixedTimestep)
    {
        _adapter = adapter;
        _rng = new Random(seed);
        _fixedTimestep = fixedTimestep;
    }

    /// <summary>
    /// Runs the simulation for the specified number of steps.
    /// </summary>
    /// <param name="steps">Number of simulation steps to execute.</param>
    public void Run(int steps)
    {
        var renderProvider = _adapter.RenderProvider;
        var camera = new CameraDescriptor(
            ProjectionType.Orthographic, 0f, 10f, 16f / 9f, 0.1f, 100f);

        for (var i = 0; i < steps; i++)
        {
            using var scope = renderProvider.BeginFrame(in camera);
            // Simulation logic using _rng for determinism
            renderProvider.EndFrame();
            renderProvider.Present();
        }
    }
}
```

### Testing and compatibility

- Unit tests cover each headless provider independently, plus `TestAdapter` and `DeterministicEngineRunner`.
- There is currently no AI/combat/map-generation integration scenario suite in this repository.
- Adapters are designed to be compatible with CI environments (GitHub Actions, Azure DevOps, etc.) without requiring graphics or audio hardware.
- `TestAdapter` exposes `RecordedCalls` for asserting recorded adapter lifecycle calls (provider call recording is not yet implemented).

### Performance targets

- HeadlessAdapter and TestAdapter must complete integration scenarios in under 100ms per scenario on typical CI hardware.
- No allocations or operations that would cause test flakiness or non-determinism.
- Call recording memory usage bounded by scenario size; consider clearing between scenarios.

## Known issues and design concerns

- **Integration scenarios not implemented**: The plan references AI/combat/map-generation integration scenarios, but the repository currently only contains unit tests.
- **TestAdapter provider call recording not implemented**: `TestAdapter` currently records adapter lifecycle calls only; provider calls are delegated directly.
- **Deterministic runner is a skeleton**: `DeterministicEngineRunner` stores a fixed timestep and initializes a seeded RNG, but does not yet advance a simulation clock or use RNG to drive any outputs.
- **Scenario scripting format**: `HeadlessInputProvider` supports scripted state via setter methods, but there is no queue/script playback format yet.
- **Call recording overhead**: If/when provider-call recording is added, recording all calls may impact performance for very large scenarios. Consider bounding the recording buffer.

## See also

- [Phase 1 — Interface development](./phase-1-interface-development.md)
- Parent plan: [Engine Decoupling](https://github.com/JohnLudlow/FourXGame/blob/main/docs/plans/4x-game/technical/engine-decoupling/engine-decoupling.md)

## References

- [Phase 2 issue #5](https://github.com/JohnLudlow/GameEngineAdapter/issues/5)
- [Phase 1 issue #1](https://github.com/JohnLudlow/GameEngineAdapter/issues/1)
