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
