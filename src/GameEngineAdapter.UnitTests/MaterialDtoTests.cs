namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class MaterialDtoTests
{
    [Fact]
    public void MaterialDto_Construction_StoresAllFields()
    {
        // Arrange
        var uniforms = new Dictionary<string, object> { ["color"] = (object)"red" };
        var textureSlots = new List<int> { 0, 1 };

        // Act
        var material = new MaterialDto(
            ShaderId: "shader_lit",
            Uniforms: uniforms,
            TextureSlots: textureSlots);

        // Assert
        Assert.Equal("shader_lit", material.ShaderId);
        Assert.Equal(uniforms, material.Uniforms);
        Assert.Equal(textureSlots, material.TextureSlots);
    }

    [Fact]
    public void MaterialDto_Uniforms_CanBeAccessedByKey()
    {
        // Arrange
        var uniforms = new Dictionary<string, object> { ["alpha"] = (object)0.5f };
        var material = new MaterialDto("s", uniforms, []);

        // Act / Assert
        Assert.Equal(0.5f, material.Uniforms["alpha"]);
    }

    [Fact]
    public void MaterialDto_TextureSlots_ContainsExpectedIndices()
    {
        // Arrange
        var slots = new List<int> { 2, 5 };
        var material = new MaterialDto("s", new Dictionary<string, object>(), slots);

        // Assert
        Assert.Equal(2, material.TextureSlots.Count);
        Assert.Equal(2, material.TextureSlots[0]);
        Assert.Equal(5, material.TextureSlots[1]);
    }

    [Fact]
    public void MaterialDto_EqualityByValue()
    {
        // Arrange
        var uniforms = new Dictionary<string, object>();
        var slots = new List<int>();
        var a = new MaterialDto("s", uniforms, slots);
        var b = new MaterialDto("s", uniforms, slots);

        // Assert
        Assert.Equal(a, b);
    }
}
