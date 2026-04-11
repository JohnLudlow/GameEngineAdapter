namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Defines the type of camera projection.
/// </summary>
public enum ProjectionType { Orthographic, Perspective }

/// <summary>
/// Describes camera configuration for a render frame.
/// </summary>
/// <param name="Projection">The projection model to use.</param>
/// <param name="FieldOfViewDegrees">Field of view in degrees (for perspective projection).</param>
/// <param name="OrthographicSize">Size of the orthographic view (for orthographic projection).</param>
/// <param name="AspectRatio">Aspect ratio of the view.</param>
/// <param name="NearPlane">Distance to the near clipping plane.</param>
/// <param name="FarPlane">Distance to the far clipping plane.</param>
public readonly record struct CameraDescriptor(
    ProjectionType Projection,
    float FieldOfViewDegrees,
    float OrthographicSize,
    float AspectRatio,
    float NearPlane,
    float FarPlane);
