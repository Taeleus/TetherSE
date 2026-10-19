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

## Recheck against current upstream guidance (2026-10-18)

References reviewed:
- https://github.com/StarCpt/PluginHub (README and SamplePlugin.xml)
- https://github.com/CometWorks/client-plugin-template (README)
- https://github.com/CometWorks/skills (plugin development, client development,
  and plugin-review policy documents)

The automated se-dev-plugin-review workflow supports registry PRs, not this source
repository PR, so its preparation/posting workflow was not invoked. Its review
policy was used as reference. No skill package or preparation tools were installed.
The registry's actual sample supports GUID Id plus RepoId; that authoritative
schema takes precedence over conflicting owner/repo-ID wording in one skill page.

Additional corrections:
- Deployment is now opt-in: no automatic Pulsar-directory discovery. The evaluated
  default Pulsar property is empty. Explicit local path overrides still work.
- README now documents Release builds, source-loader testing, and bug reports.

Checks performed:
- Read all five runtime C# files and the PR diff; reflection targets a fixed game
  type/method. No external input selects code, and no HTTP, process execution,
  runtime file writes, updates, whitelist, DLC or Pulsar patches were found.
- Project assembly references resolve under the supplied game Bin64; framework
  references come from the installed SDK/reference assemblies. No active NuGet,
  analyzer, COM or project references are added. Publicizer samples remain comments.
  Unused template game-library references are not shipped assets. This is a path
  and dependency review, not a cryptographic verification of the Steam install.
- Reviewed Directory.Build.props, Version.Build.props and the deployment target.
  No Directory.Build.props.user exists in this checkout. App.config contains
  framework binding redirects; it is not registered as a loader asset.
- No committed DLL, executable or native assets, active descriptor packages, or
  active asset entries. SourceDirectories selects ClientPlugin only.
- Release builds for net48 and net10.0 passed. A separate net48 Release build with
  DefineConstants=TRACE and GenerateAssemblyInfo=false passed, exercising the
  non-LOCAL_BUILD source attributes. This is not Pulsar source-loader validation.
- Both builds report zero errors/warnings in their summaries; MSBuild additionally
  prints advisory binding-redirect suggestions for Unsafe and ValueTuple.
- Evaluated ImplicitUsings=disable and Pulsar empty; git diff --check passed.

Remaining human/runtime checks:
- No new gameplay changes were made during this recheck. The user's tested dev
  DLL is still the earlier Debug build; a Release/source-loader in-game run remains
  unperformed. No installed DLL was replaced during this recheck.
- The original multiplayer checklist and PluginHub range/fairness decision remain
  open. A normal build and successful admin-capable test do not settle those gates.
- No license file is present in the inherited repository. No new license was
  invented or applied to the original author's code during this review.

The PR remains draft and must not be merged without the owner's approval.

## Range reduction (2026-10-18)

The new range revision supersedes the 100/500 m limits documented above:
all tools transfer at or below 50 m, pause above 50 m through 200 m, and
disconnect above 200 m. Initial tether selection also requires 50 m proximity.
Notifications use the same constants. README boundary checks and the descriptor
description now reflect these limits. Prior gameplay test reports above apply
to the earlier limits; the new boundaries still require in-game testing.


## Bounding-box development revision (2026-10-18)

Supersedes the earlier 50/200 m limits. Uses character distance outside the selected
grid's oriented local bounds: transfer inside, pause through 100 m outside, break
beyond 100 m. Separate mechanically connected subgrids do not extend these bounds.
Existing target ownership/faction sharing and per-request access checks remain.

Enemy detection scans client-visible grids and their owned fat blocks, including
mixed ownership on the anchor. Uses the game's Enemies relation and conservative
world-AABB separation <= 5000 m. Scans at connection, every 30 updates while linked,
and before each transfer batch. An enemy breaks the tether and prevents reconnect;
scan failure also rejects/disconnects. This cannot detect unreplicated entities.
No global distance changes or server component were added.

Added /tether bounds and /tether bounds off. Green wireframe is the working box;
orange is a 100 m expansion along each axis, an enclosing guide rather than the
exact rounded Euclidean break boundary at corners. Rendering follows grid motion
and clears on death/unload/disposal. It does not change the tether selection.

Validation: net48 Release build passed. An external harness executed the exact
geometry methods against VRage.Math with 16 passing assertions for rotated and
translated bounds, corners, symmetry, and 100 m / 5 km boundaries. No test C# files
were added to plugin sources. Enemy detection, rendering, multiplayer behavior
and server acceptance still require in-game testing. No performance benchmark.
Prior test reports above apply to earlier versions, not this implementation.

## Preview refinement (2026-10-18)

User reported successful gameplay testing of the bounds/enemy revision, but
excessively thick preview lines. Reduced inner/outer width parameters from
0.05/0.10 to a shared 0.0005, retaining colors and placement.
Enabled bounds now expire 30 seconds after an actual tether disconnection,
including manual off. Uses monotonic elapsed time, not frame counts. Reconnection
cancels expiry; repeated disconnected updates cannot restart it. Explicit preview
off and lifecycle cleanup still clear immediately. A standalone preview is not
timed until a connected tether disconnects.


## Description readability and gameplay feedback (2026-10-18)

Replaced the selector tooltip with a short purpose statement. Details now use
plain-text sections, blank lines and one command per line; XML newline entities
preserve formatting. Updated repository and local dev metadata, retaining their
distinct plugin identities. Descriptions remain below 1000 characters.
No DLL rebuild was needed for this metadata-only change.

User confirmed the thinner wireframes look good, adjust in real time, and the
30-second disappearance timer works in game. The revised description formatting
still needs visual confirmation in Pulsar.


## Enemy overlap revision (2026-10-19)

Replaced the 5 km exclusion with intersection of the two grids' oriented bounding
boxes. The 5 km enclosing-AABB distance is only a candidate filter; being nearby
does not itself block transfers. Mixed ownership and the tethered grid itself
remain checked. Touching bounds count as intersection. Existing access checks,
scan cadence, transfer bounds and 100 m disconnect buffer are unchanged.
Updated README and both repository/local dev XML descriptions.

Validation: net48 Release build passed; 12 oriented-overlap checks passed using
the exact method and game math library (including touching and rotated boxes).
Installed TetherSEDev DLL hash verified. User confirmed the new behavior works in game.
