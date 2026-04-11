namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// DTO for submitting a text draw command.
/// </summary>
/// <param name="Text">Text content to render.</param>
/// <param name="Transform">World-space transform for the text.</param>
/// <param name="FontId">Asset identifier for the font.</param>
/// <param name="FontSize">Font size in points.</param>
/// <param name="Material">Material to apply when rendering.</param>
/// <param name="Layer">Sort layer for deterministic draw ordering.</param>
public readonly record struct TextDrawDto(
    string Text,
    TransformDto Transform,
    string FontId,
    float FontSize,
    MaterialDto Material,
    int Layer);
