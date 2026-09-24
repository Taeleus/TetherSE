using System;
using Sandbox.Game;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Inventory;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Utils;
using VRageMath;

namespace TetherSE;

public static class Tether
{
    private static readonly MyStringHash CharacterInventoryId = MyStringHash.GetOrCompute("Inventory");

    public static void Update()
    {
        if (MyAPIGateway.Multiplayer == null || MySession.Static.LocalCharacter == null)
        {
            return;
        }

        var localPlayer = MySession.Static.LocalCharacter;
        var utils = MyAPIGateway.Utilities;

        if (ticks < 50)
        {
            ticks++;
            return;
        }

        ticks = 0;

        if (GetTargetedBlock.selectedBlock == null)
        {
            return;
        }

        if (Vector3D.Distance(localPlayer.PositionComp.GetPosition(),
                GetTargetedBlock.selectedBlock.GetPosition()) > Patches.maxUseDistance)
        {
            utils.ShowMessage("Tether Broke!",
                $"You Moved More Than {Patches.maxUseDistance}m from tethered block.");
            GetTargetedBlock.selectedBlock = null;
            GetTargetedBlock.selectedObject = null;
            return;
        }

        var equippedTool = MySession.Static.LocalCharacter.HandItemDefinition?.Id.SubtypeName;
        if (string.IsNullOrEmpty(equippedTool))
        {
            return;
        }

        if (equippedTool.Contains("Welder", StringComparison.OrdinalIgnoreCase))
        {
            if (localPlayer.BuildPlanner.Count == 0)
            {
                return;
            }

            DoWelder(localPlayer);
            return;
        }

        if (equippedTool.Contains("Grinder", StringComparison.OrdinalIgnoreCase))
        {
            DoGrinder();
            return;
        }

        if (equippedTool.Contains("Drill", StringComparison.OrdinalIgnoreCase))
        {
            DoDrill();
        }
    }

    private static void DoWelder(MyCharacter localPlayer)
    {
        Patches.UseObjectPatch(Patches.maxUseDistance);
        Reflections.Withdraw.Invoke(null,
            new object[] { (MyEntity)GetTargetedBlock.selectedObject.Owner,
                localPlayer.GetInventory(CharacterInventoryId), null });
        Patches.UseObjectPatch(5f);
    }

    private static void DoGrinder()
    {
        var inventory = (MyInventory)GetTargetedBlock.selectedBlock.GetInventory();
        var characterInventory = (MyInventory)MySession.Static.LocalCharacter.GetInventory(CharacterInventoryId);
        foreach (var item in characterInventory.GetItems())
        {
            var objectId = item.Content.GetObjectId().ToString();
            if (!objectId.Contains("ore", StringComparison.OrdinalIgnoreCase)
                && !objectId.Contains("ingot", StringComparison.OrdinalIgnoreCase)
                && !objectId.Contains("component", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            MyConstants.DEFAULT_INTERACTIVE_DISTANCE = 10000;
            MyInventory.TransferByPlanner(characterInventory, inventory,
                item.Content.GetObjectId(), MyItemFlags.None, item.Amount);
            MyConstants.DEFAULT_INTERACTIVE_DISTANCE = 10;
        }
    }

    private static void DoDrill()
    {
        var inventory = (MyInventory)GetTargetedBlock.selectedBlock.GetInventory();
        var characterInventory = (MyInventory)MySession.Static.LocalCharacter.GetInventory(CharacterInventoryId);
        foreach (var item in characterInventory.GetItems())
        {
            if (!item.Content.GetObjectId().ToString().Contains("ore", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            MyConstants.DEFAULT_INTERACTIVE_DISTANCE = 10000;
            MyInventory.TransferByPlanner(characterInventory, inventory,
                item.Content.GetObjectId(), MyItemFlags.None, item.Amount);
            MyConstants.DEFAULT_INTERACTIVE_DISTANCE = 10;
        }
    }

    private static int ticks;
}