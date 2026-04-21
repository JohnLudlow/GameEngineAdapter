namespace JohnLudlow.GameEngineAdapter.Headless;

using JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Runs a headless simulation with deterministic fixed timestep and seeded RNG.
/// </summary>
public sealed class DeterministicEngineRunner
{
    private readonly HeadlessAdapter _adapter;
    private readonly Random _rng;
    private readonly TimeSpan _fixedTimestep;

    /// <summary>
    /// Initializes a new deterministic engine runner.
    /// </summary>
    /// <param name="adapter">The headless adapter to drive.</param>
    /// <param name="seed">RNG seed for reproducibility.</param>
    /// <param name="fixedTimestep">Time interval per simulation step.</param>
    public DeterministicEngineRunner(HeadlessAdapter adapter, int seed, TimeSpan fixedTimestep)
    {
        _adapter = adapter;
        _rng = new Random(seed);
        _fixedTimestep = fixedTimestep;
    }

    /// <summary>
    /// Runs the simulation for the specified number of steps.
    /// </summary>
    /// <param name="steps">Number of simulation steps to execute.</param>
    public void Run(int steps)
    {
        var renderProvider = _adapter.RenderProvider;
        var camera = new CameraDescriptor(
            ProjectionType.Orthographic, 0f, 10f, 16f / 9f, 0.1f, 100f);

        for (var i = 0; i < steps; i++)
        {
            using var scope = renderProvider.BeginFrame(in camera);
            // Simulation logic using _rng for determinism
            renderProvider.EndFrame();
            renderProvider.Present();
        }
    }
}
