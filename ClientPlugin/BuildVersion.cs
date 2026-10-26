using System.Reflection;

[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]
[assembly: AssemblyInformationalVersion(TetherSE.BuildVersion.Version)]

namespace TetherSE;

// Shared by local builds and Pulsar's source compiler.
internal static class BuildVersion
{
    public const string Version = "1.1.0";
}
