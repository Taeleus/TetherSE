using System;
using Sandbox.Game.Entities;
using Sandbox.Game.World;
using VRage.Game;
using VRage.Utils;
using VRageMath;

namespace TetherSE;

// Only inspects entities replicated to this client; not server-authoritative.
public static class TetherArea
{
    public const double BreakDistance = 100.0;
    public const double EnemyDistance = 5000.0;

    public static double DistanceOutside(MyCubeGrid grid, Vector3D position)
    {
        var local = Vector3D.Transform(position, MatrixD.Invert(grid.WorldMatrix));
        var bounds = grid.PositionComp.LocalAABB;
        var closest = Vector3D.Clamp(local, (Vector3D)bounds.Min, (Vector3D)bounds.Max);
        return Vector3D.Distance(local, closest);
    }

    // Enclosing world AABBs deliberately underestimate separation between rotated
    // grids: conservative exclusion, never a false clearance due to rotation.
    public static double BoundsDistance(BoundingBoxD a, BoundingBoxD b)
    {
        var x = Math.Max(0, Math.Max(a.Min.X - b.Max.X, b.Min.X - a.Max.X));
        var y = Math.Max(0, Math.Max(a.Min.Y - b.Max.Y, b.Min.Y - a.Max.Y));
        var z = Math.Max(0, Math.Max(a.Min.Z - b.Max.Z, b.Min.Z - a.Max.Z));
        return Math.Sqrt(x * x + y * y + z * z);
    }

    public static bool CheckEnemies(MyCubeGrid anchor, out string reason)
    {
        reason = null;
        try
        {
            var identity = MySession.Static?.LocalHumanPlayer?.Identity?.IdentityId ?? 0;
            if (identity == 0 || anchor == null || anchor.Closed || anchor.MarkedForClose)
            {
                reason = "Tether unavailable: player or grid unavailable.";
                return false;
            }
            foreach (var entity in MyEntities.GetEntities())
            {
                var grid = entity as MyCubeGrid;
                if (grid == null || grid.Closed || grid.MarkedForClose) continue;
                if (BoundsDistance(anchor.PositionComp.WorldAABB, grid.PositionComp.WorldAABB) > EnemyDistance)
                    continue;
                // Include the anchor itself: enemy-owned blocks on mixed grids count.
                foreach (var block in grid.GetFatBlocks())
                {
                    if (block.Closed || block.MarkedForClose || block.OwnerId == 0) continue;
                    if (block.GetUserRelationToOwner(identity) != MyRelationsBetweenPlayerAndBlock.Enemies)
                        continue;
                    reason = "Tether blocked: detected enemy-owned grid/block within 5 km.";
                    return false;
                }
            }
            return true;
        }
        catch (Exception exception)
        {
            MyLog.Default.WriteLineAndConsole("TetherSE enemy check failed: " + exception);
            reason = "Tether disconnected: enemy check failed. See game log.";
            return false;
        }
    }
}