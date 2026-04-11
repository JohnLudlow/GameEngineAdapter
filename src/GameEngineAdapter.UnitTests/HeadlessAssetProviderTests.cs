namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessAssetProviderTests
{
    [Fact]
    public void HeadlessAssetProvider_Loader_IsNotNull()
    {
        // Arrange / Act
        var provider = new HeadlessAssetProvider();

        // Assert
        Assert.NotNull(provider.Loader);
    }

    [Fact]
    public void HeadlessAssetProvider_GetAsset_UnknownId_ReturnsNull()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();

        // Act
        var result = provider.GetAsset("nonexistent_asset");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HeadlessAssetProvider_LoadAndCache_CachesAsset()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();

        // Act
        provider.LoadAndCache("texture_sky");
        var result = provider.GetAsset("texture_sky");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void HeadlessAssetProvider_IsAssetLoaded_ReturnsFalseBeforeLoad()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();

        // Act / Assert
        Assert.False(provider.IsAssetLoaded("font_arial"));
    }

    [Fact]
    public void HeadlessAssetProvider_IsAssetLoaded_ReturnsTrueAfterLoad()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();
        provider.LoadAndCache("font_arial");

        // Act / Assert
        Assert.True(provider.IsAssetLoaded("font_arial"));
    }

    [Fact]
    public void HeadlessAssetProvider_UnloadAsset_RemovesFromCache()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();
        provider.LoadAndCache("mesh_tree");

        // Act
        provider.UnloadAsset("mesh_tree");

        // Assert
        Assert.False(provider.IsAssetLoaded("mesh_tree"));
        Assert.Null(provider.GetAsset("mesh_tree"));
    }

    [Fact]
    public void HeadlessAssetProvider_ClearCache_RemovesAllAssets()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();
        provider.LoadAndCache("sprite_hero");
        provider.LoadAndCache("sprite_enemy");

        // Act
        provider.ClearCache();

        // Assert
        Assert.False(provider.IsAssetLoaded("sprite_hero"));
        Assert.False(provider.IsAssetLoaded("sprite_enemy"));
    }

    [Fact]
    public void HeadlessAssetProvider_LoadAndCache_CalledTwice_DoesNotReplaceExisting()
    {
        // Arrange
        var provider = new HeadlessAssetProvider();
        provider.LoadAndCache("texture_rock");
        var first = provider.GetAsset("texture_rock");

        // Act — load again, should not replace
        provider.LoadAndCache("texture_rock");
        var second = provider.GetAsset("texture_rock");

        // Assert — same instance returned both times
        Assert.Same(first, second);
    }
}
