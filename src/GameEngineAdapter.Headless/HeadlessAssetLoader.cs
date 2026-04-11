namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Stub asset loader that returns placeholder objects for any requested identifier.
/// </summary>
public sealed class HeadlessAssetLoader : IAssetLoader
{
    /// <inheritdoc />
    public Task<object> LoadAsync(string assetId, CancellationToken ct = default) =>
        Task.FromResult(new object());

    /// <inheritdoc />
    public object Load(string assetId) => new();
}
