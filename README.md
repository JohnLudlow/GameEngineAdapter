# GameEngineAdapter

> [!TIP]
> **Adapter contracts for decoupled game engines**

GameEngineAdapter provides a set of stable, minimal C# interfaces and DTOs for building engine-agnostic game logic and adapters. It enables game code to run on multiple engines (MonoGame, Stride, Raylib, TUI, Headless, etc.) by defining a clear boundary between simulation and engine bindings for rendering, input, audio, and asset management.

**Why use this project?**

- Write game logic once, run it on any supported engine
- Clean separation of concerns for testability and portability
- Minimal, allocation-friendly DTOs and interfaces for high performance

## IMPORTANT

> [!IMPORTANT]
> This project is under active development. The interface contracts are not yet stable and breaking changes may occur before v1.0. See [Phase 1 plan](docs/plans/phase-1-interface-development.md) for current status.

## Features

- **Engine-agnostic adapter contracts**: Interfaces for rendering, input, audio, UI, and asset management
- **Minimal, allocation-friendly DTOs**: Compact structs for draw commands, transforms, and materials
- **Capability negotiation**: Adapters advertise supported features (2D, 3D, audio, etc.) at runtime
- **Extensible provider model**: Obtain engine-specific providers for rendering, input, UI, and assets
- **Headless/test adapters**: Support for deterministic simulation and CI testing

## Roadmap

- [Phase 1: Interface development](docs/plans/phase-1-interface-development.md) ([#2](https://github.com/JohnLudlow/GameEngineAdapter/issues/2))
  - Define stable adapter contracts and DTOs
  - Document interfaces and provide minimal working examples
  - Add headless/test adapters for CI
- Future: Engine-specific adapters (MonoGame, Stride, Raylib, etc.)

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/JohnLudlow/GameEngineAdapter.git
   cd GameEngineAdapter
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Build the project:

   ```bash
   dotnet build
   ```

## Usage

This is a class library. Reference it from your engine adapter or game project:

```xml
<ItemGroup>
  <ProjectReference Include="../GameEngineAdapter/GameEngineAdapter.csproj" />
</ItemGroup>
```

### Example: Implementing an Adapter

```csharp
public class MyEngineAdapter : IEngineAdapter
{
    public EngineCapabilities Capabilities => new EngineCapabilities { Supports2D = true, ContractVersion = "1.0.0" };
    public Task InitializeAsync(EngineConfig config, CancellationToken ct = default) => Task.CompletedTask;
    public Task ShutdownAsync(CancellationToken ct = default) => Task.CompletedTask;
    public IRenderProvider GetRenderProvider() => new MyRenderProvider();
    // ...other providers
    public void Dispose() { }
}
```

See [docs/plans/phase-1-interface-development.md](docs/plans/phase-1-interface-development.md) for more API examples.

## Configuration

Adapters may accept configuration via an `EngineConfig` object. See the plan for details. No global configuration is required for the library itself.

## Contributing

Contributions are welcome! Please:

- [Open issues](https://github.com/JohnLudlow/GameEngineAdapter/issues) for bugs, feature requests, or questions
- Fork the repo and create feature branches for your changes
- Follow the [plan](docs/plans/phase-1-interface-development.md) for interface and adapter contributions
- Add or update tests for new features
- Submit a pull request with a clear description

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

- Project Link: [https://github.com/JohnLudlow/GameEngineAdapter](https://github.com/JohnLudlow/GameEngineAdapter)
