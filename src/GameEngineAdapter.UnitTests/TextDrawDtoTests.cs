namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class TextDrawDtoTests
{
    private static MaterialDto MakeMaterial() =>
        new("default", new Dictionary<string, object>(), []);

    private static TransformDto MakeTransform() =>
        new(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);

    [Fact]
    public void TextDrawDto_Construction_StoresAllFields()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();

        // Act
        var dto = new TextDrawDto(
            Text: "Hello World",
            Transform: transform,
            FontId: "font_sans",
            FontSize: 14f,
            Material: material,
            Layer: 3);

        // Assert
        Assert.Equal("Hello World", dto.Text);
        Assert.Equal(transform, dto.Transform);
        Assert.Equal("font_sans", dto.FontId);
        Assert.Equal(14f, dto.FontSize);
        Assert.Equal(material, dto.Material);
        Assert.Equal(3, dto.Layer);
    }

    [Fact]
    public void TextDrawDto_EqualityByValue()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new TextDrawDto("Hi", transform, "f", 12f, material, 0);
        var b = new TextDrawDto("Hi", transform, "f", 12f, material, 0);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void TextDrawDto_DifferentText_NotEqual()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new TextDrawDto("Hello", transform, "f", 12f, material, 0);
        var b = new TextDrawDto("Goodbye", transform, "f", 12f, material, 0);

        // Assert
        Assert.NotEqual(a, b);
    }
}
