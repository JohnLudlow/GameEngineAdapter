namespace JohnLudlow.GameEngineAdapter.Headless;

/// <summary>
/// DTO recording a single adapter call for test assertion.
/// </summary>
/// <param name="ProviderName">Name of the provider (e.g. "Render", "Input").</param>
/// <param name="MethodName">Name of the method called.</param>
/// <param name="Arguments">Arguments passed to the method.</param>
public readonly record struct RecordedCall(
    string ProviderName,
    string MethodName,
    IReadOnlyList<object?> Arguments);
