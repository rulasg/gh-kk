[CmdletBinding()]
param (
    [Parameter()][string]$Project = "/Users/rulasg/Downloads/gh-kk/gh-kk.csproj",
    [Parameter()][string]$Tag
)

# Stop on errors

$ErrorActionPreference = "Stop"
# Define the runtime identifiers (RIDs) for .NET
# Mapping from Go platforms to .NET RIDs based on the readme.md table
$runtimes = @{
    "osx-x64" = "darwin-amd64"
    "osx-arm64" = "darwin-arm64"
    "linux-x86" = "linux-386"
    "linux-arm" = "linux-arm"
    "linux-x64" = "linux-amd64"
    "linux-arm64" = "linux-arm64"
    "win-x86" = "windows-386"
    "win-x64" = "windows-amd64"
}

# Create dist and uploads directories if they don't exist
$distDir = "dist"
$uploads = "uploads"
if (-not (Test-Path -Path $distDir)) {
    New-Item -ItemType Directory -Path $distDir
}
# Reset uploads directory
if (Test-Path -Path $uploads) {
    Remove-Item -Path $uploads -Recurse -Force
} 
New-Item -ItemType Directory -Path $uploads

# Build for each runtime
foreach ($runtime in $runtimes.Keys) {
    Write-Host "Building for $runtime..."
    
    # Set extension based on whether it's Windows
    $extension = ""
    if ($runtime.StartsWith("win-")) {
        $extension = ".exe"
    }
    
    # Publish for the specific runtime
    dotnet publish $Project --configuration Release --runtime $runtime --ucr /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --output "$distDir/$runtime"

    # Find binary file (ignore .pdb files) and copy to uploads with Go platform name
    $binaryName = "gh-kk$extension"
    $binaryPath = "$distDir/$runtime/$binaryName"
    
    if (Test-Path -Path $binaryPath) {
        $goPlatform = $runtimes[$runtime]
        $goFileName = "gh-kk-$goPlatform$extension"
        Copy-Item -Path $binaryPath -Destination "$uploads/$goFileName"
        Write-Host "Copied binary to $uploads/$goFileName"
    } else {
        Write-Warning "Binary file not found at $binaryPath"
    }

}

# Check if there are any files in dist and uploads directories
$distFiles = Get-ChildItem -Path $distDir
$uploadFiles = Get-ChildItem -Path $uploads

if ($distFiles.Count -eq 0) {
    Write-Error "No files found in dist/"
    exit 1
}

if ($uploadFiles.Count -eq 0) {
    Write-Error "No files found in uploads/"
    exit 1
}

# Generate checksums if GPG_FINGERPRINT is set
$includePattern = "dist/* $uploads/*"
if ($env:GPG_FINGERPRINT) {
    foreach ($file in (Get-ChildItem -Path $distDir)) {
        Get-FileHash -Path $file.FullName -Algorithm SHA256 | ForEach-Object { "$($_.Hash.ToLower())  $($file.Name)" } | Out-File -Append -FilePath "checksums.txt" -Encoding utf8
    }
    
    # Sign checksums with GPG
    gpg --output checksums.txt.sig --detach-sign checksums.txt
    $includePattern = "dist/* checksums*"
}

# Get the latest tag
if([string]::IsNullOrEmpty($Tag)) {
    Write-Host "No tag provided, fetching latest tag from git..."
    $Tag = git describe --tags --abbrev=0
} else {
    git tag -a $Tag -m "Release tag" -s
}

# Push tags to 
git push --tags

# Determine if this is a prerelease (contains hyphen in tag)
$prerelease = ""
if ($tag -match ".*-.*") {
    $prerelease = "-p"
}

$reponame = (git remote get-url origin) -replace 'https://github.com/','' -replace '\.git$',''

# Generate release notes
# gh api repos/$reponame/releases/generate-notes -f tag_name="$tag" -q .body | Out-File -FilePath "CHANGELOG.md" -Encoding utf8

# Create the release
$releaselist = gh release list --json tagName | ConvertFrom-Json
if($releaselist.tagName -contains $tag) {
    Write-Host "Release $tag already exists, skipping creation."
} else {
    Write-Host "Creating release $tag..."
    gh release create $tag $prerelease --generate-notes --latest=true
}

foreach ($file in (Get-ChildItem -Path $uploads)) {
    Write-Host "Uploading $($file.Name) to release..."
    gh release upload $tag $file.FullName --clobber
}
