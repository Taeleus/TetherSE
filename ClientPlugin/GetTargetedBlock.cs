using System.Collections.Generic;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.Entity.UseObject;
using VRage.Game.ModAPI;
using VRageMath;

namespace TetherSE;

public static class GetTargetedBlock
{
    public static void ClearSelection()
    {
        var previous = selectedBlock;
        selectedBlock = null;
        selectedObject = null;
        if (previous != null && !previous.Closed && !previous.MarkedForClose && MySession.Static != null)
            MyVisualScriptLogicProvider.SetHighlightLocal(previous.Name, -1, 0,
                Color.White, MySession.Static.LocalPlayerId, null);
    }

    public static void GetPort()
    {
        Tether.Disconnect();
        var utils = MyAPIGateway.Utilities;
        var character = MySession.Static?.LocalCharacter;
        var caster = character?.EquippedTool?.Components?.Get<MyCasterComponent>();
        var block = caster?.HitBlock;
        if (block?.FatBlock is IMyTerminalBlock terminalBlock)
        {
            if (!Tether.HasAccess(terminalBlock))
            {
                utils.ShowNotification("Tether requires your own inventory or an accessible faction-shared inventory.", 5000, "Red");
                return;
            }
            var grid = terminalBlock.CubeGrid as MyCubeGrid;
            if (grid == null || TetherArea.DistanceOutside(grid, character.PositionComp.GetPosition()) > TetherArea.BreakDistance)
            {
                utils.ShowNotification("Move within 100 m of the grid bounds to connect.", 4000, "White");
                return;
            }
            if (!TetherArea.CheckEnemies(grid, out var reason))
            {
                utils.ShowNotification(reason, 5000, "Red");
                return;
            }
            var objects = terminalBlock.Components.Get<MyUseObjectsComponentBase>();
            var usableObjects = new List<IMyUseObject>();
            objects?.GetInteractiveObjects(usableObjects);
            foreach (var usableObject in usableObjects)
            {
                if (usableObject.PrimaryAction != UseActionEnum.OpenInventory) continue;
                selectedBlock = terminalBlock;
                selectedObject = usableObject;
                // Preserve scenario/visual-script entity names.
                if (string.IsNullOrEmpty(terminalBlock.Name))
                {
                    terminalBlock.Name = terminalBlock.EntityId.ToString();
                    MyEntities.SetEntityName((MyEntity)terminalBlock);
                }
                MyVisualScriptLogicProvider.SetHighlightLocal(terminalBlock.Name, 2, 3,
                    Color.ForestGreen, MySession.Static.LocalPlayerId, null);
                Tether.Connected();
                utils.ShowNotification($"Tethered to {terminalBlock.CustomName}", 4000, "White");
                return;
            }
        }
        utils?.ShowNotification("Target a block with an inventory access port while holding a tool.", 4000, "Red");
    }

    public static IMyUseObject selectedObject;
    public static IMyTerminalBlock selectedBlock;
}
