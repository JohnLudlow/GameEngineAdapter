namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Interface for audio playback (play, stop, volume control).
/// </summary>
public interface IAudioPlayer
{
    /// <summary>Plays the specified audio asset.</summary>
    void StartPlayback(string audioAssetId, bool loopPlayback = false);

    /// <summary>Stops playback of the specified audio asset.</summary>
    void StopPlayBack(string audioAssetId);

    /// <summary>Sets the volume for the specified audio asset (0.0–1.0).</summary>
    void SetVolume(string audioAssetId, float volume);
}
