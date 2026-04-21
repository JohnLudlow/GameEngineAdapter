namespace JohnLudlow.GameEngineAdapter.UnitTests.Translator;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Test-only provider that translates adapter DTO submissions into an engine-facing backend API.
/// Translation is intentionally explicit and minimal (pass-through) to validate mapping patterns.
/// </summary>
public sealed class TranslatingRenderProvider(IFakeRenderBackend backend) : IRenderProvider
{
    public FrameScope BeginFrame(in CameraDescriptor camera)
    {
        backend.BeginFrame(in camera);
        return new FrameScope();
    }

    public void SubmitSprite(in SpriteDrawDto dto)
    {
        var transform = dto.Transform;
        var material = dto.Material;
        backend.DrawSprite(dto.SpriteId, in transform, in material, dto.Layer);
    }

    public void SubmitText(in TextDrawDto dto)
    {
        var transform = dto.Transform;
        var material = dto.Material;
        backend.DrawText(dto.Text, in transform, dto.FontId, dto.FontSize, in material, dto.Layer);
    }

    public void SubmitMesh(in MeshDrawDto dto)
    {
        var transform = dto.Transform;
        var material = dto.Material;
        backend.DrawMesh(dto.MeshId, in transform, in material, dto.Layer);
    }

    public void EndFrame() => backend.EndFrame();

    public void Present() => backend.Present();
}
