namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Recording decorator for <see cref="IAssetProvider"/> that logs all method calls.
/// </summary>
public sealed class RecordingAssetProvider : IAssetProvider
{
    private readonly IAssetProvider _inner;
    private readonly IList<RecordedCall> _recordedCalls;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingAssetProvider"/> class.
    /// </summary>
    /// <param name="inner">The underlying asset provider to wrap.</param>
    /// <param name="recordedCalls">The list to record method calls into.</param>
    public RecordingAssetProvider(IAssetProvider inner, IList<RecordedCall> recordedCalls)
    {
        _inner = inner;
        _recordedCalls = recordedCalls;
    }

    /// <inheritdoc />
    public IAssetLoader Loader => _inner.Loader;

    /// <inheritdoc />
    public object? GetAsset(string assetId)
    {
        _recordedCalls.Add(new RecordedCall("Asset", "GetAsset", [assetId]));
        return _inner.GetAsset(assetId);
    }

    /// <inheritdoc />
    public void UnloadAsset(string assetId)
    {
        _recordedCalls.Add(new RecordedCall("Asset", "UnloadAsset", [assetId]));
        _inner.UnloadAsset(assetId);
    }

    /// <inheritdoc />
    public bool IsAssetLoaded(string assetId)
    {
        _recordedCalls.Add(new RecordedCall("Asset", "IsAssetLoaded", [assetId]));
        return _inner.IsAssetLoaded(assetId);
    }

    /// <inheritdoc />
    public void ClearCache()
    {
        _recordedCalls.Add(new RecordedCall("Asset", "ClearCache", []));
        _inner.ClearCache();
    }
}