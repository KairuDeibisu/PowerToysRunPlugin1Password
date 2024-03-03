Push-Location
Set-Location $PSScriptRoot

# Ensure you're in a git repository and git is available
if (-not (Test-Path .git)) {
    Write-Error "This folder does not appear to be a git repository."
    Pop-Location
    exit
}

# Ensure the 'gh' command is available
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI ('gh') is not installed or not in PATH."
    Pop-Location
    exit
}

$name = '_1Password'
$assembly = "Community.PowerToys.Run.Plugin.$name"
$version = "v$((Get-Content ./plugin.json | ConvertFrom-Json).Version)"
$archs = @('x64', 'arm64')
$projectFile = "1Password.csproj"

git tag $version
git push --tags

Remove-Item ./out/*.zip -Recurse -Force -ErrorAction Ignore
foreach ($arch in $archs) {
    $releasePath = Join-Path $PSScriptRoot "bin/$arch/Release/net8.0-windows"

    dotnet build $projectFile -c Release /p:Platform=$arch -o $releasePath

    if (-not (Test-Path $releasePath)) {
        Write-Error "Build failed or output path is incorrect for architecture $arch."
        continue
    }

    $outputDir = Join-Path $PSScriptRoot "out/$name/$arch"
    Remove-Item $outputDir -Recurse -Force -ErrorAction Ignore
    New-Item -ItemType Directory -Path $outputDir -Force

    Copy-Item "$releasePath/$assembly.dll", `
               "$releasePath/plugin.json", `
               "$releasePath/Images", `
               "$releasePath/$assembly.deps.json", `
               "$releasePath/OnePassword.NET.dll", `
               "$releasePath/Otp.NET.dll" `
               -Destination $outputDir -Recurse -Force

    if ((Get-ChildItem $outputDir -File).Count -eq 0) {
        Write-Error "No files found to compress for architecture $arch."
        continue
    }

    $zipFilePath = Join-Path $PSScriptRoot "out/$name-$version-$arch.zip"
    Compress-Archive -Path "$outputDir/*" -DestinationPath $zipFilePath -Force
}

# Ensure we're still at the script's root directory for consistency
Set-Location $PSScriptRoot

# Collect the paths to all zip files in the 'out' directory
$zipFiles = Get-ChildItem -Path "./out" -Filter "*.zip" -Recurse | ForEach-Object { $_.FullName }

if ($zipFiles.Count -eq 0) {
    Write-Error "No zip files found to upload."
    Pop-Location
    exit
}

# Create a GitHub release and upload the zip files
gh release create $version $zipFiles --title "$name Release $version" --notes "Release notes here."

# Return to the previous location
Pop-Location
