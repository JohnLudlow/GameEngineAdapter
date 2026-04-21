namespace JohnLudlow.GameEngineAdapter.UnitTests.Translator;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class TranslatingRenderProviderTests
{
    private static TransformDto MakeTransform(float x = 1f, float y = 2f, float z = 3f) =>
        new(x, y, z, RotationX: 10f, RotationY: 20f, RotationZ: 30f, ScaleX: 1f, ScaleY: 2f, ScaleZ: 3f);

    private static MaterialDto MakeMaterial(string shaderId)
    {
        var uniforms = new Dictionary<string, object>
        {
            ["u_color"] = "red",
            ["u_alpha"] = 0.75f,
        };

        return new MaterialDto(shaderId, uniforms, [0, 2]);
    }

    private static CameraDescriptor MakeCamera() =>
        new(ProjectionType.Orthographic, FieldOfViewDegrees: 0f, OrthographicSize: 10f, AspectRatio: 16f / 9f, NearPlane: 0.1f, FarPlane: 100f);

    [Fact]
    public void SubmitSprite_TranslatesToBackendCall_WithEquivalentValues()
    {
        // Arrange
        var backend = new RecordingFakeRenderBackend();
        var provider = new TranslatingRenderProvider(backend);

        var transform = MakeTransform();
        var material = MakeMaterial("shader_sprite");
        var dto = new SpriteDrawDto("sprite_1", transform, material, Layer: 42);

        // Act
        provider.SubmitSprite(in dto);

        // Assert
        Assert.Single(backend.Calls);
        var call = backend.Calls[0];
        Assert.Equal("DrawSprite", call.MethodName);

        Assert.Equal("sprite_1", call.Arguments[0]);
        Assert.Equal(transform, call.Arguments[1]);
        Assert.Equal(material, call.Arguments[2]);
        Assert.Equal(42, call.Arguments[3]);

        // And ensure reference-typed members are forwarded (not deep-copied)
        var forwardedMaterial = (MaterialDto)call.Arguments[2]!;
        Assert.Same(material.Uniforms, forwardedMaterial.Uniforms);
    }

    [Fact]
    public void SubmitText_TranslatesToBackendCall_WithEquivalentValues()
    {
        // Arrange
        var backend = new RecordingFakeRenderBackend();
        var provider = new TranslatingRenderProvider(backend);

        var transform = MakeTransform(x: 9f, y: 8f, z: 7f);
        var material = MakeMaterial("shader_text");
        var dto = new TextDrawDto("Hello", transform, FontId: "font_main", FontSize: 13.5f, material, Layer: 3);

        // Act
        provider.SubmitText(in dto);

        // Assert
        Assert.Single(backend.Calls);
        var call = backend.Calls[0];
        Assert.Equal("DrawText", call.MethodName);

        Assert.Equal("Hello", call.Arguments[0]);
        Assert.Equal(transform, call.Arguments[1]);
        Assert.Equal("font_main", call.Arguments[2]);
        Assert.Equal(13.5f, call.Arguments[3]);
        Assert.Equal(material, call.Arguments[4]);
        Assert.Equal(3, call.Arguments[5]);
    }

    [Fact]
    public void SubmitMesh_TranslatesToBackendCall_WithEquivalentValues()
    {
        // Arrange
        var backend = new RecordingFakeRenderBackend();
        var provider = new TranslatingRenderProvider(backend);

        var transform = MakeTransform();
        var material = MakeMaterial("shader_mesh");
        var dto = new MeshDrawDto("mesh_tree", transform, material, Layer: 1);

        // Act
        provider.SubmitMesh(in dto);

        // Assert
        Assert.Single(backend.Calls);
        var call = backend.Calls[0];
        Assert.Equal("DrawMesh", call.MethodName);

        Assert.Equal("mesh_tree", call.Arguments[0]);
        Assert.Equal(transform, call.Arguments[1]);
        Assert.Equal(material, call.Arguments[2]);
        Assert.Equal(1, call.Arguments[3]);
    }

    [Fact]
    public void MultipleSubmits_AreRecordedInOrder()
    {
        // Arrange
        var backend = new RecordingFakeRenderBackend();
        var provider = new TranslatingRenderProvider(backend);

        // Act
        var sprite = new SpriteDrawDto("s", MakeTransform(), MakeMaterial("m1"), Layer: 0);
        var text = new TextDrawDto("t", MakeTransform(), "f", 12f, MakeMaterial("m2"), Layer: 1);
        var mesh = new MeshDrawDto("m", MakeTransform(), MakeMaterial("m3"), Layer: 2);

        provider.SubmitSprite(in sprite);
        provider.SubmitText(in text);
        provider.SubmitMesh(in mesh);

        // Assert
        Assert.Equal(3, backend.Calls.Count);
        Assert.Equal("DrawSprite", backend.Calls[0].MethodName);
        Assert.Equal("DrawText", backend.Calls[1].MethodName);
        Assert.Equal("DrawMesh", backend.Calls[2].MethodName);
    }

    [Fact]
    public void FrameBoundaryCalls_AreForwarded()
    {
        // Arrange
        var backend = new RecordingFakeRenderBackend();
        var provider = new TranslatingRenderProvider(backend);
        var camera = MakeCamera();

        // Act
        using var scope = provider.BeginFrame(in camera);
        provider.EndFrame();
        provider.Present();

        // Assert
        Assert.Equal(3, backend.Calls.Count);
        Assert.Equal("BeginFrame", backend.Calls[0].MethodName);
        Assert.Equal(camera, backend.Calls[0].Arguments[0]);
        Assert.Equal("EndFrame", backend.Calls[1].MethodName);
        Assert.Equal("Present", backend.Calls[2].MethodName);
    }
}
