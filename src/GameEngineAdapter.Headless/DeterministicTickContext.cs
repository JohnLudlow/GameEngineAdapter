namespace JohnLudlow.GameEngineAdapter.Headless;

/// <summary>
/// Per-step context provided to the tick callback in a deterministic simulation run.
/// </summary>
/// <param name="SimulationTime">
/// Simulated time at the start of this step.
/// </param>
/// <param name="Random">
/// Seeded random number generator for deterministic outputs. Shared with the runner; calls consume the sequence.
/// </param>
/// <param name="Adapter">
/// The headless adapter driving this simulation. Provides access to providers and asset/input/audio state.
/// </param>
public readonly record struct DeterministicTickContext(
    TimeSpan SimulationTime,
    Random Random,
    HeadlessAdapter Adapter);