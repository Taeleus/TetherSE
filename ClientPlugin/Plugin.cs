using System.Reflection;
using VRage.Plugins;
using TetherSE;

// Define assembly version when compiled by Pulsar
#if !LOCAL_BUILD
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#endif
    
namespace ClientPlugin;

// ReSharper disable once UnusedType.Global
public class Plugin : IPlugin
{
    public const string Name = "TetherSE";

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public void Init(object gameInstance)
    {
        Reflections.Initialize();
    }

    public void Dispose()
    {
    }

    public void Update()
    {
        Tether.Update();
    }

}