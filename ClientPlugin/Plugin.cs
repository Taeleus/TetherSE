using VRage.Plugins;
using TetherSE;

namespace ClientPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin
{
    public const string Name = "TetherSE";

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
        OwnGrindingPatches.Initialize();
        Reflections.Initialize();
    }

    public void Dispose()
    {
        Tether.Disconnect();
        BoundsPreview.Clear();
        CombatLockout.Clear();
        OwnGrindingPatches.Dispose();
    }

    public void Update()
    {
        Tether.Update();
    }

}
