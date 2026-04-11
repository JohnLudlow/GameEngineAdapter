namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// DTO for submitting a sprite draw command.
/// </summary>
/// <param name="SpriteId">Asset identifier for the sprite texture.</param>
/// <param name="Transform">World-space transform for the sprite.</param>
/// <param name="Material">Material to apply when rendering.</param>
/// <param name="Layer">Sort layer for deterministic draw ordering.</param>
public readonly record struct SpriteDrawDto(
    string SpriteId,
    TransformDto Transform,
    MaterialDto Material,
    int Layer);
