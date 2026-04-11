namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;

public class EngineCapabilitiesTests
{
    [Fact]
    public void EngineCapabilities_Construction_StoresAllFields()
    {
        // Arrange
        var formats = new List<string> { "png", "jpg" };

        // Act
        var caps = new EngineCapabilities(
            Supports2D: true,
            Supports3D: false,
            SupportsShaders: true,
            SupportsAudio: false,
            SupportsRichUI: true,
            ContractVersion: "1.0.0",
            SupportedTextureFormats: formats,
            MaxTextureSize: 4096,
            MaxAudioChannels: 0);

        // Assert
        Assert.True(caps.Supports2D);
        Assert.False(caps.Supports3D);
        Assert.True(caps.SupportsShaders);
        Assert.False(caps.SupportsAudio);
        Assert.True(caps.SupportsRichUI);
        Assert.Equal("1.0.0", caps.ContractVersion);
        Assert.Equal(formats, caps.SupportedTextureFormats);
        Assert.Equal(4096, caps.MaxTextureSize);
        Assert.Equal(0, caps.MaxAudioChannels);
    }

    [Fact]
    public void EngineCapabilities_EqualityByValue()
    {
        // Arrange
        var formats = new List<string>();
        var a = new EngineCapabilities(true, false, false, false, false, "1.0.0", formats, 1024, 8);
        var b = new EngineCapabilities(true, false, false, false, false, "1.0.0", formats, 1024, 8);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void EngineCapabilities_DifferentContractVersion_NotEqual()
    {
        // Arrange
        var formats = new List<string>();
        var a = new EngineCapabilities(true, false, false, false, false, "1.0.0", formats, 0, 0);
        var b = new EngineCapabilities(true, false, false, false, false, "2.0.0", formats, 0, 0);

        // Assert
        Assert.NotEqual(a, b);
    }
}
