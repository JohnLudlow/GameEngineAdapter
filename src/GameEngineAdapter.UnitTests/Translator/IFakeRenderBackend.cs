namespace JohnLudlow.GameEngineAdapter.UnitTests.Translator;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Minimal fake engine-facing render API used by translator tests.
/// This allows verifying DTO-to-engine-call mapping without depending on a real engine SDK.
/// </summary>
public interface IFakeRenderBackend
{
    void BeginFrame(in CameraDescriptor camera);

    void DrawSprite(string spriteId, in TransformDto transform, in MaterialDto material, int layer);

    void DrawText(
        string text,
        in TransformDto transform,
        string fontId,
        float fontSize,
        in MaterialDto material,
        int layer);

    void DrawMesh(string meshId, in TransformDto transform, in MaterialDto material, int layer);

    void EndFrame();

    void Present();
}
