namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Root interface for an engine adapter.
/// </summary>
public interface IEngineAdapter : IDisposable
{
  /// <summary>Gets the advertised capabilities of the engine adapter.</summary>
  EngineCapabilities Capabilities { get; }

  /// <summary>Initializes the engine adapter with the specified configuration.</summary>
  Task InitializeAsync(EngineConfig config, CancellationToken ct = default);

  /// <summary>Shuts down the engine adapter and releases resources.</summary>
  Task ShutdownAsync(CancellationToken ct = default);

  /// <summary>Gets the provider for rendering operations.</summary>
  IRenderProvider RenderProvider { get; }

  /// <summary>Gets the provider for polling input state.</summary>
  IInputProvider InputProvider { get; }

  /// <summary>Gets the provider for user interface operations.</summary>
  IUserInterfaceProvider UserInterfaceProvider { get; }

  /// <summary>Gets the provider for asset management.</summary>
  IAssetProvider AssetProvider { get; }

  /// <summary>Gets the provider for audio playback.</summary>
  IAudioPlayer AudioPlayer { get; }
}
