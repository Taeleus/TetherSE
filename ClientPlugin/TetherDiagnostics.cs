using System;
using Sandbox.ModAPI;
using VRage.Utils;

namespace TetherSE;

internal static class TetherDiagnostics
{
    public static void Show(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        MyLog.Default.WriteLineAndConsole("TetherSE: " + message);
        try { MyAPIGateway.Utilities?.ShowNotification(message, 4000, "White"); }
        catch (Exception exception) { MyLog.Default.WriteLineAndConsole("TetherSE notification failed: " + exception.Message); }
    }
}