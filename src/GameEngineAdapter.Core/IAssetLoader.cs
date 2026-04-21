namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Interface for loading assets from storage (disk, network, embedded resources).
/// Separate from <see cref="IAssetProvider"/> which handles caching and lifecycle.
/// </summary>
public interface IAssetLoader
{
    /// <summary>Asynchronously loads an asset by identifier.</summary>
    Task<object> LoadAsync(string assetId, CancellationToken ct = default);

    /// <summary>Synchronously loads an asset by identifier.</summary>
    object Load(string assetId);
}
