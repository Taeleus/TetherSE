using HarmonyLib;
using VRage.Game;

namespace TetherSE;

public static class Patches
{
    public static void UseObjectPatch(float useDistance)
    {
        if (useDistance > maxUseDistance)
        {
            useDistance = maxUseDistance;
        }

        var field = AccessTools.Field("VRage.Game.MyConstants:DEFAULT_INTERACTIVE_DISTANCE");
        field.SetValue(null, useDistance);
    }

    public static float maxUseDistance = 500f;
}