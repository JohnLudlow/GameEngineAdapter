# TUI contract considerations

This document records the current contract boundary before a terminal UI adapter is designed. It is not an
implementation plan.

## Agreed first-release scope

The first usable TUI must include labels, checkboxes, lists, panels, buttons, and tab controls.

- Mouse click support is included; mouse move and scroll support are not part of this first scope.
- Tab and Shift+Tab must cycle deterministically through enabled controls.
- Full redraws are permitted, provided they remain visually stable during idle and normal interaction.

The detailed interaction semantics remain open. In particular, the design must define which controls are focusable, each
control's keyboard and click behavior, focus changes after controls are disabled or removed, and resize behavior.

## Agreed interaction baseline

The focusable controls are checkboxes, lists, buttons, tab controls, and panels or groups.

- Tab and Shift+Tab traverse enabled focusable controls deterministically.
- A panel or group is a container-entry point: focus enters its first enabled eligible child rather than stopping on the
  container itself.
- Space and Enter activate the focused interactive control.
- Clicking an enabled interactive control moves focus to that control before applying its control-specific action.
- Disabling the focused control retains focus but prevents activation; re-enabling it restores its ability to activate
  without moving focus.
- Removing the focused control moves focus to the next enabled eligible control, wrapping when necessary.

The per-control action semantics remain open. For example, the design must still define how checkboxes toggle, how lists
change selection, how tab controls change the visible page, and what action a button invokes.

## Agreed control-action baseline

- Space, Enter, and click toggle an enabled checkbox.
- Up and Down change a focused list's selection. Space and Enter activate the selected list item.
- Space, Enter, and click invoke an enabled button's assigned action exactly once.
- Left and Right change a focused tab control's selected page. Clicking a tab also selects its page. Space and Enter
  activate the selected page.

The application-specific effects of activating a list item, button, or tab page remain the responsibility of the
consumer. The shared UI contract should describe the input, selection, focus, and activation events without assuming
those domain effects.

## Agreed navigation-bounds baseline

- Pressing Up on the first selected list item or Down on the last selected list item keeps the current selection
  unchanged.
- Pressing Left on the first selected tab selects the last tab.
- Pressing Right on the last selected tab selects the first tab.

The initial selection, behavior for an empty list or tab control, and behavior after a selected item or page is removed
remain open decisions.

## Agreed selection-lifecycle baseline

- The consumer must provide the initial selection for every non-empty list and tab control.
- A missing initial selection, or a selection identifier not present in the control's current items or pages, rejects
  the control definition as invalid.
- An empty list or tab control renders as non-interactive; navigation and activation do nothing.
- When a selected item or page is removed, select the next remaining item or page. If no next item or page exists,
  select the previous remaining one.

The resulting selected item or page must be reported to the consumer so that its application state remains the source of
truth.

## Agreed resize and redraw baseline

- The adapter owns first-release layout using its current terminal viewport.
- A terminal resize recalculates layout but does not change consumer-owned UI or application state, including focus and
  selection.
- When the viewport cannot meet the UI's minimum layout requirements, the adapter renders an explicit minimum-size
  fallback view rather than silently changing the UI state.
- A terminal viewport change or visible UI-state change must be redrawn by the next frame. An unchanged idle UI does not
  require a redraw.
- The shared contract must remain extensible for a consumer to provide viewport-size rules for alternative layouts in a
  later release. The first release does not define that extension's API shape.

## Agreed TUI capability baseline

- The first-release TUI advertises no 2D or 3D rendering support.
- It supports mouse clicks and keyboard interaction.
- Color is not guaranteed. Consumer UI must not depend on color alone to convey information.

## Agreed headless test-observation baseline

Headless contract tests must observe:

- the focused control identifier;
- the selected item or page identifier;
- consumer events;
- the selected layout outcome or minimum-size fallback; and
- whether a visible change is rendered by the next frame.

These observations must not expose terminal-specific internals such as cursor positions, escape sequences, rendering
buffers, or terminal-library widgets.

## Agreed UI update baseline

- The MVP accepts immutable whole UI-tree updates from the consumer.
- Stable control identifiers and the contract boundary must permit incremental updates in a later release, when large UI
  trees make whole-tree replacement unnecessarily expensive.
- A consumer update may intentionally cause an adapter-to-consumer transition event when that behavior is defined for
  the transition. Events are not implicitly emitted for every update.

## Current state

`IUserInterfaceProvider` is an empty interface. It therefore supplies no cross-adapter representation for:

- controls such as checkboxes, lists, and panels;
- focus order or keyboard navigation;
- pointer or mouse events;
- control activation and value-change events; or
- UI layout and render invalidation.

The existing `IInputProvider` and `IRenderProvider` should be reviewed alongside any proposed UI contract, but neither
substitutes for a defined UI interaction model.

## Requirements to settle before implementation

Define and agree on the following observable behaviours:

1. **Control set:** which controls are required for the first usable TUI.
2. **Input:** keyboard commands, mouse click/move/scroll support, and input event delivery.
3. **Focus:** initial focus, deterministic Tab/Shift+Tab order, pointer focus, and what happens when the focused control
   is removed or disabled.
4. **Rendering:** full redraw versus invalidation/dirty regions, resize handling, and the frame at which updates become
   visible.
5. **Capabilities:** what terminal-specific capabilities should be advertised and how a consumer reacts when they are
   unavailable.
6. **Testability:** headless assertions for focus transitions, input dispatch, and output/invalidation decisions.

## Evidence from the prior prototype

The discarded prototype exhibited flickering, non-interactive controls, no apparent mouse support, and unpredictable tab
focus. Treat those as acceptance criteria for the next design:

- no visible redraw flicker during idle or normal interaction;
- keyboard activation and state changes work for every included control;
- mouse behaviour is either supported and tested or explicitly excluded;
- focus traversal is deterministic and testable.

Only after these requirements are represented in the shared contract should a TUI implementation select a terminal UI
library and begin adapter code.
