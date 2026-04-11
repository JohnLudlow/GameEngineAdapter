namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Adapter-owned provider for polling input state (keyboard, mouse, gamepad).
/// </summary>
public interface IInputProvider
{
    /// <summary>Returns true if the specified key is currently pressed.</summary>
    bool IsKeyDown(string key);

    /// <summary>Returns true if the specified mouse button is currently pressed.</summary>
    bool IsMouseButtonDown(int button);

    /// <summary>Returns the current mouse position in screen coordinates.</summary>
    (float X, float Y) GetMousePosition();
}
