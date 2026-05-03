namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class MapGenerationIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<MeshDrawDto> RunMapScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, seed, TimeSpan.FromMilliseconds(16));
        var render = (HeadlessRenderProvider)adapter.RenderProvider;

        runner.Run(100, ctx =>
        {
            for (var i = 0; i < 10; i++)
            {
                var x = ctx.Random.NextSingle() * 512f;
                var y = ctx.Random.NextSingle() * 512f;
                var z = ctx.Random.NextSingle() * 10f;
                var scaleX = ctx.Random.NextSingle() * 4f + 1f;
                var scaleY = ctx.Random.NextSingle() * 4f + 1f;
                var scaleZ = ctx.Random.NextSingle() * 4f + 1f;
                var transform = new TransformDto(x, y, z, 0f, 0f, 0f, scaleX, scaleY, scaleZ);
                adapter.RenderProvider.SubmitMesh(
                    new MeshDrawDto($"tile_{i}", transform, DefaultMaterial, 0));
            }
        });

        return render.RecordedMeshes;
    }

    [Fact]
    public void MapScenario_Produces1000Meshes()
    {
        var meshes = RunMapScenario(0);
        Assert.Equal(1000, meshes.Count);
    }

    [Fact]
    public void MapScenario_IsDeterministic()
    {
        var run1 = RunMapScenario(0);
        var run2 = RunMapScenario(0);
        Assert.Equal(run1, run2);
    }
}
