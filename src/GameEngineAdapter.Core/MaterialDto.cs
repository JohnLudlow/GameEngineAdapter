namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Data transfer object for material configuration.
/// </summary>
/// <param name="ShaderId">Identifier for the shader to use.</param>
/// <param name="Uniforms">Dictionary of shader uniform values.</param>
/// <param name="TextureSlots">Indices of texture slots to bind.</param>
public readonly record struct MaterialDto(
    string ShaderId,
    IReadOnlyDictionary<string, object> Uniforms,
    IReadOnlyList<int> TextureSlots);
