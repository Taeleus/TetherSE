# Combat lockout validation

Release: **1.1.1**, two-minute cooldowns and observation waits. Player feedback
is the generic `Tether unavailable.` HUD notice, with no debug chat. Detailed
reasons remain in the game log.

1.1.0 verification (2026-10-26): both runtime builds and 268 regression
assertions pass. The installed Pulsar source compiler builds all 18 plugin
files. The maintainer confirmed the final gameplay checklist passed, including
combat/destruction, quiet connector/rotor behavior, vanilla framework absence,
and reconnect/world-switch checks using the source-compiled release candidate.

1.1.1 verification (2026-10-28): both runtime builds and all 268 assertions
pass, and the installed Pulsar compiler builds all 18 source files. This patch
only defers idle-weapon diagnostic text; the gameplay checklist above was
completed for 1.1.0.

Sections below labeled dev.7, dev.8 or dev.9 record historical debugging tests.
Use the current two-minute duration when repeating them.

Run from the repository root:

```powershell
dotnet run --project tests/CombatLockout.Tests/CombatLockout.Tests.csproj -c Release
```

The harness compiles the production cooldown, persistence, combat monitor,
WeaponCore observer, and own-grinding patches. It uses small game API doubles,
an in-memory storage double, and real Harmony patches applied to test methods.
It uses Lib.Harmony and no test framework.

Coverage includes vanilla fire/timestamps, signed replicated integrity changes,
health, own grinding through full dismantling, simultaneous incoming damage,
target switching, respawn, mechanical and connector groups, late/incompatible
WeaponCore APIs, sorter weapons and multiple weapon IDs, charge/fire events,
shot counters, core hit callbacks, non-weapon core components, cleanup ordering,
persisted session/restart cooldowns, corrupt/unavailable storage, and expiry.
Session/server replacement tests verify observer re-registration while retaining
the cooldown. The doubles reproduce API signatures and message payload shapes
from the locally installed SDX2/WeaponCore sources; they do not prove network
event delivery.

## Required gameplay checks

Use ordinary player accounts in a vanilla dedicated-server world and the SDX2
creative/test environment. Live server replication, API callback behavior, and
Nexus transitions should be repeated when changing relevant integration code.

- Damage armor, destroy a block outright and collide: lockout. Damage only the character: no timeout.
- Fire vanilla fixed guns, turrets, missiles, and a personal weapon, including
  single shots. Confirm ordinary tool activation does not count as gunfire.
- On SDX2, test sorter-based railguns, railgun charging, torpedo launchers,
  PDC defensive fire, and every firing part of a multi-weapon block. Test both
  the firing player's view and another player aboard the same ship.
- Receive SDX2 railgun, torpedo blast/fragment, PDC, and any available handheld
  core damage. Compare host and remote-client behavior; callback availability
  can differ with WeaponCore synchronization settings.
- Confirm first observation of any grid or WeaponCore weapon requires two quiet
  minutes; missing/incompatible core observation prevents transfers.
- Repeat damage/fire during cooldown. Try disconnecting, another port/ship,
  and a respawn. Reconnect only after two quiet minutes.
- Damage/fire on rotor, hinge, piston, and connector-docked grids. A supply
  station connected to a fighting ship must not evade the combat restriction.
- Grind your own blocks/armor through final removal with hand/ship grinders.
  Incoming damage and other-player grinding must still trigger the lockout.
- Cross a Nexus sector during cooldown, using both ordinary and seamless
  switching if available. Reconnect and restart the game during cooldown.
  Verify the remaining deadline survives; the saved deadline may add up to
  one second in historical builds. Current restored timeouts are capped at two minutes. Confirm expiration without new activity permits reconnection.
- Confirm existing enemy-overlap behavior remains, and movement alone does
  not disable Tether. There is no speed restriction or version command.
- Unload/reload worlds repeatedly and check for duplicate callbacks, warnings,
  or stale entity subscriptions. Check vanilla without WeaponCore installed.

The local file `TetherSE.CombatCooldown.v1.txt` in the game's global Storage
folder is runtime state; the harness uses an in-memory replacement and does not
write the real player's cooldown. Modified clients, unreplicated activity, and
other combat frameworks remain outside these client-side guarantees.

