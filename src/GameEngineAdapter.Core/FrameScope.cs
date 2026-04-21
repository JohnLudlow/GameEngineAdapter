namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Disposable marker scope for a single render frame.
/// Disposing this scope currently performs no action.
/// </summary>
public readonly record struct FrameScope : IDisposable
{
    /// <summary>No-op marker disposal for the current frame scope.</summary>
    public void Dispose() { /* intentionally no-op marker scope */ }
}
