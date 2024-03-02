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
    $releasePath = "./bin/$arch/Release/net8.0-windows"

    dotnet build $projectFile -c Release /p:Platform=$arch -o $releasePath

    if (-not (Test-Path $releasePath)) {
        Write-Error "Build failed or output path is incorrect for architecture $arch."
        continue
    }

    $outputDir = "./out/$name/$arch"
    Remove-Item $outputDir -Recurse -Force -ErrorAction Ignore
    New-Item -ItemType Directory -Path $outputDir -Force

    Copy-Item "$releasePath/$assembly.dll", `
               "$releasePath/plugin.json", `
               "$releasePath/Images", `
               "$releasePath/$assembly.deps.json", `
               "$releasePath/OnePassword.NET.dll", `
               "$releasePath/Otp.NET.dll" `
               -Destination $outputDir -Recurse -Force
    Compress-Archive "$outputDir/*" "./out/$name-$version-$arch.zip" -Force
}

gh release create $version (Get-ChildItem ./out/*.zip -Name)
Pop-Location
