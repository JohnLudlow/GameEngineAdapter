namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Capabilities variant for headless or test-oriented adapters.
/// </summary>
/// <param name="Base">Base engine capabilities.</param>
/// <param name="SupportsOffscreenRendering">Indicates if offscreen rendering/capture is supported.</param>
/// <param name="DeterministicTick">Indicates if the engine supports a deterministic update tick.</param>
public readonly record struct HeadlessEngineCapabilities(
    EngineCapabilities Base,
    bool SupportsOffscreenRendering,
    bool DeterministicTick);
