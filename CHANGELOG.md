# Changelog

Changes are grouped by Tether version. Historical development entries describe
local prerelease snapshots; published releases have their own dated section.

## Unreleased

No changes yet.

## 1.1.0 — 2026-10-26

### Added
- Two-minute combat lockout for qualifying ship damage, personal or ship weapon
  fire, and supported electronic-warfare effects, including mechanical and
  connector-docked constructs. Cooldowns survive target changes and reconnects.
- Vanilla and optional WeaponCore weapon observation, plus optional WeaponCore
  and Vanilla+ EMP/jump-inhibition support without requiring those frameworks.
- Ownership restrictions for bounds previews: a strict majority of ownable
  blocks must belong to you or be explicitly shared with your faction. Access
  changes close the preview automatically.
- Shared version metadata, per-version changelog and local development deployment.

### Fixed
- Live connector/advanced-rotor topology notifications no longer count as block
  destruction or duplicate/remove weapon observers. Real destruction still counts.
- Failed selection and provider cleanup paths release caches and callbacks.
  Closing ownership representatives no longer hide other live owned blocks.
- Combat history cannot be discarded through rapid holstering, weapon removal,
  undocking, grid reloads, splits or merges. Newly observed grids/WeaponCore
  weapons and merge survivors require a two-minute observation interval.
- EMP beams use their actual impact location instead of maximum-range endpoints.

### Behavior
- Player health loss and your own grinding do not cause combat timeouts.
- An isolated connector hit is tolerated; repeated connector damage within one
  second still counts, with duplicate reports filtered. Destruction is immediate.
- Public feedback is `Tether unavailable.` with no diagnostic chat or countdown;
  detailed reasons remain in the game log. Existing enemy-overlap checks remain.
- The README focuses on getting started, commands, access, range and combat rules.

### Validation
- Both plugin targets build successfully; 268 regression assertions pass.
- The installed Pulsar source compiler successfully compiles all 18 plugin files.
- The maintainer confirmed the final in-game checklist, including the corrected
  connector behavior and the candidate's two-minute, quiet release settings.

## 1.1.0-rc.1 — 2026-10-26 (release candidate, not published)

### Changed
- Restored two-minute combat cooldowns and observation waits for release.
- Removed diagnostic chat and detailed timeout notifications. Players see
  `Tether unavailable.`; detailed reasons remain in the game log.
- Simplified the README around setup, commands, access, range and combat rules.
- Retained the validated connector/advanced-rotor topology fix, connector-damage
  filtering, player-damage exemption, combat persistence and optional framework
  support developed in the preceding snapshots.

### Validation
- Automated regression coverage includes quiet player feedback, detailed log
  provenance and strict two-minute restoration of persisted cooldowns.
- The installed Pulsar source compiler successfully built all 18 plugin files.
- Final candidate gameplay checks precede the public release
  commit and PluginHub update. No release tag or public update is created here.

## 1.1.0-dev.9 — 2026-10-26 (debug build)

### Fixed
- Connector and advanced-rotor conveyor notifications were incorrectly treated
  as destroyed blocks. Vanilla connector detach notifies each grid that the
  other grid's live collar was removed; the live dev.8 log showed this exact
  `Block removal` trigger for `sdx_connectorDockingLarge1`.
- Ignore foreign-grid and still-resident removal notifications, including
  same-grid topology changes. These events no longer drop or duplicate optional
  weapon observers. Real local removals still trigger and clean up normally.
- Explicit split/transfer movement no longer counts as block destruction.
  New-fragment/merge observation and genuine damage history remain intact.

### Validation
- Added mirrored connector notifications, same-grid notifications, watcher
  retention, actual destruction and split/transfer regression scenarios.
- Keeps 15-second debug waits and local reason messages for gameplay retesting.

## 1.1.0-dev.8 — 2026-10-26 (debug build)

### Changed
- Combat cooldowns, grid/merge observation waits and WeaponCore observation
  waits are reduced to 15 seconds for debugging. Old persisted timeouts are
  capped at 15 seconds on load, including the normal write-ahead margin.
- Breaks and blocked connection attempts print local chat, HUD and game-log
  diagnostics. Timed reasons follow the originating activity through docking,
  undocking and group refresh rather than becoming a generic unavailable message.
