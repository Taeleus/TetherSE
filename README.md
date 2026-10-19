# TetherSE

TetherSE links your hand tools to an owned or faction-shared inventory in Space Engineers. Welders pull components from the linked inventory, while grinders and drills send materials back.

Built for **Pulsar Legacy (.NET Framework 4.8)**. No companion server mod is required.

## Getting started

1. Equip a welder, grinder, or drill.
2. Aim at a block with an inventory access port that you own or can access through faction sharing.
3. Type `/tether` in chat to connect.
4. Work inside that grid's bounding box. Use `/tether bounds` to see the working area.
5. Type `/tether off` when you want to disconnect.

For welding, add the components you need to the game's build planner. Transfers run periodically while the corresponding tool is equipped; you do not need to hold the fire button.

## Commands

| Command | Action |
| --- | --- |
| `/tether` | Connect to the inventory port you are targeting with a tool. |
| `/tether off` | Disconnect and remove the inventory highlight. |
| `/tether bounds` | Toggle wireframe bounds for the grid you are looking at. |
| `/tether bounds off` | Hide the wireframe preview. |

Selecting another tether target disconnects the previous one, even if the new target is invalid. Showing bounds does not connect a tether.

## Working area and disconnect distance

The working area is the bounding box of the grid containing your selected inventory. It follows the grid as it moves, rotates, or changes size. Separate subgrids are not combined into one working area.

| Your position | Tether behavior |
| --- | --- |
| Inside or on the grid's bounds | Transfers are enabled. |
| Outside, up to 100 m beyond the bounds | Transfers pause; the tether stays connected. |
| More than 100 m beyond the bounds | The tether disconnects. |

Distance is measured from your character to the nearest point on the box, not from the selected inventory or the grid's center. Returning inside resumes a paused tether automatically. After a break, use `/tether` to connect again.

## Ownership and enemy overlap

The selected inventory must be personally owned or explicitly shared with your faction by an owner in that faction. Normal inventory-access permissions must also allow access. Unowned blocks and another player's blocks set only to Share All are not eligible. Admin access does not bypass these requirements.

Access is checked during operation and before transfers. Losing permission disconnects the tether.

An enemy grid whose bounding box overlaps the tethered grid's bounding box breaks the tether and prevents reconnection while the overlap remains. Both boxes follow their grids' rotation and position. Nearby enemies outside the bounds do not block tethering. This also applies to overlapping mixed-ownership grids containing enemy-owned blocks, including the tethered grid itself. Enemy status follows the game's ownership and faction relations.

## Wireframe preview

Use `/tether bounds` while looking at a grid. Targeting it with a hand tool works too; without a tool target, the preview can select a grid up to 500 m away.

- **Green:** the working area where transfers are enabled.
- **Orange:** the 100 m break-range guide. The actual break boundary has rounded corners, so the wireframe box extends slightly beyond it at the corners.

Both boxes adjust in real time as the grid moves, rotates, or changes size. Enabled bounds disappear 30 seconds after the tether disconnects. Use `/tether bounds off` to hide them immediately.

## Tool behavior

| Tool | Transfers |
| --- | --- |
| Welder | Pulls build-planner components using the game's inventory and conveyor workflow. |
| Grinder | Sends carried ore, ingots, and components to the linked inventory. |
| Drill | Sends carried ore to the linked inventory. |

Grinder and drill transfers include matching items already in your inventory. Available space and the game's normal transfer checks still apply. The plugin does not change the game's global interaction distance.

## Building locally

From the repository root, build the Legacy version with your game's Bin64 path:

```powershell
dotnet build ClientPlugin/ClientPlugin.csproj -c Release -f net48 -p:Bin64="D:\SteamLibrary\steamapps\common\SpaceEngineers\Bin64" -p:RunPostBuildEvent=Never
```

The output is `ClientPlugin/bin/Release/net48/TetherSE.dll`. The project also contains a `net10.0` target; the current plugin descriptor selects the Legacy runtime.

Automatic deployment is disabled by default. Local paths can be configured in the ignored `Directory.Build.props.user` file. `RunPostBuildEvent=Never` keeps deployment disabled even when a Pulsar path is configured.

When using the separate TetherSEDev local build, enable it instead of the original TetherSE so only one version runs.

## Reporting issues

Open an [issue](https://github.com/Taeleus/TetherSE/issues) with your game and Pulsar versions, steps to reproduce, ownership/sharing setup, and any relevant game-log excerpt.
