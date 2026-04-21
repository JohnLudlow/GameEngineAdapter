namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Headless;

public class HeadlessInputProviderTests
{
    [Fact]
    public void HeadlessInputProvider_ScriptKeyDown_KeyIsDown()
    {
        // Arrange
        var provider = new HeadlessInputProvider();

        // Act
        provider.ScriptKeyDown("Space");

        // Assert
        Assert.True(provider.IsKeyDown("Space"));
    }

    [Fact]
    public void HeadlessInputProvider_ScriptKeyUp_AfterDown_KeyIsUp()
    {
        // Arrange
        var provider = new HeadlessInputProvider();
        provider.ScriptKeyDown("Enter");

        // Act
        provider.ScriptKeyUp("Enter");

        // Assert
        Assert.False(provider.IsKeyDown("Enter"));
    }

    [Fact]
    public void HeadlessInputProvider_IsKeyDown_UnscriptedKey_ReturnsFalse()
    {
        // Arrange
        var provider = new HeadlessInputProvider();

        // Act / Assert
        Assert.False(provider.IsKeyDown("A"));
    }

    [Fact]
    public void HeadlessInputProvider_ScriptMouseButtonDown_ButtonIsDown()
    {
        // Arrange
        var provider = new HeadlessInputProvider();

        // Act
        provider.ScriptMouseButtonDown(0);

        // Assert
        Assert.True(provider.IsMouseButtonDown(0));
    }

    [Fact]
    public void HeadlessInputProvider_ScriptMouseButtonUp_AfterDown_ButtonIsUp()
    {
        // Arrange
        var provider = new HeadlessInputProvider();
        provider.ScriptMouseButtonDown(1);

        // Act
        provider.ScriptMouseButtonUp(1);

        // Assert
        Assert.False(provider.IsMouseButtonDown(1));
    }

    [Fact]
    public void HeadlessInputProvider_IsMouseButtonDown_UnscriptedButton_ReturnsFalse()
    {
        // Arrange
        var provider = new HeadlessInputProvider();

        // Act / Assert
        Assert.False(provider.IsMouseButtonDown(2));
    }

    [Fact]
    public void HeadlessInputProvider_ScriptMousePosition_SetsPosition()
    {
        // Arrange
        var provider = new HeadlessInputProvider();

        // Act
        provider.ScriptMousePosition(320f, 240f);

        // Assert
        var (x, y) = provider.GetMousePosition();
        Assert.Equal(320f, x);
        Assert.Equal(240f, y);
    }

    [Fact]
    public void HeadlessInputProvider_Reset_ClearsKeyState()
    {
        // Arrange
        var provider = new HeadlessInputProvider();
        provider.ScriptKeyDown("W");
        provider.ScriptKeyDown("A");

        // Act
        provider.Reset();

        // Assert
        Assert.False(provider.IsKeyDown("W"));
        Assert.False(provider.IsKeyDown("A"));
    }

    [Fact]
    public void HeadlessInputProvider_Reset_ClearsMouseButtonState()
    {
        // Arrange
        var provider = new HeadlessInputProvider();
        provider.ScriptMouseButtonDown(0);
        provider.ScriptMouseButtonDown(1);

        // Act
        provider.Reset();

        // Assert
        Assert.False(provider.IsMouseButtonDown(0));
        Assert.False(provider.IsMouseButtonDown(1));
    }

    [Fact]
    public void HeadlessInputProvider_Reset_ResetsMousePositionToOrigin()
    {
        // Arrange
        var provider = new HeadlessInputProvider();
        provider.ScriptMousePosition(100f, 200f);

        // Act
        provider.Reset();

        // Assert
        var (x, y) = provider.GetMousePosition();
        Assert.Equal(0f, x);
        Assert.Equal(0f, y);
    }
}
