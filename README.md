# TetherSE

Connect your welder, grinder, or drill to an owned or faction-shared inventory in Space Engineers.

- **Welders** pull components requested in your build planner through the inventory's accessible conveyor network.
- **Grinders and drills** send materials from your character back to the selected inventory.

Requires **Pulsar Legacy**. No server mod is required.

## Getting started

1. Enable **TetherSE** in Pulsar and start the game.
2. Equip a welder, grinder, or drill.
3. Aim at an inventory access port you own, or one explicitly shared with your faction.
4. Type `/tether` in chat.
5. Work inside the selected grid's bounds. Use `/tether bounds` to show the working area.

For welding, add the blocks or components you need to the game's build planner. Transfers run periodically while the appropriate tool is equipped; you do not need to hold the fire button.

## Commands

| Command | Action |
| --- | --- |
| `/tether` | Connect to the inventory port you are targeting with a tool. |
| `/tether off` | Disconnect. |
| `/tether bounds` | Show or hide the targeted grid's bounds. |
| `/tether bounds off` | Hide the bounds preview. |

Selecting another target disconnects the previous one, even if the new target is unavailable. Showing bounds does not connect a tether.

## Where it works

The working area follows the bounds of the grid containing your selected inventory. Separate subgrids do not enlarge it.

| Your position | What happens |
| --- | --- |
| Inside the grid's bounds | Transfers work. |
| Outside, up to 100 m beyond the bounds | Transfers pause. Returning inside resumes them. |
| More than 100 m outside | The tether breaks. Use `/tether` to reconnect. |

The preview shows the working area in green and a 100 m guide in orange. The orange box extends beyond the actual break distance at its corners. The preview hides 30 seconds after disconnection.

Bounds previews require more than half of the grid's ownable blocks to belong to you or be explicitly shared with your faction. Unowned blocks count against that majority; armor without ownership does not count. The preview closes if access is lost.

## Ownership and access

Your selected inventory must be personally owned or explicitly faction-shared by a member of your faction. Unowned inventories, enemy inventories, and another player's inventories set only to **Share All** cannot be selected. Losing access disconnects the tether.

The ownership rule applies to the selected port. Welders can pull from accessible conveyor-connected inventories, including docked grids and unowned or Share All inventories when the game's normal permissions allow it. Tether does not grant access to locked cargo. Grinder and drill deposits go to the selected inventory.

An enemy grid overlapping the working bounds blocks tethering. Enemy-owned blocks on a mixed-ownership grid can also block it. Nearby enemies outside the bounds do not trigger this check.

## Combat cooldown

Tether disconnects and becomes unavailable for **two minutes after the latest qualifying ship damage, weapon fire, or electronic-warfare effect**. Activity on mechanical subgrids and connector-docked ships counts too. Your own handheld weapon fire also counts.

- Bumping or hurting your character does **not** start a cooldown.
- Grinding your own blocks is allowed. Incoming damage while grinding still counts.
- An isolated connector hit is tolerated. Repeated connector damage within one second, or outright block destruction, triggers the cooldown.
- Ordinary docking and undocking do not count as combat.
- Newly observed grids and WeaponCore weapons need a two-minute observation period. This includes joining or reconnecting, new grids, split fragments, and merge survivors.

Changing targets, undocking, respawning, or reconnecting does not clear an existing cooldown. Continued combat extends it. Use `/tether` to reconnect after the restriction clears.

Combat restrictions show **Tether unavailable.** Detailed reasons are kept in the game log. Enemy overlap and unavailable weapon observers can keep tethering blocked until those conditions clear.

Combat detection supports vanilla weapons and WeaponCore, with EMP and jump-inhibition detection for WeaponCore and Vanilla+ when present. Tether observes information received by your client; it is not a server-enforced anti-cheat system.

## Troubleshooting

If Tether will not connect:

- Keep a tool equipped and aim directly at an inventory access port.
- Check ownership and faction sharing, then check your distance from the grid.
- Allow two quiet minutes after joining, encountering a new grid, or recent combat.
- If it remains unavailable, include your game log when reporting the issue.

Report bugs through [GitHub issues](https://github.com/Taeleus/TetherSE/issues). Include your Tether/Pulsar versions, server or modpack, and steps to reproduce the problem.

See the [changelog](CHANGELOG.md) for version history, [release and local development instructions](RELEASING.md), and [validation checklist](tests/CombatLockout.Tests/README.md).
