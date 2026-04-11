namespace JohnLudlow.GameEngineAdapter;


/// <summary>
/// Configuration data for initializing an engine adapter.
/// </summary>
/// <param name="AdapterName">Adapter type or engine backend identifier (e.g. "MonoGame", "Stride").</param>
/// <param name="ResourcePath">Optional path to engine resources or platform binaries.</param>
/// <param name="Options">Optional key-value configuration entries.</param>
public readonly record struct EngineConfig(
    string AdapterName,
    string? ResourcePath,
    IReadOnlyDictionary<string, object>? Options);
