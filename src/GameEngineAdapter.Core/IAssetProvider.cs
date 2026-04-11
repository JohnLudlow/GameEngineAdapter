namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Adapter-owned provider for asset lifecycle management, caching, and querying.
/// Does not load assets directly — delegates to <see cref="IAssetLoader"/> for I/O.
/// </summary>
public interface IAssetProvider
{
    /// <summary>Returns the asset loader used by this provider.</summary>
    IAssetLoader Loader { get; }

    /// <summary>Returns a previously loaded asset by identifier, or null if not cached.</summary>
    object? GetAsset(string assetId);

    /// <summary>Unloads a previously loaded asset and removes it from the cache.</summary>
    void UnloadAsset(string assetId);

    /// <summary>Returns true if the specified asset is currently loaded and cached.</summary>
    bool IsAssetLoaded(string assetId);

    /// <summary>Evicts all cached assets.</summary>
    void ClearCache();
}
