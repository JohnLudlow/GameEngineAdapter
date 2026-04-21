namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;

public class CameraDescriptorTests
{
    [Fact]
    public void CameraDescriptor_Construction_WithOrthographic_StoresFields()
    {
        // Arrange / Act
        var cam = new CameraDescriptor(
            Projection: ProjectionType.Orthographic,
            FieldOfViewDegrees: 0f,
            OrthographicSize: 10f,
            AspectRatio: 16f / 9f,
            NearPlane: 0.1f,
            FarPlane: 100f);

        // Assert
        Assert.Equal(ProjectionType.Orthographic, cam.Projection);
        Assert.Equal(0f, cam.FieldOfViewDegrees);
        Assert.Equal(10f, cam.OrthographicSize);
        Assert.Equal(16f / 9f, cam.AspectRatio);
        Assert.Equal(0.1f, cam.NearPlane);
        Assert.Equal(100f, cam.FarPlane);
    }

    [Fact]
    public void CameraDescriptor_Construction_WithPerspective_StoresFields()
    {
        // Arrange / Act
        var cam = new CameraDescriptor(
            Projection: ProjectionType.Perspective,
            FieldOfViewDegrees: 60f,
            OrthographicSize: 0f,
            AspectRatio: 4f / 3f,
            NearPlane: 0.01f,
            FarPlane: 500f);

        // Assert
        Assert.Equal(ProjectionType.Perspective, cam.Projection);
        Assert.Equal(60f, cam.FieldOfViewDegrees);
        Assert.Equal(4f / 3f, cam.AspectRatio);
    }

    [Fact]
    public void CameraDescriptor_EqualityByValue()
    {
        // Arrange
        var a = new CameraDescriptor(ProjectionType.Orthographic, 0f, 10f, 1.778f, 0.1f, 100f);
        var b = new CameraDescriptor(ProjectionType.Orthographic, 0f, 10f, 1.778f, 0.1f, 100f);

        // Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void CameraDescriptor_DifferentProjection_NotEqual()
    {
        // Arrange
        var a = new CameraDescriptor(ProjectionType.Orthographic, 0f, 10f, 1f, 0.1f, 100f);
        var b = new CameraDescriptor(ProjectionType.Perspective, 60f, 0f, 1f, 0.1f, 100f);

        // Assert
        Assert.NotEqual(a, b);
    }
}
