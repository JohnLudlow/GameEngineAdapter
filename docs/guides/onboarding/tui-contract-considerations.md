# TUI contract considerations

This document records the current contract boundary before a terminal UI
adapter is designed. It is not an implementation plan.

## Current state

`IUserInterfaceProvider` is an empty interface. It therefore supplies no
cross-adapter representation for:

- controls such as checkboxes, lists, and panels;
- focus order or keyboard navigation;
- pointer or mouse events;
- control activation and value-change events; or
- UI layout and render invalidation.

The existing `IInputProvider` and `IRenderProvider` should be reviewed
alongside any proposed UI contract, but neither substitutes for a defined UI
interaction model.

## Requirements to settle before implementation

Define and agree on the following observable behaviours:

1. **Control set:** which controls are required for the first usable TUI.
2. **Input:** keyboard commands, mouse click/move/scroll support, and input
   event delivery.
3. **Focus:** initial focus, deterministic Tab/Shift+Tab order, pointer focus,
   and what happens when the focused control is removed or disabled.
4. **Rendering:** full redraw versus invalidation/dirty regions, resize
   handling, and the frame at which updates become visible.
5. **Capabilities:** what terminal-specific capabilities should be advertised
   and how a consumer reacts when they are unavailable.
6. **Testability:** headless assertions for focus transitions, input dispatch,
   and output/invalidation decisions.

## Evidence from the prior prototype

The discarded prototype exhibited flickering, non-interactive controls, no
apparent mouse support, and unpredictable tab focus. Treat those as acceptance
criteria for the next design:

- no visible redraw flicker during idle or normal interaction;
- keyboard activation and state changes work for every included control;
- mouse behaviour is either supported and tested or explicitly excluded;
- focus traversal is deterministic and testable.

Only after these requirements are represented in the shared contract should a
TUI implementation select a terminal UI library and begin adapter code.
