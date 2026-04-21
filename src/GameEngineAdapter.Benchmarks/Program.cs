namespace JohnLudlow.GameEngineAdapter.Benchmarks;

using BenchmarkDotNet.Running;

internal static class Program
{
    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
