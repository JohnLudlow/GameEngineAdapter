namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;

public class MeshDrawDtoTests
{
    private static MaterialDto MakeMaterial() =>
        new("default", new Dictionary<string, object>(), []);

    private static TransformDto MakeTransform() =>
        new(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);

    [Fact]
    public void MeshDrawDto_Construction_StoresAllFields()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();

        // Act
        var dto = new MeshDrawDto(
            MeshId: "mesh_cube",
            Transform: transform,
            Material: material,
            Layer: 2);

        // Assert
        Assert.Equal("mesh_cube", dto.MeshId);
        Assert.Equal(transform, dto.Transform);
        Assert.Equal(material, dto.Material);
        Assert.Equal(2, dto.Layer);
    }

    [Fact]
    public void MeshDrawDto_EqualityByValue()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new MeshDrawDto("m", transform, material, 0);
        var b = new MeshDrawDto("m", transform, material, 0);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void MeshDrawDto_DifferentMeshId_NotEqual()
    {
        // Arrange
        var transform = MakeTransform();
        var material = MakeMaterial();
        var a = new MeshDrawDto("mesh_a", transform, material, 0);
        var b = new MeshDrawDto("mesh_b", transform, material, 0);

        // Assert
        Assert.NotEqual(a, b);
    }
}
