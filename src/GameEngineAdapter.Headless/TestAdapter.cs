namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

public sealed class TestAdapter : IEngineAdapter
{
    private readonly HeadlessAdapter _inner;
    private readonly List<RecordedCall> _recordedCalls = [];

    private readonly RecordingRenderProvider _renderProvider;
    private readonly RecordingInputProvider _inputProvider;
    private readonly RecordingAssetProvider _assetProvider;
    private readonly RecordingAudioPlayer _audioPlayer;

    public IReadOnlyList<RecordedCall> RecordedCalls => _recordedCalls;

    public EngineCapabilities Capabilities => _inner.Capabilities;

    public IRenderProvider RenderProvider => _renderProvider;

    public IInputProvider InputProvider => _inputProvider;

    public IUserInterfaceProvider UserInterfaceProvider => _inner.UserInterfaceProvider;

    public IAssetProvider AssetProvider => _assetProvider;

    public IAudioPlayer AudioPlayer => _audioPlayer;

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
