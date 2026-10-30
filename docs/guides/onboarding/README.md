# GameEngineAdapter onboarding

This guide provides the shortest reliable route through the repository for a contributor who needs to understand its
contracts before changing or adding an adapter.

## Start the CodeTour

1. Install the [CodeTour](https://marketplace.visualstudio.com/items?itemName=vsls-contrib.codetour) VS Code extension.
2. Open the repository root in VS Code.
3. Run **CodeTour: Start CodeTour** from the Command Palette.
4. Choose **GameEngineAdapter onboarding**.

The tour is stored in [`.tours/onboarding.tour`](../../../.tours/onboarding.tour). CodeTour discovers that location
automatically.

## Reading path

Follow the steps in this order:

1. Read the root [`README.md`](../../../README.md) for the public purpose and current roadmap.
2. Open [`GameEngineAdapter.slnx`](../../../GameEngineAdapter.slnx) to see the four projects.
3. Read the contracts in [`src/GameEngineAdapter.Core`](../../../src/GameEngineAdapter.Core), beginning with
   `IEngineAdapter`, `EngineCapabilities`, and `IRenderProvider`.
4. Read [`HeadlessAdapter`](../../../src/GameEngineAdapter.Headless/HeadlessAdapter.cs) as the concrete reference
   implementation.
5. Trace [`DeterministicEngineRunner`](../../../src/GameEngineAdapter.Headless/DeterministicEngineRunner.cs) into
   [`CombatIntegrationTests`](../../../src/GameEngineAdapter.UnitTests/CombatIntegrationTests.cs).
6. Use the [deterministic integration-testing guide](../deterministic-integration-testing.md) when writing a
   cross-provider scenario.

## Source of truth

The current C# contracts are the authoritative API surface. Planning documents provide intent and historical context,
but their illustrative snippets may lag behind the code. Verify proposed work against the interfaces in
`src/GameEngineAdapter.Core` and the tests that exercise them.

## TUI exploration

Do not begin a TUI implementation by mapping graphical draw calls directly to terminal controls. First read
[TUI contract considerations](tui-contract-considerations.md). In particular, `IUserInterfaceProvider` is currently
empty, so control interaction, focus order, and mouse behaviour are not yet represented by the shared contract.

## Next references

- [Architecture map](architecture-map.md)
- [Phase 1 interface-development plan](../../plans/phase-1-interface-development.md)
- [Deterministic integration testing](../deterministic-integration-testing.md)