- Diagnostics distinguish new-grid/merge and WeaponCore observation, native
  damage versus replicated integrity versus WeaponCore hits, repeated connector
  damage, block removal, vanilla fire, core part fire/prefire/counter events,
  electronic warfare, restored timeouts, and observer/API failures.
- Damage diagnostics include amount, grid name/ID and block definition/ID when
  available. Unresolved fat blocks are labeled explicitly. Ongoing conditions
  such as enemy overlap or failed observers are identified as ongoing blocks.

### Validation
- Existing combat/access behavior is preserved apart from the shorter duration
  and diagnostic output. The connector cause remains under investigation.
- Added regressions for strict 15-second expiry/restoration, reason retention,
  private chat delivery, per-part WeaponCore reasons and unrelated-activity silence.

## 1.1.0-dev.7 — 2026-10-25 (development)

### Changed
- Player health loss and WeaponCore hits on the player no longer start or extend
  a combat timeout. Personal weapon fire still does.
- Connector-only structural damage requires repeated hits within one second on
  the same grid. Each observation path has its own window so one hit reported
  by native damage, replicated integrity and WeaponCore is not counted three times.
  Multiple component-integrity callbacks in one simulation frame count once.
- Other block damage, outright block destruction and non-damage EMP/jump
  inhibition remain immediate triggers. Existing ownership/bounds checks stay.

### Validation
- Added connector-window boundary, duplicate-report, player-damage exemption,
  quiet docking/undocking and newly observed docked-grid regression scenarios.
- Gameplay testing of dev.6 confirmed combat, access and bounds behavior, with
  docking damage and player bump damage motivating these changes.
- A fresh two-minute observation interval after reconnecting remains intentional.
  Two reported reconnect crashes were native access violations; no Tether
  exception identifies their cause. Crash dumps remain available for diagnosis.

## 1.1.0-dev.6 — 2026-10-25 (development)

### Fixed
- Failed or interrupted port selection now disposes scan caches even if a
  highlight or other selection step throws. Highlight cleanup cannot interrupt
  disconnect or world teardown. Combat target selection waits for a usable port.
- Ownership scans rebuild when a cached representative closes, keeping the
  remaining live blocks in that ownership group visible.
- WeaponCore callbacks become inert before unregistering, even if the provider
  throws during cleanup. Partial registrations are tracked and retired on retry.
- Final weapon samples and live construct-link checks preserve shots/damage
  followed by holstering, weapon removal, or undocking between plugin updates.
  A failed removal-time sample disables tethering while completing cleanup.
- Vanilla+ EMP beams now use the actual impact location rather than the maximum
  trajectory endpoint. A validated passive IL observer preserves original EMP
  behavior; unsupported beam method layouts disable the optional integration.

### Changed
- Every newly observed grid needs two minutes of observation before tethering,
  including newly built, re-replicated and split grids. Merge survivors restart
  this interval. This closes damage-history loss through reloads/reconnects and
  grid transformations. Only the selected construct imports that restriction;
  observing unrelated activity does not create a warning or proximity signal.

### Validation
- Expanded production-source regressions cover the lifecycle and bypass cases.
- Both plugin targets compile against the installed game assemblies. The beam
  observer also accepts the compiled unmodified stable Vanilla+ Workshop method.
- Live dedicated-server, SDX2/Nexus, and remote-client replication tests remain
  required before publishing.

## 1.1.0-dev.5 — 2026-10-25 (development)

### Fixed
- Selecting an inventory block without an `OpenInventory` use object no longer
  creates enemy-scan ownership caches and event subscriptions that persist until
  the next disconnect or selection. The scan now runs only after finding a
  usable inventory port, before connecting; enemy checks remain mandatory.

## 1.1.0-dev.4 — 2026-10-25 (development)

### Changed
- `/tether bounds` requires a strict majority of the selected grid's ownable
  blocks to be owned by you or explicitly faction-shared by your own faction.
  Unowned blocks count against access; armor without ownership is excluded.
  Ties, enemy grids, and fully unowned grids cannot show a preview.
- Visible bounds close silently when ownership, sharing, faction membership,
  or grid composition no longer meets that rule. Access is checked before drawing.

