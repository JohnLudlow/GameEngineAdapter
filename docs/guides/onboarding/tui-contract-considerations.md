# TUI contract considerations

This document records the current contract boundary before a terminal UI
adapter is designed. It is not an implementation plan.

## Agreed first-release scope

The first usable TUI must include labels, checkboxes, lists, panels, buttons, and tab controls.

- Mouse click support is included; mouse move and scroll support are not part of this first scope.
- Tab and Shift+Tab must cycle deterministically through enabled controls.
- Full redraws are permitted, provided they remain visually stable during idle and normal interaction.

The detailed interaction semantics remain open. In particular, the design must define which controls are focusable, each control's keyboard and click behavior, focus changes after controls are disabled or removed, and resize behavior.

## Agreed interaction baseline

The focusable controls are checkboxes, lists, buttons, tab controls, and panels or groups.

- Tab and Shift+Tab traverse enabled focusable controls deterministically.
- A panel or group is a container-entry point: focus enters its first enabled eligible child rather than stopping on the container itself.
- Space and Enter activate the focused interactive control.
- Clicking an enabled interactive control moves focus to that control before applying its control-specific action.
- Disabling the focused control retains focus but prevents activation; re-enabling it restores its ability to activate without moving focus.
- Removing the focused control moves focus to the next enabled eligible control, wrapping when necessary.

The per-control action semantics remain open. For example, the design must still define how checkboxes toggle, how lists change selection, how tab controls change the visible page, and what action a button invokes.

## Agreed control-action baseline

- Space, Enter, and click toggle an enabled checkbox.
- Up and Down change a focused list's selection. Space and Enter activate the selected list item.
- Space, Enter, and click invoke an enabled button's assigned action exactly once.
- Left and Right change a focused tab control's selected page. Clicking a tab also selects its page. Space and Enter activate the selected page.

The application-specific effects of activating a list item, button, or tab page remain the responsibility of the consumer. The shared UI contract should describe the input, selection, focus, and activation events without assuming those domain effects.

## Agreed navigation-bounds baseline

- Pressing Up on the first selected list item or Down on the last selected list item keeps the current selection unchanged.
- Pressing Left on the first selected tab selects the last tab.
- Pressing Right on the last selected tab selects the first tab.

The initial selection, behavior for an empty list or tab control, and behavior after a selected item or page is removed remain open decisions.

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
