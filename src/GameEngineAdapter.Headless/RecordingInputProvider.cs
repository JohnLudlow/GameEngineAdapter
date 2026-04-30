namespace JohnLudlow.GameEngineAdapter.Headless;

using System.Collections.ObjectModel;
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
        var result = _inner.IsKeyDown(key);
        _recordedCalls.Add(new RecordedCall("Input", "IsKeyDown", [key, result]));
        return result;
    }

    /// <inheritdoc />
    public bool IsMouseButtonDown(int button)
    {
        var result = _inner.IsMouseButtonDown(button);
        _recordedCalls.Add(new RecordedCall("Input", "IsMouseButtonDown", [button, result]));
        return result;
    }

    /// <inheritdoc />
    public (float X, float Y) GetMousePosition()
    {
        var result = _inner.GetMousePosition();
        _recordedCalls.Add(new RecordedCall("Input", "GetMousePosition", [result.X, result.Y]));
        return result;
    }
}