using JohnLudlow.GameEngineAdapter.Core;

namespace JohnLudlow.GameEngineAdapter.Headless;

/// <summary>
/// Records render commands for verification without GPU interaction.
/// </summary>
public sealed class HeadlessRenderProvider : IRenderProvider
{
    private readonly List<SpriteDrawDto> _recordedSprites = [];
    private readonly List<TextDrawDto> _recordedTexts = [];
    private readonly List<MeshDrawDto> _recordedMeshes = [];

    /// <summary>Gets the list of recorded sprite draw commands.</summary>
    public IReadOnlyList<SpriteDrawDto> RecordedSprites => _recordedSprites;

    /// <summary>Gets the list of recorded text draw commands.</summary>
    public IReadOnlyList<TextDrawDto> RecordedTexts => _recordedTexts;

    /// <summary>Gets the list of recorded mesh draw commands.</summary>
    public IReadOnlyList<MeshDrawDto> RecordedMeshes => _recordedMeshes;

    /// <inheritdoc />
    public FrameScope BeginFrame(in CameraDescriptor camera) => new();

    /// <inheritdoc />
    public void SubmitSprite(in SpriteDrawDto dto) => _recordedSprites.Add(dto);

    /// <inheritdoc />
    public void SubmitText(in TextDrawDto dto) => _recordedTexts.Add(dto);

    /// <inheritdoc />
    public void SubmitMesh(in MeshDrawDto dto) => _recordedMeshes.Add(dto);

    /// <inheritdoc />
    public void EndFrame() { }

    /// <inheritdoc />
    public void Present() { }

    /// <summary>Clears all recorded commands.</summary>
    public void Clear()
    {
        _recordedSprites.Clear();
        _recordedTexts.Clear();
        _recordedMeshes.Clear();
    }
}