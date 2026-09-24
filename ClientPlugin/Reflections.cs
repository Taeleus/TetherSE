using System.Reflection;
using Sandbox.Game.Gui;

namespace TetherSE;

public static class Reflections
{
    public static void Initialize()
    {
        Withdraw = typeof(MyGuiScreenGamePlay).GetMethod(
            "ProcessWithdraw", BindingFlags.NonPublic | BindingFlags.Static);
    }

    public static MethodInfo Withdraw;
}