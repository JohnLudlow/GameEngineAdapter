namespace JohnLudlow.GameEngineAdapter;

/// <summary>
/// World-space transform for an object.
/// </summary>
/// <param name="X">X coordinate of the position.</param>
/// <param name="Y">Y coordinate of the position.</param>
/// <param name="Z">Z coordinate of the position.</param>
/// <param name="RotationX">Rotation around the X axis.</param>
/// <param name="RotationY">Rotation around the Y axis.</param>
/// <param name="RotationZ">Rotation around the Z axis.</param>
/// <param name="ScaleX">Scale along the X axis.</param>
/// <param name="ScaleY">Scale along the Y axis.</param>
/// <param name="ScaleZ">Scale along the Z axis.</param>
public readonly record struct TransformDto(
    float X, float Y, float Z,
    float RotationX, float RotationY, float RotationZ,
    float ScaleX, float ScaleY, float ScaleZ);
