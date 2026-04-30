namespace JohnLudlow.GameEngineAdapter.Headless;

using System.Collections.ObjectModel;
using JohnLudlow.GameEngineAdapter.Core;

using System.Collections.ObjectModel;
using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Recording decorator for <see cref="IRenderProvider"/> that logs all method calls.
/// </summary>
public sealed class RecordingRenderProvider : IRenderProvider
{
    private readonly IRenderProvider _inner;
    private readonly IList<RecordedCall> _recordedCalls;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingRenderProvider"/> class.
    /// </summary>
    /// <param name="inner">The underlying render provider to wrap.</param>
    /// <param name="recordedCalls">The list to record method calls into.</param>
    public RecordingRenderProvider(IRenderProvider inner, IReadOnlyList<RecordedCall> recordedCalls)
    {
        _inner = inner;
        _recordedCalls = (IList<RecordedCall>)recordedCalls;
    }

    /// <inheritdoc />
    public FrameScope BeginFrame(in CameraDescriptor camera)
    {
        _recordedCalls.Add(new RecordedCall("Render", "BeginFrame", [camera]));
        return _inner.BeginFrame(in camera);
    }

    /// <inheritdoc />
    public void SubmitSprite(in SpriteDrawDto dto)
    {
        _recordedCalls.Add(new RecordedCall("Render", "SubmitSprite", [dto]));
        _inner.SubmitSprite(in dto);
    }

    /// <inheritdoc />
    public void SubmitText(in TextDrawDto dto)
    {
        _recordedCalls.Add(new RecordedCall("Render", "SubmitText", [dto]));
        _inner.SubmitText(in dto);
    }

    /// <inheritdoc />
    public void SubmitMesh(in MeshDrawDto dto)
    {
        _recordedCalls.Add(new RecordedCall("Render", "SubmitMesh", [dto]));
        _inner.SubmitMesh(in dto);
    }

    /// <inheritdoc />
    public void EndFrame()
    {
        _recordedCalls.Add(new RecordedCall("Render", "EndFrame", []));
        _inner.EndFrame();
    }

    /// <inheritdoc />
    public void Present()
    {
        _recordedCalls.Add(new RecordedCall("Render", "Present", []));
        _inner.Present();
    }
}