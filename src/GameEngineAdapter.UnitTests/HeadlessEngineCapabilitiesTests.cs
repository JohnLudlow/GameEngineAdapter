namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;

public class HeadlessEngineCapabilitiesTests
{
    private static EngineCapabilities MakeBase() =>
        new(true, false, false, false, false, "1.0.0", [], 0, 0);

    [Fact]
    public void HeadlessEngineCapabilities_Construction_WrapsBase()
    {
        // Arrange
        var baseCaps = MakeBase();

        // Act
        var caps = new HeadlessEngineCapabilities(
            Base: baseCaps,
            SupportsOffscreenRendering: true,
            DeterministicTick: true);

        // Assert
        Assert.Equal(baseCaps, caps.Base);
    }

    [Fact]
    public void HeadlessEngineCapabilities_SupportsOffscreenRendering_IsStored()
    {
        // Arrange / Act
        var caps = new HeadlessEngineCapabilities(MakeBase(), SupportsOffscreenRendering: true, DeterministicTick: false);

        // Assert
        Assert.True(caps.SupportsOffscreenRendering);
    }

    [Fact]
    public void HeadlessEngineCapabilities_DeterministicTick_IsStored()
    {
        // Arrange / Act
        var caps = new HeadlessEngineCapabilities(MakeBase(), SupportsOffscreenRendering: false, DeterministicTick: true);

        // Assert
        Assert.True(caps.DeterministicTick);
    }

    [Fact]
    public void HeadlessEngineCapabilities_EqualityByValue()
    {
        // Arrange
        var baseCaps = MakeBase();
        var a = new HeadlessEngineCapabilities(baseCaps, true, true);
        var b = new HeadlessEngineCapabilities(baseCaps, true, true);

        // Assert
        Assert.Equal(a, b);
    }
}