## Non-damage electronic warfare

- Verify ordinary manual block shutdowns, timers, script switches, and normal
  jump-drive use do not produce a combat lockout, with and without Vanilla+.
- On a WeaponCore server, receive EMP and jump-nullification without integrity
  damage. Verify affected-block state refreshes the cooldown while active.
- On a Vanilla+ server, test EMP missile impact, proximity/end-of-life EMP,
  EMP beams, and zero-damage jump-inhibition rounds, on a host and remote client.
- Check disarmed missiles, zero effect duration/strength, same-construct
  friendly-fire exclusions, and impacts outside your functional blocks' bounds.
- Test repeated EMP while blocks are already disabled, including server/client
  disagreement in projectile position or disarm state. The client fallback is
  based on observed framework detonations/impacts, not server acknowledgment.
- Join a vanilla world with no frameworks after visiting modded worlds. Confirm
  optional hooks are removed and normal tethering works after any active
  cooldown expires. Repeat with Vanilla+ and WeaponCore separately.

The test doubles cover these optional signatures and passive Harmony observers,
including framework absence, but do not reproduce the actual network transport.

## Development audit regressions

The harness also covers unknown-history grid reloads/reconnects/splits/merges,
rapid holster/removal after firing, dock/fire-or-damage/undock between updates,
provider cleanup failures, partial registrations, stale callbacks, closed
ownership representatives, and the distinction between an EMP beam impact and
its maximum-range endpoint. Existing quiet-world scenarios explicitly advance
past the observation interval; separate audit scenarios assert the real initial
lockout. No real player storage is modified.

Before publishing, repeat these cases in game:

- Let the observation interval expire, then repeat damage/fire immediately
  followed by holstering, switching tools, removing the gun or undocking.
- Damage a grid before selecting it; reload/re-replicate, reconnect, split or
  merge it. Tether must wait instead of accepting the missing history as peace.
- Fire an EMP beam at a nearby grid with its maximum trajectory endpoint far
  away. Confirm the actual impact causes the restriction on a remote client.
- Select an inventory block with no usable inventory port; disconnect and
  unload repeatedly. Check logs for interrupted cleanup or duplicate observers.
## Connector and player-damage retest (dev.7)

- Let both grids finish observation. Dock and undock without damage or fire:
  tether should stay usable. A newly observed docked grid still has its wait.
- Damage only a connector once: no combat timeout. Damage it again within one
  second: 15-second timeout. Repeat on host, remote client and WeaponCore.
- Wait over one second between isolated connector hits: no combat timeout.
- Destroy a connector outright or damage any other block: immediate timeout.
- Receive EMP/jump inhibition affecting a connector: immediate timeout.
- Bump/hurt the character, including a WeaponCore hit: no combat timeout.
  Fire a personal weapon: still triggers normally.
- Start a real ship combat timeout, then undock/reconnect: it must persist.
## Connector debug build (dev.8)

All timed cooldowns and observation intervals are now 15 seconds. Persistent
observer failures and enemy overlap are ongoing conditions, not timed waits.

Reproduce the connector issue once after restarting with dev.8. Keep the complete
`Tether DEBUG` chat line: it identifies the trigger, grid, block, damage channel
or weapon part and remaining time. Diagnostics are mirrored to the game log.
Try approaching without docking, then docking, then undocking after waiting for
each restriction to expire. Do not change the connector filter during this test;
the labels distinguish damage from newly observed entities or observer failures.
## Live connector notification retest (dev.9)

The dev.8 live log identified `Block removal` for a still-existing SDX collar.
The game emits these events during conveyor unlinking; they are now filtered
using owning grid and actual membership rather than a connector subtype list.

With both grids already observed and quiet, repeat entering the seat, docking,
undocking and approaching the collar. These topology changes should not produce
a block-removal timeout. Repeat with vanilla connectors and advanced rotors.
Actual repeated connector damage, removal/destruction, weapon fire and EMP must
still produce their specific debug cause and the 15-second timeout.