# TetherSE

Client-side inventory assistance for Space Engineers / Pulsar. This branch is a
compliance candidate, not an approved PluginHub release.

## Commands

- `/tether`: while holding a tool, target an inventory access port to connect.
  Selecting another target first disconnects the previous tether, even if the new
  target is invalid.
- `/tether off`: disconnect and remove the highlight.

## Bounds preview

- Use /tether bounds while looking at a grid to toggle its wireframes.
  Tool targeting is preferred; otherwise a camera ray selects a grid within 500 m.
- Green shows the actual working bounds. Orange shows the box expanded by 100 m
  on each axis. It is a guide: the true distance boundary is rounded at edges and
  corners, so some space inside the orange corners is already beyond break range.
- /tether bounds off hides the preview. It follows the selected grid as it moves
  and resizes. Previewing does not connect or authorize transfers.
- Preview clears on death, grid removal, world unload, and plugin disposal.
- Test enemy arrival/departure, mixed ownership, faction relation changes, and
  reconnection blocking. Confirm highlighting clears when enemies break a tether.
- Test the preview on a moving/rotating grid and after leaving/rejoining the world.
## Access and range

The selected block must be personally owned, or explicitly shared with the
player's faction by an owner in that same faction. The game's HasPlayerAccess
check must also pass. Unowned, neutral, enemy, and Share All blocks owned by
someone else are not eligible. Admin access does not override these restrictions.

All tools use the selected inventory's grid-local bounding box, transformed with
the grid's rotation and position. Subgrids are not combined into a larger box.

- Inside/on the box: request transfers.
- Outside, at most 100 m from the nearest point on the box: pause transfers.
- More than 100 m outside: break the tether.
- Detected enemy-owned blocks on a grid within 5 km: break the tether and reject
  reconnecting. This includes mixed ownership on the tethered grid itself.

The character position determines whether it is inside. Enemy proximity uses the
distance between enclosing world-axis-aligned grid boxes, not grid centers. This
is conservative: rotated grids can trigger exclusion farther than their true
oriented-box separation. Relations use the game's Enemies classification;
unowned blocks are not classified as enemies by this check.

Enemy scans inspect only entities replicated to the client. They cannot prove
that no enemy exists within 5 km. Checks run on connection, every 30 active
updates (including paused tethers), and before each transfer batch. At 60 updates
per second periodic detection can take up to about half a second. Scan failures
disconnect rather than permitting transfers. There is no server-side component.

Notifications appear on entering/leaving the paused state, not on every update.
The plugin revalidates the selected block and character every active update and
immediately before withdrawals and individual deposits. Losing access or the
character, closing the target, unloading the world, and `/tether off` clear it.

No game interaction-distance globals are modified. Transfer requests still go
through the game's existing server validation. Being inside Tether's range is
not a guarantee that a server will accept a request. The resumed notification
means requests are enabled again, not that a transfer has been acknowledged.

## Tools

- Welder: uses the game's build-planner withdrawal workflow, including its
  available-inventory/conveyor logic. The selected port is the access anchor.
- Grinder: deposits all carried ore, ingots, and components, including materials
  that were already in the character inventory.
- Drill: deposits all carried ore.

Transfers are attempted periodically while the corresponding tool is equipped;
pressing the tool's fire button is not required. Inventory capacity and server
checks still apply. No server companion is included.

## Build without deployment

From the repository root:

```powershell
dotnet build ClientPlugin/ClientPlugin.csproj -c Release -f net48 -p:Bin64="D:\SteamLibrary\steamapps\common\SpaceEngineers\Bin64" -p:RunPostBuildEvent=Never
```

Use `net10.0` instead for the other project target. Deployment is disabled by
default. Set `Pulsar` explicitly in your ignored `Directory.Build.props.user` or
on the command line only when deployment is intended. `RunPostBuildEvent=Never`
prevents deployment even with that setting. Building reads the game assemblies;
it does not modify them.

## Multiplayer validation checklist

Use a non-admin player on a dedicated server, as well as a local test world:

1. Verify connection and transfers for personally owned and faction-shared blocks.
2. Reject unowned, enemy, neutral, other-player private, and Share All targets.
3. Revoke sharing, leave the faction, or transfer ownership while tethered; verify
   disconnection and no subsequent requests.
4. Test all three tools inside, on a face, just outside, and 99/100/101 m outside
   the bounds. Repeat at corners and with a rotated/moving grid.
5. Verify one pause/resume notification per transition and no transfers while paused.
6. Compare being near the hull of a large ship with being far from the entire grid;
   server validation may differ. Confirm actual inventory changes on both sides.
7. Test full destination, empty source, incomplete build planner, and conveyor sources.
8. Test `/tether off`, invalid replacement selection, death/respawn, target deletion,
   and leaving/rejoining a world. Confirm no stale selection/highlight.
9. Verify unrelated `/tetherSomething` messages are not intercepted.
10. Confirm normal interaction reach remains unchanged before and after all tools.

The bounding-box/enemy-exclusion policy requires maintainer approval; removing a global override
alone does not establish compliance with PluginHub's multiplayer-advantage rule.

## Reporting issues

Report bugs at https://github.com/Taeleus/TetherSE/issues with your game/Pulsar
versions, runtime, reproduction steps, ownership/sharing setup, distance and
relevant log excerpt. Redact private server details and player identifiers.

For source-based testing, current Pulsar supports development folders via its
`-sources` option. Test a Release build before publication; an ordinary MSBuild
success does not verify Pulsar's own source compiler or in-game initialization.
