namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public class TestAdapterTests
{
    private static EngineConfig MakeConfig() =>
        new("Test", null, null);

    [Fact]
    public void TestAdapter_Constructor_WrapsHeadlessAdapter()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert — capabilities should match headless defaults
        Assert.True(adapter.Capabilities.Supports2D);
    }

    [Fact]
    public void TestAdapter_RenderProvider_DelegatesToInner()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.RenderProvider);
    }

    [Fact]
    public void TestAdapter_InputProvider_DelegatesToInner()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.InputProvider);
    }

    [Fact]
    public void TestAdapter_UserInterfaceProvider_DelegatesToInner()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.UserInterfaceProvider);
    }

    [Fact]
    public void TestAdapter_AssetProvider_DelegatesToInner()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.AssetProvider);
    }

    [Fact]
    public void TestAdapter_AudioPlayer_DelegatesToInner()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.AudioPlayer);
    }

    [Fact]
    public void TestAdapter_RecordedCalls_InitiallyEmpty()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.Empty(adapter.RecordedCalls);
    }

    [Fact]
    public async Task TestAdapter_InitializeAsync_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());
        var config = MakeConfig();

        // Act
        await adapter.InitializeAsync(config);

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Adapter", call.ProviderName);
        Assert.Equal("InitializeAsync", call.MethodName);
    }

    [Fact]
    public async Task TestAdapter_ShutdownAsync_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        await adapter.ShutdownAsync();

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Adapter", call.ProviderName);
        Assert.Equal("ShutdownAsync", call.MethodName);
    }

    [Fact]
    public async Task TestAdapter_RecordedCalls_ReflectsCallSequence()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        await adapter.InitializeAsync(MakeConfig());
        await adapter.ShutdownAsync();

        // Assert
        Assert.Equal(2, adapter.RecordedCalls.Count);
        Assert.Equal("InitializeAsync", adapter.RecordedCalls[0].MethodName);
        Assert.Equal("ShutdownAsync", adapter.RecordedCalls[1].MethodName);
    }

    [Fact]
    public async Task TestAdapter_InitializeAsync_RecordsConfigAsArgument()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());
        var config = new EngineConfig("SpecificAdapter", "/res", null);

        // Act
        await adapter.InitializeAsync(config);

        // Assert
        var call = adapter.RecordedCalls[0];
        Assert.Single(call.Arguments);
        Assert.Equal(config, call.Arguments[0]);
    }

    [Fact]
    public void TestAdapter_Dispose_DoesNotThrow()
    {
        // Arrange
        var adapter = new TestAdapter(MakeConfig());

        // Act / Assert
        adapter.Dispose();
    }
}
