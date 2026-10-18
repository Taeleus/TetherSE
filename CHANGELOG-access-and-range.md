# Access and range revision

## Problem and behavior

The previous client modified the global interaction-distance value around
transfers, accepted relationships that did not establish ownership, and left the
highlight behind on a range break. The revision removes all distance-global
writes, requires personal ownership or explicit faction sharing plus the game's
access check, and revalidates the selected inventory during operation.

All tools share a provisional 100 m transfer range. Transfers pause above 100 m;
the selection disconnects above 500 m. Transition notifications avoid repeated
messages. `/tether off`, range breaks, unavailable targets, character replacement,
and session cleanup all clear the selection and highlight.

The selected port anchors the range check. Welder withdrawals retain Keen's
build-planner/conveyor source selection; contributing inventories are not each
subject to a separate 100 m radius. Transfers run while a relevant tool is equipped.

## Packaging

- Legacy deployment targets .NET Framework 4.8; the descriptor explicitly selects
  NETFramework. The project also retains its existing net10.0 build target.
- Implicit usings are explicitly disabled.
- The unused Harmony dependency and global-distance patch source are removed.
- No whitelist, DLC, Pulsar behavior, auto-update, or runtime assembly-location
  changes are included. No executable assets are registered.
- The descriptor's source pin identifies the implementation commit; subsequent
  metadata-only commits can update that pin without changing the compiled source.

## Validation and limits

- Both net48 and net10.0 builds passed before packaging cleanup; final build
  validation is recorded in the pull request.
- The user tested the separate Legacy TetherSEDev deployment and reported working
  welding/grinding, pausing beyond 100 m, disconnecting beyond 500 m, and highlight
  cleanup on disconnect.
- The user reported rejection of an unowned grid, followed by successful tether
  selection after assigning personal ownership and disabling admin tools.
- Fully unprivileged server transfer validation remains unconfirmed: disabling
  admin tools may not remove the account's admin rank. Faction sharing/revocation,
  drill behavior, full inventories and lifecycle cases still need the README's
  multiplayer checklist. Do not treat those cases as already tested.
- Inspection of the installed game IL found that the admin branch of
  CheckCanAddItems emits the misleading "Spawned items!" log during transfer
  validation. The user also reproduced it without this plugin.
- No benchmark was performed. In-range batch cadence remains 51 updates; the
  added access checks run client-side. Pausing suppresses transfer requests
  between 100 and 500 m. No server sim-rate improvement is claimed.

## Review status

The multiplayer-advantage rule remains subject to PluginHub maintainer approval,
including acceptance of the 100 m transfer distance. This revision is a candidate,
not an assertion that the plugin has been accepted into PluginHub.

The PR must remain unmerged until the repository owner approves it. The original
local plugin and game installation are not modified by this revision's deployment.
