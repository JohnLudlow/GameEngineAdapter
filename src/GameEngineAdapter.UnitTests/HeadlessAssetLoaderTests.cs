namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessAssetLoaderTests
{
    [Fact]
    public void HeadlessAssetLoader_Load_ReturnsNonNull()
    {
        // Arrange
        var loader = new HeadlessAssetLoader();

        // Act
        var result = loader.Load("texture_hero");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void HeadlessAssetLoader_Load_ReturnsDifferentInstancesPerCall()
    {
        // Arrange
        var loader = new HeadlessAssetLoader();

        // Act
        var a = loader.Load("asset_a");
        var b = loader.Load("asset_b");

        // Assert — each call produces a fresh object
        Assert.NotSame(a, b);
    }

    [Fact]
    public async Task HeadlessAssetLoader_LoadAsync_ReturnsNonNull()
    {
        // Arrange
        var loader = new HeadlessAssetLoader();

        // Act
        var result = await loader.LoadAsync("texture_background");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task HeadlessAssetLoader_LoadAsync_Completes()
    {
        // Arrange
        var loader = new HeadlessAssetLoader();

        // Act / Assert — must not throw or hang
        await loader.LoadAsync("audio_sfx");
    }
}
