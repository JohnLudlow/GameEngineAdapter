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
        player.Play("music_main", loop: true);

        // Assert
        Assert.Single(player.RecordedCalls);
        var call = player.RecordedCalls[0];
        Assert.Equal("Play", call.Method);
        Assert.Equal("music_main", call.AudioAssetId);
        Assert.Equal(true, call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_Play_WithDefaultLoop_RecordsFalse()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.Play("sfx_jump");

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
        player.Stop("music_main");

        // Assert
        Assert.Single(player.RecordedCalls);
        var call = player.RecordedCalls[0];
        Assert.Equal("Stop", call.Method);
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
        Assert.Equal("SetVolume", call.Method);
        Assert.Equal("music_main", call.AudioAssetId);
        Assert.Equal(0.5f, call.Arg);
    }

    [Fact]
    public void HeadlessAudioPlayer_MultipleOperations_RecordsAllInOrder()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();

        // Act
        player.Play("sfx_jump");
        player.SetVolume("sfx_jump", 0.8f);
        player.Stop("sfx_jump");

        // Assert
        Assert.Equal(3, player.RecordedCalls.Count);
        Assert.Equal("Play", player.RecordedCalls[0].Method);
        Assert.Equal("SetVolume", player.RecordedCalls[1].Method);
        Assert.Equal("Stop", player.RecordedCalls[2].Method);
    }

    [Fact]
    public void HeadlessAudioPlayer_Clear_RemovesAllCalls()
    {
        // Arrange
        var player = new HeadlessAudioPlayer();
        player.Play("sfx_a");
        player.Stop("sfx_b");

        // Act
        player.Clear();

        // Assert
        Assert.Empty(player.RecordedCalls);
    }
}
