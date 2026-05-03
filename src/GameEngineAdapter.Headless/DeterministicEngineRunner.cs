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
    private readonly CameraDescriptor _camera;

    /// <summary>
    /// Gets the underlying headless adapter driven by this runner.
    /// </summary>
    public HeadlessAdapter Adapter => _adapter;

    /// <summary>
    /// Gets the seeded random number generator used by this runner.
    /// </summary>
    public Random Rng => _rng;

    /// <summary>
    /// Gets the current simulated time. Starts at <see cref="TimeSpan.Zero"/> and advances by
    /// one <c>fixedTimestep</c> after each step's <c>Present()</c> call.
    /// </summary>
    public TimeSpan SimulationTime { get; private set; } = TimeSpan.Zero;

    /// <summary>
    /// Initializes a new deterministic engine runner.
    /// </summary>
    /// <param name="adapter">The headless adapter to drive.</param>
    /// <param name="seed">RNG seed for reproducibility.</param>
    /// <param name="fixedTimestep">Time interval per simulation step.</param>
    /// <param name="camera">
    /// Camera descriptor passed to <c>BeginFrame</c> each step. Defaults to
    /// <c>default(CameraDescriptor)</c>; the headless render provider ignores camera
    /// configuration, so the default value is safe for headless deterministic runs.
    /// </param>
    public DeterministicEngineRunner(
        HeadlessAdapter adapter,
        int seed,
        TimeSpan fixedTimestep,
        CameraDescriptor camera = default)
    {
        _adapter = adapter;
        _rng = new Random(seed);
        _fixedTimestep = fixedTimestep;
        _camera = camera;
        SimulationTime = TimeSpan.Zero;
    }

    /// <summary>
    /// Runs the simulation for the specified number of steps.
    /// </summary>
    /// <param name="steps">Number of simulation steps to execute.</param>
    /// <param name="onTick">
    /// Optional per-step callback invoked between <c>BeginFrame</c> and <c>EndFrame</c>,
    /// receiving the current <see cref="SimulationTime"/>, the runner's seeded <see cref="Rng"/>,
    /// and the underlying <see cref="Adapter"/>. May be <see langword="null"/>.
    /// </param>
    public void Run(int steps, Action<DeterministicTickContext>? onTick = null)
    {
        var renderProvider = _adapter.RenderProvider;

        for (var i = 0; i < steps; i++)
        {
            using var scope = renderProvider.BeginFrame(in _camera);
            onTick?.Invoke(new DeterministicTickContext(SimulationTime, _rng, _adapter));
            renderProvider.EndFrame();
            renderProvider.Present();
            SimulationTime += _fixedTimestep;
        }
    }
}
