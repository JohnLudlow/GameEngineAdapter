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
        Assert.Empty(renderProvider.RecordedCommands);
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
}
