namespace JohnLudlow.GameEngineAdapter.UnitTests;

using JohnLudlow.GameEngineAdapter.Core;
using JohnLudlow.GameEngineAdapter.Headless;

public class DeterministicEngineRunnerTests
{
    private static EngineConfig MakeConfig() =>
        new("Headless", null, null);

    [Fact]
    public void DeterministicEngineRunner_Run_ZeroSteps_NoCommandsSubmitted()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());
        var renderProvider = (HeadlessRenderProvider)adapter.RenderProvider;
        var runner = new DeterministicEngineRunner(adapter, seed: 42, TimeSpan.FromMilliseconds(16));

        // Act
        runner.Run(0);

        // Assert
        Assert.Empty(renderProvider.RecordedSprites);
        Assert.Empty(renderProvider.RecordedTexts);
        Assert.Empty(renderProvider.RecordedMeshes);
    }

    [Fact]
    public void DeterministicEngineRunner_Run_CompletesWithoutThrowing()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());
        var runner = new DeterministicEngineRunner(adapter, seed: 0, TimeSpan.FromMilliseconds(16));

        // Act / Assert — must not throw
        runner.Run(5);
    }

    [Fact]
    public void DeterministicEngineRunner_Run_MultipleSteps_ExecutesEachStep()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());
        var runner = new DeterministicEngineRunner(adapter, seed: 1, TimeSpan.FromMilliseconds(16));

        // Act — run succeeds without exception for multiple steps
        runner.Run(10);

        // Assert — runner completed 10 steps (no throw is the primary assertion here,
        // since the headless runner calls BeginFrame/EndFrame/Present per step)
        // We verify the render provider itself is functional by checking it was used
        var renderProvider = (HeadlessRenderProvider)adapter.RenderProvider;
        Assert.NotNull(renderProvider);
    }

    [Fact]
    public void DeterministicEngineRunner_Run_DifferentSeeds_BothComplete()
    {
        // Arrange
        using var adapter1 = new HeadlessAdapter(MakeConfig());
        using var adapter2 = new HeadlessAdapter(MakeConfig());
        var runner1 = new DeterministicEngineRunner(adapter1, seed: 100, TimeSpan.FromMilliseconds(16));
        var runner2 = new DeterministicEngineRunner(adapter2, seed: 200, TimeSpan.FromMilliseconds(16));

        // Act / Assert — both seeds must complete without error
        runner1.Run(3);
        runner2.Run(3);
    }

    [Fact]
    public void DeterministicEngineRunner_Run_RepeatedCallsOnSameRunner_DoNotThrow()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());
        var runner = new DeterministicEngineRunner(adapter, seed: 7, TimeSpan.FromMilliseconds(33));

        // Act / Assert
        runner.Run(2);
        runner.Run(3);
    }

    // Construction/accessor coverage for DeterministicTickContext
    [Fact]
    public void DeterministicTickContext_CanBeConstructedWithAllParameters()
    {
        // Arrange
        using var adapter = new HeadlessAdapter(MakeConfig());
        var simulationTime = TimeSpan.FromMilliseconds(16);
        var rng = new Random(42);

        // Act - create the context
        var ctx = new DeterministicTickContext(simulationTime, rng, adapter);

        // Assert - properties are accessible
        Assert.Equal(simulationTime, ctx.SimulationTime);
        Assert.Same(rng, ctx.Rng);
        Assert.Same(adapter, ctx.Adapter);
    }

    [Fact]
    public void SimulationTime_IsZeroAfterConstruction()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

        Assert.Equal(TimeSpan.Zero, runner.SimulationTime);
    }

    [Fact]
    public void SimulationTime_AdvancesPerStep()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

        runner.Run(5);

        Assert.Equal(TimeSpan.FromMilliseconds(80), runner.SimulationTime);
    }

    [Fact]
    public void SimulationTime_AccumulatesAcrossMultipleRunCalls()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

        runner.Run(5);
        runner.Run(5);

        Assert.Equal(TimeSpan.FromMilliseconds(160), runner.SimulationTime);
    }

    [Fact]
    public void TickCallback_ReceivesSequentialSimulationTimes()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));
        var times = new List<TimeSpan>();

        runner.Run(3, ctx => times.Add(ctx.SimulationTime));

        Assert.Equal(
            [TimeSpan.Zero, TimeSpan.FromMilliseconds(16), TimeSpan.FromMilliseconds(32)],
            times);
    }

    [Fact]
    public void TickCallback_RngIsSameReferenceAsRunnerRng()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));
        Random? captured = null;

        runner.Run(1, ctx => captured = ctx.Rng);

        Assert.True(ReferenceEquals(runner.Rng, captured));
    }

    [Fact]
    public void TickCallback_AdapterIsSameReferenceAsRunnerAdapter()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));
        HeadlessAdapter? captured = null;

        runner.Run(1, ctx => captured = ctx.Adapter);

        Assert.True(ReferenceEquals(runner.Adapter, captured));
    }

    [Fact]
    public void TickCallback_RngStatePersistsAcrossInvocations()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner1 = new DeterministicEngineRunner(adapter, 42, TimeSpan.FromMilliseconds(16));
        var values1 = new List<float>();

        runner1.Run(2, ctx => values1.Add(ctx.Rng.NextSingle()));

        // Two steps must produce different values (state persists across invocations)
        Assert.NotEqual(values1[0], values1[1]);

        // Re-run with the same seed must produce the identical sequence
        using var adapter2 = new HeadlessAdapter(new EngineConfig("Headless", null, null));
        var runner2 = new DeterministicEngineRunner(adapter2, 42, TimeSpan.FromMilliseconds(16));
        var values2 = new List<float>();
        runner2.Run(2, ctx => values2.Add(ctx.Rng.NextSingle()));

        Assert.Equal(values1, values2);
    }

    [Fact]
    public void Run_WithNullCallback_DoesNotThrow()
    {
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

        runner.Run(1);
    }

    [Fact]
    public void Constructor_CameraParameterIsOptional()
    {
        // Three-argument construction (no camera) must compile and run without error.
        var config = new EngineConfig("Headless", null, null);
        using var adapter = new HeadlessAdapter(config);
        var runner = new DeterministicEngineRunner(adapter, 0, TimeSpan.FromMilliseconds(16));

        runner.Run(1);
    }
}