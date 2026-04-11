using JohnLudlow.GameEngineAdapter.Core;

namespace JohnLudlow.GameEngineAdapter.Headless;

/// <summary>
/// Records render commands for verification without GPU interaction.
/// </summary>
public sealed class HeadlessRenderProvider : IRenderProvider
{
    private readonly List<object> _recordedCommands = [];

    /// <summary>Gets the list of recorded render commands.</summary>
    public IReadOnlyList<object> RecordedCommands => _recordedCommands;

    /// <inheritdoc />
    public FrameScope BeginFrame(in CameraDescriptor camera) => new();

    /// <inheritdoc />
    public void SubmitSprite(in SpriteDrawDto dto) => _recordedCommands.Add(dto);

    /// <inheritdoc />
    public void SubmitText(in TextDrawDto dto) => _recordedCommands.Add(dto);

    /// <inheritdoc />
    public void SubmitMesh(in MeshDrawDto dto) => _recordedCommands.Add(dto);

    /// <inheritdoc />
    public void EndFrame() { }

    /// <inheritdoc />
    public void Present() { }

    /// <summary>Clears all recorded commands.</summary>
    public void Clear() => _recordedCommands.Clear();
}