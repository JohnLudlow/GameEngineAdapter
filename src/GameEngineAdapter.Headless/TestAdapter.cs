namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// A test-friendly adapter that wraps <see cref="HeadlessAdapter"/> and records all provider
/// and lifecycle method calls to <see cref="RecordedCalls"/> for assertion in unit tests.
/// </summary>
public sealed class TestAdapter : IEngineAdapter
{
    private readonly HeadlessAdapter _inner;
    private readonly List<RecordedCall> _recordedCalls = [];

    private readonly RecordingRenderProvider _renderProvider;
    private readonly RecordingInputProvider _inputProvider;
    private readonly RecordingAssetProvider _assetProvider;
    private readonly RecordingAudioPlayer _audioPlayer;

    /// <summary>Gets the underlying <see cref="HeadlessAdapter"/> instance.</summary>
    public HeadlessAdapter Inner => _inner;

    /// <summary>Gets the ordered log of all recorded provider and lifecycle calls.</summary>
    public IReadOnlyList<RecordedCall> RecordedCalls => _recordedCalls;

    /// <inheritdoc />
    public EngineCapabilities Capabilities => _inner.Capabilities;

    /// <inheritdoc />
    public IRenderProvider RenderProvider => _renderProvider;

    /// <inheritdoc />
    public IInputProvider InputProvider => _inputProvider;

    /// <inheritdoc />
    public IUserInterfaceProvider UserInterfaceProvider => _inner.UserInterfaceProvider;

    /// <inheritdoc />
    public IAssetProvider AssetProvider => _assetProvider;

    /// <inheritdoc />
    public IAudioPlayer AudioPlayer => _audioPlayer;

    /// <summary>
    /// Initializes a new <see cref="TestAdapter"/> wrapping a <see cref="HeadlessAdapter"/>
    /// created from the specified configuration.
    /// </summary>
    /// <param name="config">Adapter configuration.</param>
    public TestAdapter(EngineConfig config)
    {
        _inner = new HeadlessAdapter(config);
        _renderProvider = new RecordingRenderProvider(_inner.RenderProvider, _recordedCalls);
        _inputProvider = new RecordingInputProvider(_inner.InputProvider, _recordedCalls);
        _assetProvider = new RecordingAssetProvider(_inner.AssetProvider, _recordedCalls);
        _audioPlayer = new RecordingAudioPlayer(_inner.AudioPlayer, _recordedCalls);
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
