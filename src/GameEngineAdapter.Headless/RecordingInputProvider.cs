namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Recording decorator for <see cref="IInputProvider"/> that logs all method calls.
/// </summary>
public sealed class RecordingInputProvider : IInputProvider
{
    private readonly IInputProvider _inner;
    private readonly IList<RecordedCall> _recordedCalls;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingInputProvider"/> class.
    /// </summary>
    /// <param name="inner">The underlying input provider to wrap.</param>
    /// <param name="recordedCalls">The list to record method calls into.</param>
    public RecordingInputProvider(IInputProvider inner, IList<RecordedCall> recordedCalls)
    {
        _inner = inner;
        _recordedCalls = recordedCalls;
    }

    /// <inheritdoc />
    public bool IsKeyDown(string key)
    {
        _recordedCalls.Add(new RecordedCall("Input", "IsKeyDown", [key]));
        return _inner.IsKeyDown(key);
    }

    /// <inheritdoc />
    public bool IsMouseButtonDown(int button)
    {
        _recordedCalls.Add(new RecordedCall("Input", "IsMouseButtonDown", [button]));
        return _inner.IsMouseButtonDown(button);
    }

    /// <inheritdoc />
    public (float X, float Y) GetMousePosition()
    {
        _recordedCalls.Add(new RecordedCall("Input", "GetMousePosition", []));
        return _inner.GetMousePosition();
    }
}