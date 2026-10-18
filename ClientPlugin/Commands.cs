using System;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace TetherSE;

[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
public class Commands : MySessionComponentBase
{
    public override void BeforeStart()
    {
        Tether.Disconnect();
        MyAPIUtilities.Static.MessageEntered += Command;
    }

    protected override void UnloadData()
    {
        MyAPIUtilities.Static.MessageEntered -= Command;
        Tether.Disconnect();
        base.UnloadData();
    }

    private static void Command(string message, ref bool sendToOthers)
    {
        var parts = (message ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || !parts[0].Equals("/tether", StringComparison.OrdinalIgnoreCase)) return;
        sendToOthers = false;
        if (parts.Length == 1) GetTargetedBlock.GetPort();
        else if (parts.Length == 2 && parts[1].Equals("off", StringComparison.OrdinalIgnoreCase))
            Tether.Disconnect("Tether disconnected.");
        else MyAPIGateway.Utilities?.ShowNotification("Usage: /tether or /tether off", 4000, "White");
    }
}
