# Issue #1 follow-up — Translator tests and benchmarks

## Overview

Issue #1 is functionally complete for contract shape (interfaces + DTOs), but two acceptance criteria remain:

1. Translator-style unit tests that verify DTOs are correctly mapped to an engine-facing call surface using fakes.
2. A benchmark suite that measures DTO translation performance (target: single-digit microseconds on hot paths).

This plan describes how to add those two items without changing the public contract surface in `GameEngineAdapter.Core`.

## Table of contents

- [Issue #1 follow-up — Translator tests and benchmarks](#issue-1-follow-up--translator-tests-and-benchmarks)
  - [Overview](#overview)
  - [Table of contents](#table-of-contents)
  - [Plan issue](#plan-issue)
  - [Plan status](#plan-status)
  - [Definition of terms](#definition-of-terms)
  - [Architectural considerations and constraints](#architectural-considerations-and-constraints)
  - [Implementation guide](#implementation-guide)
    - [Plan requirements](#plan-requirements)
    - [Phase 1 — Translator tests](#phase-1--translator-tests)
    - [Phase 2 — Benchmarks](#phase-2--benchmarks)
  - [See also](#see-also)
  - [References](#references)

## Plan issue

- [#1](https://github.com/JohnLudlow/GameEngineAdapter/issues/1)

## Plan status

Completed

## Definition of terms

| Term | Meaning | Reference |
| ---- | ------- | --------- |
| BenchmarkDotNet | .NET microbenchmark framework that runs code repeatedly under controlled conditions to measure throughput/latency. | <https://benchmarkdotnet.org/> |
| Translator test | Unit test that verifies a DTO is translated/mapped into a lower-level call surface correctly (typically using fakes/spies). | |

## Architectural considerations and constraints

- **Do not change contracts**: `JohnLudlow.GameEngineAdapter.Core` is the public contract assembly. Translator tests and benchmarks should not require changes to public interfaces unless strictly necessary.
- **Keep engine-agnostic**: Translator tests should validate mapping logic without taking a dependency on a real engine SDK.
- **Avoid benchmark noise**: Benchmarks should run in Release and avoid allocations or I/O unrelated to the measured translation.
- **CI integration already scaffolded**: There is an existing composite action at `actions/steps/benchmark/action.yml` with TODO placeholders for the benchmark project path, and the workflow job is currently commented out in `.github/workflows/main.yml`.

## Implementation guide

### Plan requirements

- (***Not started***) Translator tests exist for DTO→engine-call mapping
  - GIVEN a fake engine call surface
  - WHEN an adapter-facing provider receives DTOs
  - THEN the provider calls the fake engine API with correctly translated values.

- (***Not started***) Benchmark suite exists for DTO translation
  - GIVEN a benchmark project
  - WHEN the benchmark runs in Release
  - THEN it reports per-operation timing for translation hot paths.

- (***Not started***) CI can run benchmarks (optional but recommended)
  - GIVEN a PR build
  - WHEN the benchmark job is enabled
  - THEN BenchmarkDotNet results are produced as artifacts and published in the build summary.

### Phase 1 — Translator tests

***Not started***

#### Objective

Add unit tests that verify translation from DTOs (`SpriteDrawDto`, `TextDrawDto`, `MeshDrawDto`, `MaterialDto`, `TransformDto`) into an engine-facing API using fakes/spies.

#### Technical details

1. **Create a minimal fake engine call surface** inside the UnitTests project.
   - Example: `IFakeRenderBackend` with methods like `DrawSprite(string spriteId, TransformDto transform, MaterialDto material, int layer)`.
   - Implementation: `RecordingFakeRenderBackend` stores each call (method name + args) into a list.

2. **Create a sample translator provider** (test-only) that implements `IRenderProvider` and forwards to the fake backend.
   - Example type: `TranslatingRenderProvider : IRenderProvider`.
   - Translation should be intentionally simple and explicit (pass-through of IDs/transform/material/layer). The goal is to validate mapping patterns, not to build a real engine adapter.

3. **Write translator tests** validating:
   - Ordering: calls arrive in the same order as DTO submissions.
   - Fidelity: all DTO fields are forwarded without mutation.
   - Frame boundaries: `BeginFrame`/`EndFrame`/`Present` call sequences can be asserted if the translator provider chooses to forward them.

4. **Keep tests isolated**
   - Do not reuse `HeadlessRenderProvider` for translator tests; that provider records DTOs but does not exercise a DTO→engine mapping.

#### Phase requirements

- (***Not started***) Render DTO translation is verified
  - GIVEN a `TranslatingRenderProvider` backed by a recording fake backend
  - WHEN `SubmitSprite`, `SubmitText`, and `SubmitMesh` are invoked
  - THEN the fake backend receives equivalent calls with equivalent values.

- (***Not started***) Material and transform objects are forwarded correctly
  - GIVEN a DTO with non-default `TransformDto` and populated `MaterialDto.Uniforms`
  - WHEN the DTO is submitted
  - THEN the backend sees the same values.

### Phase 2 — Benchmarks

***Not started***

#### Objective

Add a BenchmarkDotNet benchmark project that measures DTO translation performance for representative hot paths.

#### Technical details

1. **Create a new benchmark project**
   - Path: `src/GameEngineAdapter.Benchmarks/GameEngineAdapter.Benchmarks.csproj`
   - References: `GameEngineAdapter.Core` and (optionally) a small internal translation implementation shared with tests.
   - Packages: `BenchmarkDotNet`.

2. **Define benchmarks**
   - Benchmark `TranslatingRenderProvider.SubmitSprite` with a fixed DTO.
   - Benchmark a loop over N submissions to reduce overhead noise.
   - Ensure Release configuration and `--filter *` works.

3. **Wire up the existing CI benchmark action (optional but recommended)**
   - Replace the `<path to benchmark project>` placeholders in `actions/steps/benchmark/action.yml` with the actual benchmark project path.
   - Uncomment/enable the `benchmark` job in `.github/workflows/main.yml` once the benchmark project exists.

#### Phase requirements

- (***Not started***) Benchmarks run locally
  - GIVEN the benchmark project
  - WHEN `dotnet run -c Release --project src/GameEngineAdapter.Benchmarks/...` is executed
  - THEN BenchmarkDotNet produces a markdown results file under `BenchmarkDotNet.Artifacts/results/`.

- (***Not started***) Benchmarks run in CI (optional)
  - GIVEN the workflow benchmark job is enabled
  - WHEN the CI pipeline runs
  - THEN benchmark results are uploaded and included in the job summary.

## See also

- [Phase 1 — Interface development](./phase-1-interface-development.md)

## References

- BenchmarkDotNet docs: <https://benchmarkdotnet.org/>
- Existing benchmark action scaffold: `actions/steps/benchmark/action.yml`
- CI workflow scaffold: `.github/workflows/main.yml`
