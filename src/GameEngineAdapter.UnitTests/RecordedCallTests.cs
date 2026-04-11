namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Headless;

public class RecordedCallTests
{
    [Fact]
    public void RecordedCall_Construction_StoresProviderName()
    {
        // Arrange / Act
        var call = new RecordedCall("Render", "SubmitSprite", []);

        // Assert
        Assert.Equal("Render", call.ProviderName);
    }

    [Fact]
    public void RecordedCall_Construction_StoresMethodName()
    {
        // Arrange / Act
        var call = new RecordedCall("Input", "IsKeyDown", []);

        // Assert
        Assert.Equal("IsKeyDown", call.MethodName);
    }

    [Fact]
    public void RecordedCall_Construction_StoresArguments()
    {
        // Arrange
        var args = new List<object?> { "arg1", 42 };

        // Act
        var call = new RecordedCall("Adapter", "InitializeAsync", args);

        // Assert
        Assert.Equal(2, call.Arguments.Count);
        Assert.Equal("arg1", call.Arguments[0]);
        Assert.Equal(42, call.Arguments[1]);
    }

    [Fact]
    public void RecordedCall_Construction_EmptyArguments_IsAllowed()
    {
        // Arrange / Act
        var call = new RecordedCall("Adapter", "ShutdownAsync", []);

        // Assert
        Assert.Empty(call.Arguments);
    }

    [Fact]
    public void RecordedCall_EqualityByValue_SameFields_AreEqual()
    {
        // Arrange
        var args = new List<object?>();
        var a = new RecordedCall("Adapter", "ShutdownAsync", args);
        var b = new RecordedCall("Adapter", "ShutdownAsync", args);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void RecordedCall_DifferentMethodName_NotEqual()
    {
        // Arrange
        var a = new RecordedCall("Adapter", "InitializeAsync", []);
        var b = new RecordedCall("Adapter", "ShutdownAsync", []);

        // Assert
        Assert.NotEqual(a, b);
    }
}
