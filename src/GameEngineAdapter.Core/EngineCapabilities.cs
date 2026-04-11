namespace JohnLudlow.GameEngineAdapter.Core;

/// <summary>
/// Describes the capabilities and features supported by an engine adapter.
/// </summary>
/// <param name="Supports2D">Indicates if 2D rendering is supported.</param>
/// <param name="Supports3D">Indicates if 3D rendering is supported.</param>
/// <param name="SupportsShaders">Indicates if custom shaders are supported.</param>
/// <param name="SupportsAudio">Indicates if audio playback is supported.</param>
/// <param name="SupportsRichUI">Indicates if rich UI features are supported.</param>
/// <param name="ContractVersion">The semantic version of the adapter contract.</param>
/// <param name="SupportedTextureFormats">List of texture formats supported by the engine.</param>
/// <param name="MaxTextureSize">Maximum supported texture size in pixels.</param>
/// <param name="MaxAudioChannels">Maximum number of concurrent audio channels.</param>
public readonly record struct EngineCapabilities(
    bool Supports2D,
    bool Supports3D,
    bool SupportsShaders,
    bool SupportsAudio,
    bool SupportsRichUI,
    string ContractVersion,
    IReadOnlyList<string> SupportedTextureFormats,
    int MaxTextureSize,
    int MaxAudioChannels);

