namespace JohnLudlow.GameEngineAdapter.UnitTests;

using System.Collections.Generic;
using System.Linq;
using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public sealed class CombatIntegrationTests
{
    private static readonly MaterialDto DefaultMaterial =
        new("default", new Dictionary<string, object>(), []);

    private static IReadOnlyList<RecordedCall> RunCombatScenario(int seed)
    {
        var config = new EngineConfig("Headless", null, null);
        using var testAdapter = new TestAdapter(config);
        var runner = new DeterministicEngineRunner(testAdapter.Inner, seed, TimeSpan.FromMilliseconds(16));

        var step = 0;
        runner.Run(10, ctx =>
        {
            var transform = new TransformDto(0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f);
            testAdapter.RenderProvider.SubmitSprite(
                new SpriteDrawDto("character", transform, DefaultMaterial, 0));

            if (step % 2 == 0)
                testAdapter.AudioPlayer.StartPlayback("weapon_fire");

            step++;
        });

        return testAdapter.RecordedCalls;
    }

    [Fact]
    public void CombatScenario_IsDeterministic()
    {
        var run1 = RunCombatScenario(1337);
        var run2 = RunCombatScenario(1337);
        // RecordedCall.Arguments uses reference equality on its IReadOnlyList<object?> field.
        // Compare the call sequence by ProviderName and MethodName to assert ordering stability.
        Assert.Equal(
            run1.Select(c => (c.ProviderName, c.MethodName)),
            run2.Select(c => (c.ProviderName, c.MethodName)));
    }

    [Fact]
    public void CombatScenario_AudioOnEvenStepsOnly()
    {
        var calls = RunCombatScenario(1337);
        var audioCalls = calls.Where(c => c.ProviderName == "Audio").ToList();
        Assert.Equal(5, audioCalls.Count);
        Assert.All(audioCalls, c => Assert.Equal("weapon_fire", (string?)c.Arguments[0]));
    }
}
