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
