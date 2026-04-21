namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Simulates input by maintaining scripted state (keys held, buttons held, mouse position)
/// for deterministic testing.
/// </summary>
public sealed class HeadlessInputProvider : IInputProvider
{
    private readonly HashSet<string> _keysDown = [];
    private readonly HashSet<int> _buttonsDown = [];
    private (float X, float Y) _mousePosition;

    /// <summary>
    /// Sets the specified key as held down.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    public void ScriptKeyDown(string key) => _keysDown.Add(key);

    /// <summary>
    /// Releases the specified key.
    /// </summary>
    /// <param name="key">The key identifier.</param>
    public void ScriptKeyUp(string key) => _keysDown.Remove(key);

    /// <summary>
    /// Sets the specified mouse button as held down.
    /// </summary>
    /// <param name="button">The mouse button index.</param>
    public void ScriptMouseButtonDown(int button) => _buttonsDown.Add(button);

    /// <summary>
    /// Releases the specified mouse button.
    /// </summary>
    /// <param name="button">The mouse button index.</param>
    public void ScriptMouseButtonUp(int button) => _buttonsDown.Remove(button);

    /// <summary>
    /// Sets the scripted mouse position.
    /// </summary>
    /// <param name="x">X coordinate in screen space.</param>
    /// <param name="y">Y coordinate in screen space.</param>
    public void ScriptMousePosition(float x, float y) => _mousePosition = (x, y);

    /// <inheritdoc />
    public bool IsKeyDown(string key) => _keysDown.Contains(key);

    /// <inheritdoc />
    public bool IsMouseButtonDown(int button) => _buttonsDown.Contains(button);

    /// <inheritdoc />
    public (float X, float Y) GetMousePosition() => _mousePosition;

    /// <summary>Resets all scripted input state.</summary>
    public void Reset()
    {
        _keysDown.Clear();
        _buttonsDown.Clear();
        _mousePosition = (0f, 0f);
    }
}
