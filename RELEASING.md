# Versioning and releases

## Version source

Edit `ClientPlugin/BuildVersion.cs`. Both local builds and Pulsar's source compiler
use its assembly attributes. `Version.Build.props` reads the Version constant for
MSBuild; no project-file version needs to be kept in sync manually.

- `Version`: the full public version, currently `1.1.2`.
- `AssemblyVersion` and `AssemblyFileVersion`: four numeric parts, currently
  `1.1.2.0`. Keep these aligned with the public version's numeric portion.
- `AssemblyInformationalVersion`: uses the full Version constant, including the
  prerelease label. Windows DLL details expose it as the product version.

Use patch versions for fixes, minor versions for compatible features, and major
versions for breaking changes. Increment the prerelease counter for distinct
published test builds, for example `1.1.1-dev.1`. A Git commit identifies the exact
source revision; version numbers identify named builds and releases.

## Branches and changelog

Develop on `dev` and group user-facing changes under the matching version in `CHANGELOG.md`. Record development snapshots explicitly; do not label an untagged build as a published release.
Merge reviewed, tested changes into `master`. Keep `dev` for ongoing work and bring
it up to date with master after each release.

## Publishing a release

1. Set the full version to the release number (for example `1.1.0`) and align the
   numeric assembly/file versions. Move Unreleased notes into a dated release
   section, then add a fresh Unreleased section.
2. Build the Legacy net48 target using the README instructions. Confirm the DLL's
   product/file versions and test that build in game.
3. Commit the release preparation on dev, review it, and merge it into master.
4. Tag that exact master commit `v1.1.0` and push the tag. Create a GitHub Release
   from that tag and copy its changelog section into the release notes. Mark any
   `-dev.N` or `-rc.N` release as a prerelease.
5. If distributing a compiled DLL, attach the build made from the tagged source.
   PluginHub continues to use its source commit pin rather than release assets.
6. Update the Commit in `TetherSE.xml` and the PluginHub submission to the same
   tested release commit. Commit the descriptor update separately; a descriptor
   cannot contain the hash of its own commit. Keep both descriptors identical.
7. Update dev from master. Start the next development version when new work begins.

Creating a branch or editing version metadata does not automatically publish a
release, push a tag, update PluginHub, or deploy a local plugin.

## Local development

Build Legacy without deploying:

```powershell
dotnet build ClientPlugin/ClientPlugin.csproj -c Release -f net48 -p:RunPostBuildEvent=Never
```

Set `-p:Bin64="D:\path\to\SpaceEngineers\Bin64"` if Steam auto-detection does
not find the installed game. Local path overrides can also live in the ignored
`Directory.Build.props.user`. The project includes a net10.0 build target too;
the public manifest currently selects Legacy/CLR.

To refresh the separate local test plugin from the `dev` checkout:

```powershell
.\Deploy-LocalDev.ps1
```

This builds `TetherSEDev` with its own ID and installs it in
`%APPDATA%\Pulsar\Legacy\Local\TetherSEDev`. It leaves the current selection
and public plugin files alone. The display name includes the build version.
Enable only one Tether version, and restart after replacing a loaded build.

The optional `-Enable` switch changes only the Current profile: it enables the
dev entry and disables public Tether. Close Pulsar/game before using that switch.
Use `-PulsarRoot` and `-Bin64` for custom install locations. Prior local files and
any edited profile are backed up under ignored `artifacts/local-dev-backups`.

Release candidates use the intended public cooldown and quiet HUD feedback.
Detailed timeout causes stay in the game log. The earlier 15-second waits and
`Tether DEBUG` chat belong to historical debugging snapshots.