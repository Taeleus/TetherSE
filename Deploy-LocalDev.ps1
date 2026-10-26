[CmdletBinding()]
param(
    [string]$PulsarRoot = (Join-Path $env:APPDATA 'Pulsar'),
    [string]$Bin64,
    [switch]$Enable
)

$ErrorActionPreference = 'Stop'
$repo = $PSScriptRoot
$branch = & git -C $repo branch --show-current
if ($LASTEXITCODE -ne 0 -or $branch -ne 'dev') {
    throw 'Run this deployment from the TetherSE dev checkout.'
}
if ($Enable -and (Get-Process -Name Legacy,Interim,SpaceEngineers -ErrorAction SilentlyContinue)) {
    throw 'Close Space Engineers and Pulsar before automatically changing the Current profile, or omit -Enable and select the plugin yourself.'
}
$legacy = Join-Path $PulsarRoot 'Legacy'
if (!(Test-Path -LiteralPath $legacy -PathType Container)) {
    throw "Pulsar Legacy data folder not found: $legacy"
}

$buildArguments = @('build', (Join-Path $repo 'ClientPlugin\ClientPlugin.csproj'),
    '-c', 'Release', '-f', 'net48', '-p:AssemblyName=TetherSEDev',
    '-p:RunPostBuildEvent=Never', '--no-restore', '-v', 'minimal')
if ($Bin64) { $buildArguments += "-p:Bin64=$Bin64" }
& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) { throw 'Development build failed; deployment was not changed.' }

$output = Join-Path $repo 'ClientPlugin\bin\Release\net48'
$dll = Join-Path $output 'TetherSEDev.dll'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($dll).ProductVersion
if ([Reflection.AssemblyName]::GetAssemblyName($dll).Name -ne 'TetherSEDev') {
    throw 'Expected a separate TetherSEDev assembly.'
}
$devId = 'D00F6A1F-5992-4FC0-8ACF-A3E94BFB61CB'
$publicId = '57F5514B-B0B8-4332-BFB9-35FB08BB76F4'
$destination = Join-Path $legacy 'Local\TetherSEDev'
$backup = Join-Path $repo ('artifacts\local-dev-backups\' + [DateTime]::Now.ToString('yyyyMMdd-HHmmss-fff'))
New-Item -ItemType Directory -Path $backup -Force | Out-Null
foreach ($name in @('plugin.dll','plugin.pdb','plugin.xml')) {
    $old = Join-Path $destination $name
    if (Test-Path -LiteralPath $old -PathType Leaf) { Copy-Item -LiteralPath $old -Destination $backup }
}

$metadata = [xml][IO.File]::ReadAllText((Join-Path $repo 'TetherSE.xml'))
$metadata.PluginData.Id = $devId
$metadata.PluginData.FriendlyName = "TetherSEDev ($version)"
$metadata.PluginData.Tooltip = "Local dev build $version - enable instead of public TetherSE."
$metadata.PluginData.Description = "LOCAL DEV $version. Enable instead of public TetherSE.`n`n" + $metadata.PluginData.Description
$metadata.PluginData.Runtimes = 'CLR'
$commit = $metadata.PluginData.SelectSingleNode('Commit')
if ($commit) { [void]$metadata.PluginData.RemoveChild($commit) }
if ($metadata.PluginData.Description.Length -gt 1000) { throw 'Local plugin description exceeds 1000 characters.' }
New-Item -ItemType Directory -Path $destination -Force | Out-Null
Copy-Item -LiteralPath $dll -Destination (Join-Path $destination 'plugin.dll') -Force
Copy-Item -LiteralPath (Join-Path $output 'TetherSEDev.pdb') -Destination (Join-Path $destination 'plugin.pdb') -Force
$metadata.Save((Join-Path $destination 'plugin.xml'))

if ($Enable) {
    $profilePath = Join-Path $legacy 'Profiles\Current.xml'
    $profile = [xml][IO.File]::ReadAllText($profilePath)
    Copy-Item -LiteralPath $profilePath -Destination (Join-Path $backup 'Current.xml')
    # Disable only the public Tether entry in the current profile.
    foreach ($entry in @($profile.SelectNodes('/Profile/GitHub/GitHubPluginConfig'))) {
        if ($entry.Id -ieq $publicId -or $entry.Id -ieq 'Taeleus/TetherSE') {
            [void]$entry.ParentNode.RemoveChild($entry)
        }
    }
    $local = $profile.SelectSingleNode('/Profile/Local')
    if ($null -eq $local) {
        $local = $profile.CreateElement('Local')
        [void]$profile.DocumentElement.AppendChild($local)
    }
    if (!(@($local.SelectNodes('string')) | Where-Object { $_.InnerText -ieq $devId })) {
        $entry = $profile.CreateElement('string')
        $entry.InnerText = $devId
        [void]$local.AppendChild($entry)
    }
    $profile.Save($profilePath)
}

Write-Host "Deployed TetherSEDev $version to $destination"
if ($Enable) { Write-Host 'Enabled local dev in Current; public Tether is disabled in that profile.' }
Write-Host "Previous local files saved in $backup"
