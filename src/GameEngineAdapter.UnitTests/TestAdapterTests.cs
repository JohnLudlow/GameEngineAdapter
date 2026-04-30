namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
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

    [Fact]
    public void TestAdapter_Inner_IsNotNull()
    {
        // Arrange / Act
        using var adapter = new TestAdapter(MakeConfig());

        // Assert
        Assert.NotNull(adapter.Inner);
    }

    [Fact]
    public void TestAdapter_RenderProvider_SubmitSprite_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());
        var transform = new TransformDto(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
        var material = new MaterialDto("default", new Dictionary<string, object>(), []);
        var dto = new SpriteDrawDto("sprite_hero", transform, material, 0);

        // Act
        adapter.RenderProvider.SubmitSprite(dto);

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Render", call.ProviderName);
        Assert.Equal("SubmitSprite", call.MethodName);
        Assert.Single(call.Arguments);
        Assert.Equal(dto, call.Arguments[0]);
    }

    [Fact]
    public void TestAdapter_InputProvider_IsKeyDown_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        adapter.InputProvider.IsKeyDown("Space");

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Input", call.ProviderName);
        Assert.Equal("IsKeyDown", call.MethodName);
        Assert.Single(call.Arguments);
        Assert.Equal("Space", call.Arguments[0]);
    }

    [Fact]
    public void TestAdapter_AssetProvider_UnloadAsset_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        adapter.AssetProvider.UnloadAsset("texture_sky");

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Asset", call.ProviderName);
        Assert.Equal("UnloadAsset", call.MethodName);
        Assert.Single(call.Arguments);
        Assert.Equal("texture_sky", call.Arguments[0]);
    }

    [Fact]
    public void TestAdapter_AudioPlayer_StartPlayback_RecordsCall()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        adapter.AudioPlayer.StartPlayback("music_main", loopPlayback: true);

        // Assert
        Assert.Single(adapter.RecordedCalls);
        var call = adapter.RecordedCalls[0];
        Assert.Equal("Audio", call.ProviderName);
        Assert.Equal("StartPlayback", call.MethodName);
        Assert.Equal(2, call.Arguments.Count);
        Assert.Equal("music_main", call.Arguments[0]);
        Assert.Equal(true, call.Arguments[1]);
    }

    [Fact]
    public async Task TestAdapter_RecordedCalls_CrossProviderSequence()
    {
        // Arrange
        using var adapter = new TestAdapter(MakeConfig());

        // Act
        await adapter.InitializeAsync(MakeConfig());
        adapter.InputProvider.IsKeyDown("Space");
        adapter.AudioPlayer.StartPlayback("music_main");
        await adapter.ShutdownAsync();

        // Assert — four calls in order across providers
        Assert.Equal(4, adapter.RecordedCalls.Count);
        Assert.Equal("InitializeAsync", adapter.RecordedCalls[0].MethodName);
        Assert.Equal("IsKeyDown", adapter.RecordedCalls[1].MethodName);
        Assert.Equal("StartPlayback", adapter.RecordedCalls[2].MethodName);
        Assert.Equal("ShutdownAsync", adapter.RecordedCalls[3].MethodName);
    }
}
