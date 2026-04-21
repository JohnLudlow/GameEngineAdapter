namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Headless audio player that records all calls for verification.
/// All playback methods are no-ops; calls are recorded for test assertion.
/// </summary>
public sealed class HeadlessAudioPlayer : IAudioPlayer
{
    private readonly List<(string Method, string AudioAssetId, object? Arg)> _recordedCalls = [];

    /// <summary>Gets the list of recorded audio calls for test assertion.</summary>
    public IReadOnlyList<(string Method, string AudioAssetId, object? Arg)> RecordedCalls =>
        _recordedCalls;

    /// <inheritdoc />
    public void StartPlayback(string audioAssetId, bool loopPlayback = false) =>
        _recordedCalls.Add(("Play", audioAssetId, loopPlayback));

    /// <inheritdoc />
    public void StopPlayBack(string audioAssetId) =>
        _recordedCalls.Add(("Stop", audioAssetId, null));

    /// <inheritdoc />
    public void SetVolume(string audioAssetId, float volume) =>
        _recordedCalls.Add(("SetVolume", audioAssetId, volume));

    /// <summary>Clears all recorded calls.</summary>
    public void Clear() => _recordedCalls.Clear();
}
