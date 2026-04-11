namespace JohnLudlow.GameEngineAdapter;

/// <summary>
/// DTO for submitting a mesh draw command.
/// </summary>
/// <param name="MeshId">Asset identifier for the mesh.</param>
/// <param name="Transform">World-space transform for the mesh.</param>
/// <param name="Material">Material to apply when rendering.</param>
/// <param name="Layer">Sort layer for deterministic draw ordering.</param>
public readonly record struct MeshDrawDto(
    string MeshId,
    TransformDto Transform,
    MaterialDto Material,
    int Layer);