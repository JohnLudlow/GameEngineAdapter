namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class AiIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<SpriteDrawDto> RunNpcScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, seed, TimeSpan.FromMilliseconds(16));
        var render = (HeadlessRenderProvider)adapter.RenderProvider;

        runner.Run(60, ctx =>
        {
            var x = ctx.Rng.NextSingle() * 100f;
            var y = ctx.Rng.NextSingle() * 100f;
            var transform = new TransformDto(x, y, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
            adapter.RenderProvider.SubmitSprite(new SpriteDrawDto("npc", transform, DefaultMaterial, 0));
        });

        return render.RecordedSprites;
    }

    [Fact]
    public void NpcScenario_IsDeterministic()
    {
        var run1 = RunNpcScenario(42);
        var run2 = RunNpcScenario(42);
        Assert.Equal(run1, run2);
    }
}
