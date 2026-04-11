namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class EngineConfigTests
{
    [Fact]
    public void EngineConfig_Construction_StoresAdapterName()
    {
        // Arrange / Act
        var config = new EngineConfig("MonoGame", null, null);

        // Assert
        Assert.Equal("MonoGame", config.AdapterName);
    }

    [Fact]
    public void EngineConfig_Construction_StoresResourcePath()
    {
        // Arrange / Act
        var config = new EngineConfig("Headless", "/assets", null);

        // Assert
        Assert.Equal("/assets", config.ResourcePath);
    }

    [Fact]
    public void EngineConfig_Construction_NullResourcePathIsAllowed()
    {
        // Arrange / Act
        var config = new EngineConfig("Headless", null, null);

        // Assert
        Assert.Null(config.ResourcePath);
    }

    [Fact]
    public void EngineConfig_Construction_StoresOptions()
    {
        // Arrange
        var options = new Dictionary<string, object> { ["vsync"] = (object)true };

        // Act
        var config = new EngineConfig("Headless", null, options);

        // Assert
        Assert.NotNull(config.Options);
        Assert.Equal(true, config.Options["vsync"]);
    }

    [Fact]
    public void EngineConfig_NullOptions_IsAllowed()
    {
        // Arrange / Act
        var config = new EngineConfig("Headless", null, null);

        // Assert
        Assert.Null(config.Options);
    }

    [Fact]
    public void EngineConfig_EqualityByValue()
    {
        // Arrange
        var a = new EngineConfig("Headless", "/res", null);
        var b = new EngineConfig("Headless", "/res", null);

        // Assert
        Assert.Equal(a, b);
    }
}
