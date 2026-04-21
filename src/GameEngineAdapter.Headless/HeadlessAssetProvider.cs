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
