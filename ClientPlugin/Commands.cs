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
        BoundsPreview.Clear();
        MyAPIUtilities.Static.MessageEntered += Command;
    }

    protected override void UnloadData()
    {
        MyAPIUtilities.Static.MessageEntered -= Command;
        Tether.Disconnect();
        BoundsPreview.Clear();
        base.UnloadData();
    }

    public override void Draw() { BoundsPreview.Draw(); }

    private static void Command(string message, ref bool sendToOthers)
    {
        var parts = (message ?? "").Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || !parts[0].Equals("/tether", StringComparison.OrdinalIgnoreCase)) return;
        sendToOthers = false;
        if (parts.Length == 1) GetTargetedBlock.GetPort();
        else if (parts.Length == 2 && parts[1].Equals("off", StringComparison.OrdinalIgnoreCase))
            Tether.Disconnect("Tether disconnected.");
        else if (parts.Length == 2 && parts[1].Equals("bounds", StringComparison.OrdinalIgnoreCase))
            BoundsPreview.Toggle();
        else if (parts.Length == 3 && parts[1].Equals("bounds", StringComparison.OrdinalIgnoreCase) &&
                 parts[2].Equals("off", StringComparison.OrdinalIgnoreCase))
        {
            BoundsPreview.Clear();
            MyAPIGateway.Utilities?.ShowNotification("Tether bounds hidden.", 3000, "White");
        }
        else MyAPIGateway.Utilities?.ShowNotification("Usage: /tether, /tether off, /tether bounds, /tether bounds off", 5000, "White");
    }
}
