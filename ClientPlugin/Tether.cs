using System;
using System.Linq;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Inventory;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

namespace TetherSE;

public static class Tether
{
    // Provisional review limits; these never alter game interaction distances.
    private static int enemyCheckTicks;

    private static readonly MyStringHash CharacterInventoryId = MyStringHash.GetOrCompute("Inventory");
    private static int ticks;
    private static bool outOfRange;
    private static MyCharacter linkedCharacter;

    public static bool HasAccess(IMyTerminalBlock block)
    {
        var player = MySession.Static?.LocalHumanPlayer;
        if (player == null || block == null || block.Closed || block.MarkedForClose || !block.HasInventory)
            return false;
        var identity = player.Identity.IdentityId;
        if (identity == 0 || block.OwnerId == 0 || !block.HasPlayerAccess(identity))
            return false;
        if (block.OwnerId == identity)
            return true;
        var cube = block as MyCubeBlock;
        if (cube?.IDModule == null || cube.IDModule.ShareMode != MyOwnershipShareModeEnum.Faction)
            return false;
        var factions = MyAPIGateway.Session?.Factions;
        var ours = factions?.TryGetPlayerFaction(identity);
        var theirs = factions?.TryGetPlayerFaction(block.OwnerId);
        return ours != null && theirs != null && ours.FactionId == theirs.FactionId;
    }

    public static void Connected()
    {
        linkedCharacter = MySession.Static?.LocalCharacter;
        ticks = 0;
        enemyCheckTicks = 0;
        outOfRange = false;
    }

    public static void Disconnect(string message = null)
    {
        GetTargetedBlock.ClearSelection();
        linkedCharacter = null;
        outOfRange = false;
        ticks = 0;
        enemyCheckTicks = 0;
        if (message != null)
            MyAPIGateway.Utilities?.ShowNotification(message, 4000, "White");
    }

    private static bool CanTransfer()
    {
        var character = MySession.Static?.LocalCharacter;
        var block = GetTargetedBlock.selectedBlock;
        if (block == null) return false;
        if (character == null || character != linkedCharacter || character.Closed || character.IsDead)
        {
            Disconnect("Tether disconnected: character changed or unavailable.");
            return false;
        }
        if (!HasAccess(block))
        {
            Disconnect("Tether disconnected: inventory unavailable or access revoked.");
            return false;
        }
        var grid = block.CubeGrid as MyCubeGrid;
        if (grid == null || grid.Closed || grid.MarkedForClose)
        {
            Disconnect("Tether disconnected: grid unavailable.");
            return false;
        }
        var distance = TetherArea.DistanceOutside(grid, character.PositionComp.GetPosition());
        if (distance > TetherArea.BreakDistance)
        {
            Disconnect("Tether broke: more than 100 m outside the grid bounds.");
            return false;
        }
        var paused = distance > 0;
        if (paused != outOfRange)
        {
            outOfRange = paused;
            MyAPIGateway.Utilities?.ShowNotification(paused
                ? "Outside tethered grid bounds. Transfers paused."
                : "Tether back in range. Transfers resumed.", 4000, "White");
        }
        return !paused;
    }

    public static void Update()
    {
        if (MySession.Static == null || MyAPIGateway.Session == null)
        {
            Disconnect();
            return;
        }
        if (GetTargetedBlock.selectedBlock == null) return;
        // Scan even while paused. Also rescan immediately before a transfer batch.
        if (++enemyCheckTicks >= 30)
        {
            enemyCheckTicks = 0;
            if (!CheckEnemies()) return;
        }
        // Validate every update, even while waiting between transfer batches.
        if (!CanTransfer()) return;
        if (++ticks < 51) return;
        if (!CheckEnemies()) return;
        ticks = 0;
        enemyCheckTicks = 0;
        var character = MySession.Static.LocalCharacter;
        var tool = character.HandItemDefinition?.Id.SubtypeName;
        if (string.IsNullOrEmpty(tool)) return;
        try
        {
            if (tool.IndexOf("Welder", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (character.BuildPlanner.Count == 0 || !CanTransfer()) return;
                if (Reflections.Withdraw == null)
                {
                    Disconnect("Tether unavailable: game withdrawal method not found.");
                    return;
                }
                Reflections.Withdraw.Invoke(null, new object[] {
                    (MyEntity)GetTargetedBlock.selectedObject.Owner,
                    character.GetInventory(CharacterInventoryId), null });
            }
            else if (tool.IndexOf("Grinder", StringComparison.OrdinalIgnoreCase) >= 0)
                Deposit(false);
            else if (tool.IndexOf("Drill", StringComparison.OrdinalIgnoreCase) >= 0)
                Deposit(true);
        }
        catch (Exception exception)
        {
            MyLog.Default.WriteLineAndConsole("TetherSE transfer failed: " + exception);
            Disconnect("Tether disconnected: transfer failed. See game log.");
        }
    }

    private static bool CheckEnemies()
    {
        var grid = GetTargetedBlock.selectedBlock?.CubeGrid as MyCubeGrid;
        if (TetherArea.CheckEnemies(grid, out var reason)) return true;
        Disconnect(reason);
        return false;
    }
    private static void Deposit(bool oreOnly)
    {
        var source = (MyInventory)MySession.Static.LocalCharacter.GetInventory(CharacterInventoryId);
        // Transfers can mutate the inventory immediately when hosting a world.
        foreach (var item in source.GetItems().ToArray())
        {
            var type = item.Content.GetObjectId().TypeId.ToString();
            var accepted = type == "MyObjectBuilder_Ore" || (!oreOnly &&
                (type == "MyObjectBuilder_Ingot" || type == "MyObjectBuilder_Component"));
            if (!accepted) continue;
            if (!CanTransfer()) return;
            var destination = (MyInventory)GetTargetedBlock.selectedBlock.GetInventory();
            MyInventory.TransferByPlanner(source, destination, item.Content.GetObjectId(),
                MyItemFlags.None, item.Amount);
        }
    }
}
