namespace JohnLudlow.GameEngineAdapter;

/// <summary>
/// Disposable scope for a single render frame. Disposing finalizes the frame.
/// </summary>
public readonly record struct FrameScope : IDisposable
{
    /// <summary>Finalizes the current frame.</summary>
    public void Dispose() { /* adapter-specific frame end logic */ }
}
