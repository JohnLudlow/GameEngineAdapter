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
