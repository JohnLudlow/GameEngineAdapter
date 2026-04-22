namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessAudioPlayerTests
{
    [Fact]
    public void HeadlessAudioPlayer_RecordedCalls_InitiallyEmpty()
    {
        // Arrange / Act
        var player = new HeadlessAudioPlayer();

        // Assert
        Assert.Empty(player.RecordedCalls);
    }

    [Fact]
    public void HeadlessAudioPlayer_Play_RecordsCall()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.StartPlayback("music_main", loopPlayback: true);

        // Assert
        Assert.Single(player.RecordedCalls);
        var call = player.RecordedCalls[0];
        Assert.Equal(nameof(player.StartPlayback), call.Method);
        Assert.Equal("music_main", call.AudioAssetId);
        Assert.Equal(true, call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_Play_WithDefaultLoop_RecordsFalse()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.StartPlayback("sfx_jump");

        // Assert
        var call = player.RecordedCalls[0];
        Assert.Equal(false, call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_Stop_RecordsCall()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.StopPlayback("music_main");

        // Assert
        Assert.Single(player.RecordedCalls);
        var call = player.RecordedCalls[0];
        Assert.Equal(nameof(player.StopPlayback), call.Method);
        Assert.Equal("music_main", call.AudioAssetId);
        Assert.Null(call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_SetVolume_RecordsCall()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.SetVolume("music_main", 0.5f);

        // Assert
        Assert.Single(player.RecordedCalls);
        var call = player.RecordedCalls[0];
        Assert.Equal(nameof(player.SetVolume), call.Method);
        Assert.Equal("music_main", call.AudioAssetId);
        Assert.Equal(0.5f, call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_MultipleOperations_RecordsAllInOrder()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.StartPlayback("sfx_jump");
        player.SetVolume("sfx_jump", 0.8f);
        player.StopPlayback("sfx_jump");

        // Assert
        Assert.Equal(3, player.RecordedCalls.Count);
        Assert.Equal(nameof(player.StartPlayback), player.RecordedCalls[0].Method);
        Assert.Equal(nameof(player.SetVolume), player.RecordedCalls[1].Method);
        Assert.Equal(nameof(player.StopPlayback), player.RecordedCalls[2].Method);
    }

    [Fact]
    public void HeadlessAudioPlayer_Clear_RemovesAllCalls()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();
        player.StartPlayback("sfx_a");
        player.StopPlayback("sfx_b");

        // Act
        player.Clear();

        // Assert
        Assert.Empty(player.RecordedCalls);
    }
}
