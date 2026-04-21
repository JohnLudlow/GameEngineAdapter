namespace JohnLudlow.GameEngineAdapter.UnitTests.Translator;

using JohnLudlow.GameEngineAdapter.Core;

public readonly record struct FakeBackendCall(
    string MethodName,
    IReadOnlyList<object?> Arguments);

/// <summary>
/// Fake render backend that records all calls for assertion.
/// </summary>
public sealed class RecordingFakeRenderBackend : IFakeRenderBackend
{
    private readonly List<FakeBackendCall> _calls = [];

    public IReadOnlyList<FakeBackendCall> Calls => _calls;

    public void BeginFrame(in CameraDescriptor camera) =>
        _calls.Add(new FakeBackendCall("BeginFrame", [camera]));

    public void DrawSprite(string spriteId, in TransformDto transform, in MaterialDto material, int layer) =>
        _calls.Add(new FakeBackendCall("DrawSprite", [spriteId, transform, material, layer]));

    public void DrawText(
        string text,
        in TransformDto transform,
        string fontId,
        float fontSize,
        in MaterialDto material,
        int layer) =>
        _calls.Add(new FakeBackendCall("DrawText", [text, transform, fontId, fontSize, material, layer]));

    public void DrawMesh(string meshId, in TransformDto transform, in MaterialDto material, int layer) =>
        _calls.Add(new FakeBackendCall("DrawMesh", [meshId, transform, material, layer]));

    public void EndFrame() =>
        _calls.Add(new FakeBackendCall("EndFrame", []));

    public void Present() =>
        _calls.Add(new FakeBackendCall("Present", []));
}
