namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Provider for rendering operations and submitting draw commands.
/// </summary>
public interface IRenderProvider
{
  /// <summary>
  /// Starts a new render frame with the specified camera configuration.
  /// </summary>
  /// <param name="camera">Camera configuration for the frame.</param>
  /// <returns>A scope that finalizes the frame when disposed.</returns>
  FrameScope BeginFrame(in CameraDescriptor camera);

  /// <summary>Submits a sprite for rendering.</summary>
  void SubmitSprite(in SpriteDrawDto dto);

  /// <summary>Submits text for rendering.</summary>
  void SubmitText(in TextDrawDto dto);

  /// <summary>Submits a mesh for rendering.</summary>
  void SubmitMesh(in MeshDrawDto dto);

  /// <summary>Ends the current render frame.</summary>
  void EndFrame();

  /// <summary>Presents the rendered frame to the display.</summary>
  void Present();
}
