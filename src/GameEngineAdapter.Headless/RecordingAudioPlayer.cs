namespace JohnLudlow.GameEngineAdapter.Headless;

using System.Collections.ObjectModel;
using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Recording decorator for <see cref="IAudioPlayer"/> that logs all method calls.
/// </summary>
public sealed class RecordingAudioPlayer : IAudioPlayer
{
    private readonly IAudioPlayer _inner;
    private readonly IList<RecordedCall> _recordedCalls;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordingAudioPlayer"/> class.
    /// </summary>
    /// <param name="inner">The underlying audio player to wrap.</param>
    /// <param name="recordedCalls">The list to record method calls into.</param>
    public RecordingAudioPlayer(IAudioPlayer inner, IReadOnlyList<RecordedCall> recordedCalls)
    {
        _inner = inner;
        _recordedCalls = (IList<RecordedCall>)recordedCalls;
    }

    /// <inheritdoc />
    public void StartPlayback(string audioAssetId, bool loopPlayback = false)
    {
        _recordedCalls.Add(new RecordedCall("Audio", "StartPlayback", [audioAssetId, loopPlayback]));
        _inner.StartPlayback(audioAssetId, loopPlayback);
    }

    /// <inheritdoc />
    public void StopPlayback(string audioAssetId)
    {
        _recordedCalls.Add(new RecordedCall("Audio", "StopPlayback", [audioAssetId]));
        _inner.StopPlayback(audioAssetId);
    }

    /// <inheritdoc />
    public void SetVolume(string audioAssetId, float volume)
    {
        _recordedCalls.Add(new RecordedCall("Audio", "SetVolume", [audioAssetId, volume]));
        _inner.SetVolume(audioAssetId, volume);
    }
}