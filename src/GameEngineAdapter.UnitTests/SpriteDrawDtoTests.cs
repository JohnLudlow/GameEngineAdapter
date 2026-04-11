namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class SpriteDrawDtoTests
{
    private static MaterialDto MakeMaterial() =>
        new("default", new Dictionary<string, object>(), []);

    private static TransformDto MakeTransform() =>
        new(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);

    [Fact]
    public void SpriteDrawDto_Construction_StoresAllFields()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();

        // Act
        var dto = new SpriteDrawDto(
            SpriteId: "sprite_hero",
            Transform: transform,
            Material: material,
            Layer: 5);

        // Assert
        Assert.Equal("sprite_hero", dto.SpriteId);
        Assert.Equal(transform, dto.Transform);
        Assert.Equal(material, dto.Material);
        Assert.Equal(5, dto.Layer);
    }

    [Fact]
    public void SpriteDrawDto_EqualityByValue()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new SpriteDrawDto("s", transform, material, 1);
        var b = new SpriteDrawDto("s", transform, material, 1);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void SpriteDrawDto_DifferentLayer_NotEqual()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new SpriteDrawDto("s", transform, material, 1);
        var b = new SpriteDrawDto("s", transform, material, 2);

        // Assert
        Assert.NotEqual(a, b);
    }
}