## 1.1.0-dev.3 — 2026-10-25 (development)

### Added
- Optional non-damage combat observation for WeaponCore EMP/jump-nullification
  using affected-block state, and Vanilla+ EMP/jump-drive inhibition using its
  effect application paths and client-observed missile/beam EMP endpoints.
- A passive Vanilla+ client fallback because its actual EMP block-disable
  callback executes on the server. Disarmed missiles, duplicate closures,
  zero-strength effects, and friendly-fire exclusions are filtered.
- Active synchronized/framework-reported effects refresh the cooldown without
  requiring structural damage. Ordinary block switches and jump-drive energy
  consumption are not treated as combat.

### Compatibility and validation
- No Vanilla+ or WeaponCore assembly dependency is added. Worlds without these
  frameworks keep the normal vanilla path; recognized frameworks that cannot
  be observed prevent tethering instead of bypassing the restriction.
- Optional hooks and callbacks detach on world unload or server replacement.
- Regression tests cover framework absence, non-damaging effects, ordinary
  switches/jumps, disarmed/friendly fire, unrelated grids, beam exceptions,
  ongoing effects, and cleanup. Live server validation remains outstanding.
- Client EMP fallback observes local framework detonations/beam endpoints and
  their configured area; it is not server confirmation that an EMP was applied.

## 1.1.0-dev.2 — 2026-10-25 (development)

### Added
- Optional WeaponCore combat observation alongside vanilla guns: sorter-based
  weapons, every weapon part, firing/charging callbacks, shot counters, and
  projectile-hit callbacks. No proximity, targeting, or radar queries are used
  for the combat lockout.
- A conservative two-minute grace period when a WeaponCore weapon is first
  observed; known core weapons with unavailable observers deny tethering.
- Cooldown persistence through session unloads, Nexus transitions, reconnects,
  and game restarts, with a one-second write-ahead margin in local storage.

### Changed
- Monitor connector-docked logical constructs as well as mechanical subgrids,
  matching the conveyor network that can supply build-planner components.
- Known WeaponCore/SDX2 worlds require a ready observer API. Monitoring/storage
  failures prevent transfers instead of silently permitting them.
- Version metadata identifies this development build as `1.1.0-dev.2`.

### Validation
- Regression coverage includes the production combat observer, actual Harmony
  hooks, vanilla and WeaponCore API doubles, persistence, lifecycle cleanup,
  and own-grinding exemptions. This does not establish live server replication
  or Nexus compatibility; the gameplay checklist remains outstanding.
- Movement remains unrestricted. Version information is supplied through the
  assembly metadata; the existing command set is unchanged.

## 1.1.0-dev.1 — 2026-10-25 (development snapshot)

### Added
- A two-minute damage and weapon-fire lockout for the character and selected
  ship, including mechanical subgrids. Repeated activity extends the cooldown;
  disconnecting, switching targets, or respawning does not clear it. Existing
  enemy-overlap checks remain in place.
- An exemption for your own grinding on personally owned blocks or armor on
  your own grid, including final block removal. Incoming damage still counts
  while grinding.
- Shared version metadata for local builds and Pulsar source builds, release
  instructions, and this versioned changelog.

## Pre-versioning baseline

The current master baseline is commit `14a910c`. Its build metadata used 1.0.0;
this entry records existing behavior, not a previously published GitHub release.

### Features and changes already included
- Transfers inside the tethered grid's bounds, pausing outside and disconnecting
  more than 100 m beyond the bounds.
- Owned or faction-shared tether targets with access checks during operation.
- Vanilla conveyor access for welder withdrawals; grinder and drill deposits
  into the selected inventory.
- Disconnection when an enemy grid's bounds overlap the tethered grid's bounds.
- Live green working-area and orange break-range wireframes, with automatic
  hiding 30 seconds after disconnection.
- Explicit disconnect and bounds commands.
- Ownership grouping cached between block and ownership changes, with live
  faction-relation checks and cleanup when grids leave the working area.
- Preservation of existing block names and removal of global interaction-distance
  overrides.

Earlier development notes are preserved in [CHANGELOG-access-and-range.md](CHANGELOG-access-and-range.md).
