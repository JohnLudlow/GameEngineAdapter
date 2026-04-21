namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessAdapterTests
{
    private static EngineConfig MakeConfig() =>
        new("Headless", null, null);

    [Fact]
    public void HeadlessAdapter_Constructor_StoresConfig()
    {
        // Arrange
        var config = MakeConfig();

        // Act
        using var adapter = new HeadlessAdapter(config);

        // Assert
        Assert.Equal(config, adapter.Config);
    }

    [Fact]
    public void HeadlessAdapter_Capabilities_Supports2D()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.True(adapter.Capabilities.Supports2D);
    }

    [Fact]
    public void HeadlessAdapter_Capabilities_ContractVersionIsSet()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.False(string.IsNullOrEmpty(adapter.Capabilities.ContractVersion));
    }

    [Fact]
    public void HeadlessAdapter_RenderProvider_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.RenderProvider);
    }

    [Fact]
    public void HeadlessAdapter_InputProvider_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.InputProvider);
    }

    [Fact]
    public void HeadlessAdapter_UserInterfaceProvider_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.UserInterfaceProvider);
    }

    [Fact]
    public void HeadlessAdapter_AssetProvider_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.AssetProvider);
    }

    [Fact]
    public void HeadlessAdapter_AudioPlayer_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.AudioPlayer);
    }

    [Fact]
    public async Task HeadlessAdapter_InitializeAsync_Completes()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Act / Assert — must not throw
        await adapter.InitializeAsync(MakeConfig());
    }

    [Fact]
    public async Task HeadlessAdapter_ShutdownAsync_Completes()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());

        // Act / Assert — must not throw
        await adapter.ShutdownAsync();
    }

    [Fact]
    public void HeadlessAdapter_Dispose_DoesNotThrow()
    {
        // Arrange
        var adapter = new HeadlessAdapter(MakeConfig());

        // Act / Assert — must not throw
        adapter.Dispose();
    }
}
