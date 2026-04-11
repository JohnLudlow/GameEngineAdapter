namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;

public class TransformDtoTests
{
    [Fact]
    public void TransformDto_Construction_StoresAllFields()
    {
        // Arrange / Act
        var transform = new TransformDto(
            X: 1f, Y: 2f, Z: 3f,
            RotationX: 10f, RotationY: 20f, RotationZ: 30f,
            ScaleX: 0.5f, ScaleY: 1.5f, ScaleZ: 2.0f);

        // Assert
        Assert.Equal(1f, transform.X);
        Assert.Equal(2f, transform.Y);
        Assert.Equal(3f, transform.Z);
        Assert.Equal(10f, transform.RotationX);
        Assert.Equal(20f, transform.RotationY);
        Assert.Equal(30f, transform.RotationZ);
        Assert.Equal(0.5f, transform.ScaleX);
        Assert.Equal(1.5f, transform.ScaleY);
        Assert.Equal(2.0f, transform.ScaleZ);
    }

    [Fact]
    public void TransformDto_DefaultConstruction_AllFieldsAreZero()
    {
        // Arrange / Act
        var transform = new TransformDto(
            X: 0f, Y: 0f, Z: 0f,
            RotationX: 0f, RotationY: 0f, RotationZ: 0f,
            ScaleX: 0f, ScaleY: 0f, ScaleZ: 0f);

        // Assert
        Assert.Equal(0f, transform.X);
        Assert.Equal(0f, transform.Y);
        Assert.Equal(0f, transform.Z);
    }

    [Fact]
    public void TransformDto_EqualityByValue()
    {
        // Arrange
        var a = new TransformDto(1f, 2f, 3f, 0f, 0f, 0f, 1f, 1f, 1f);
        var b = new TransformDto(1f, 2f, 3f, 0f, 0f, 0f, 1f, 1f, 1f);

        // Act / Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void TransformDto_InequalityWhenFieldsDiffer()
    {
        // Arrange
        var a = new TransformDto(1f, 2f, 3f, 0f, 0f, 0f, 1f, 1f, 1f);
        var b = new TransformDto(4f, 5f, 6f, 0f, 0f, 0f, 1f, 1f, 1f);

        // Act / Assert
        Assert.NotEqual(a, b);
    }
}
