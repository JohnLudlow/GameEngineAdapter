namespace JohnLudlow.GameEngineAdapter.Benchmarks;

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JohnLudlow.GameEngineAdapter.Core;

[MemoryDiagnoser]
public class RenderTranslationBenchmarks
{
    private const int Iterations = 1000;

    private TranslatingRenderProvider _provider = null!;
    private SpriteDrawDto _sprite;

    [GlobalSetup]
    public void Setup()
    {
        var backend = new NoOpFakeRenderBackend();
        _provider = new TranslatingRenderProvider(backend);

        var transform = new TransformDto(
            X: 1f, Y: 2f, Z: 3f,
            RotationX: 10f, RotationY: 20f, RotationZ: 30f,
            ScaleX: 1f, ScaleY: 2f, ScaleZ: 3f);

        var uniforms = new Dictionary<string, object>
        {
            ["u_color"] = "red",
            ["u_alpha"] = 0.75f,
        };

        var material = new MaterialDto("shader_sprite", uniforms, [0, 2]);
        _sprite = new SpriteDrawDto("sprite_1", transform, material, Layer: 0);
    }

    [Benchmark]
    public void SubmitSprite_Single() =>
        _provider.SubmitSprite(in _sprite);

    [Benchmark]
    public void SubmitSprite_Loop1000()
    {
        for (var i = 0; i < Iterations; i++)
        {
            _provider.SubmitSprite(in _sprite);
        }
    }

    private interface IFakeRenderBackend
    {
        void DrawSprite(string spriteId, in TransformDto transform, in MaterialDto material, int layer);
    }

    private sealed class NoOpFakeRenderBackend : IFakeRenderBackend
    {
        public void DrawSprite(string spriteId, in TransformDto transform, in MaterialDto material, int layer)
        {
        }
    }

    private sealed class TranslatingRenderProvider(IFakeRenderBackend backend)
    {
        public void SubmitSprite(in SpriteDrawDto dto)
        {
            var transform = dto.Transform;
            var material = dto.Material;
            backend.DrawSprite(dto.SpriteId, in transform, in material, dto.Layer);
        }
    }
}
