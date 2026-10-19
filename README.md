# TetherSE

Client-side inventory assistance for Space Engineers / Pulsar. This branch is a
compliance candidate, not an approved PluginHub release.

## Commands

- `/tether`: while holding a tool, target an inventory access port to connect.
  Selecting another target first disconnects the previous tether, even if the new
  target is invalid.
- `/tether off`: disconnect and remove the highlight.

## Access and range

The selected block must be personally owned, or explicitly shared with the
player's faction by an owner in that same faction. The game's HasPlayerAccess
check must also pass. Unowned, neutral, enemy, and Share All blocks owned by
someone else are not eligible. Admin access does not override these restrictions.

All tools use the same provisional limits, defined in ClientPlugin/Tether.cs:

- At or below 50 m: request transfers.
- Above 50 m through 200 m: keep the selection, but pause transfers.
- Above 200 m: disconnect completely.

Distances are from the character position to the selected block position.
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
4. Test all three tools at 49, 50, 51, 199, 200, and 201 m from the block.
5. Verify one pause/resume notification per transition and no transfers while paused.
6. Compare being near the hull of a large ship with being far from the entire grid;
   server validation may differ. Confirm actual inventory changes on both sides.
7. Test full destination, empty source, incomplete build planner, and conveyor sources.
8. Test `/tether off`, invalid replacement selection, death/respawn, target deletion,
   and leaving/rejoining a world. Confirm no stale selection/highlight.
9. Verify unrelated `/tetherSomething` messages are not intercepted.
10. Confirm normal interaction reach remains unchanged before and after all tools.

The 50 m transfer range requires maintainer approval; removing a global override
alone does not establish compliance with PluginHub's multiplayer-advantage rule.

## Reporting issues

Report bugs at https://github.com/Taeleus/TetherSE/issues with your game/Pulsar
versions, runtime, reproduction steps, ownership/sharing setup, distance and
relevant log excerpt. Redact private server details and player identifiers.

For source-based testing, current Pulsar supports development folders via its
`-sources` option. Test a Release build before publication; an ordinary MSBuild
success does not verify Pulsar's own source compiler or in-game initialization.
