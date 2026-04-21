namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessRenderProviderTests
{
    private static CameraDescriptor MakeCamera() =>
        new(ProjectionType.Orthographic, 0f, 10f, 16f / 9f, 0.1f, 100f);

    private static TransformDto MakeTransform() =>
        new(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);

    private static MaterialDto MakeMaterial() =>
        new("default", new Dictionary<string, object>(), []);

    [Fact]
    public void HeadlessRenderProvider_RecordedCommands_InitiallyEmpty()
    {
        // Arrange / Act
        var provider = new HeadlessRenderProvider();

        // Assert
        Assert.Empty(provider.RecordedSprites);
        Assert.Empty(provider.RecordedTexts);
        Assert.Empty(provider.RecordedMeshes);
    }

    [Fact]
    public void HeadlessRenderProvider_SubmitSprite_RecordsCommand()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();
        var dto = new SpriteDrawDto("sprite_test", MakeTransform(), MakeMaterial(), 0);

        // Act
        provider.SubmitSprite(dto);

        // Assert
        Assert.Single(provider.RecordedSprites);
        Assert.Equal(dto, provider.RecordedSprites[0]);
    }

    [Fact]
    public void HeadlessRenderProvider_SubmitText_RecordsCommand()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();
        var dto = new TextDrawDto("Hello", MakeTransform(), "font", 12f, MakeMaterial(), 0);

        // Act
        provider.SubmitText(dto);

        // Assert
        Assert.Single(provider.RecordedTexts);
        Assert.Equal(dto, provider.RecordedTexts[0]);
    }

    [Fact]
    public void HeadlessRenderProvider_SubmitMesh_RecordsCommand()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();
        var dto = new MeshDrawDto("mesh_test", MakeTransform(), MakeMaterial(), 0);

        // Act
        provider.SubmitMesh(dto);

        // Assert
        Assert.Single(provider.RecordedMeshes);
        Assert.Equal(dto, provider.RecordedMeshes[0]);
    }

    [Fact]
    public void HeadlessRenderProvider_MultipleSubmits_RecordsAllCommands()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();

        // Act
        provider.SubmitSprite(new SpriteDrawDto("s1", MakeTransform(), MakeMaterial(), 0));
        provider.SubmitSprite(new SpriteDrawDto("s2", MakeTransform(), MakeMaterial(), 1));
        provider.SubmitText(new TextDrawDto("Hi", MakeTransform(), "f", 10f, MakeMaterial(), 2));

        // Assert
        Assert.Equal(2, provider.RecordedSprites.Count);
        Assert.Single(provider.RecordedTexts);
        Assert.Empty(provider.RecordedMeshes);
    }

    [Fact]
    public void HeadlessRenderProvider_Clear_RemovesAllCommands()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();
        provider.SubmitSprite(new SpriteDrawDto("s", MakeTransform(), MakeMaterial(), 0));
        provider.SubmitMesh(new MeshDrawDto("m", MakeTransform(), MakeMaterial(), 0));

        // Act
        provider.Clear();

        // Assert
        Assert.Empty(provider.RecordedSprites);
        Assert.Empty(provider.RecordedTexts);
        Assert.Empty(provider.RecordedMeshes);
    }

    [Fact]
    public void HeadlessRenderProvider_BeginFrame_ReturnsFrameScope()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();
        var camera = MakeCamera();

        // Act
        var scope = provider.BeginFrame(in camera);

        // Assert — FrameScope is a struct, just verify it doesn't throw and is disposable
        scope.Dispose();
    }

    [Fact]
    public void HeadlessRenderProvider_EndFrame_DoesNotThrow()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();

        // Act / Assert
        provider.EndFrame();
    }

    [Fact]
    public void HeadlessRenderProvider_Present_DoesNotThrow()
    {
        // Arrange
        var provider = new HeadlessRenderProvider();

        // Act / Assert
        provider.Present();
    }
}
