<#
.SYNOPSIS
    Builds and releases the gh-kk project.

.DESCRIPTION
    This script contains functions for building and releasing the gh-kk project.
    Will create a release on rulasg/gh-kk repository and will upload the built binaries to the release.

.NOTES
    File Name      : build-and-release.ps1
    Author         : rulasg
    Prerequisite   : PowerShell 5.0 or later
    Version        : 1.0.0

.EXAMPLE
    # Build and release with specific version. Does not build if the binary already exists.
    Invoke-BuildAndRelease -Version "v1.2.3"

.EXAMPLE
    # Build and release with specific version, forcing a rebuild even if the binary exists.
    Invoke-BuildAndRelease -Version "v1.2.3" -Force
#>


function Get-TagAndSuffix {
    [CmdletBinding()]
    param(
        [string]$Tag
    )
    $versionString = $Tag.StartsWith('v') ? $Tag.Substring(1) : $Tag
    $parts = $versionString -split '-', 2
    $tagOnly = $parts[0]
    $suffix = $parts.Count -gt 1 ? $parts[1] : ''
    return @{ Tag = $tagOnly; Suffix = $suffix }
}

# function Get-OrCreateTag {
#     [CmdletBinding()]
#     param (
#         [Parameter()][string]$Tag
#     )

#     # If tag is provided, check if it exists, create if not
#     if (-Not [string]::IsNullOrEmpty($Tag)) {
#         # Check if tag already exists
#         $existingTags = git tag -l $Tag
#         if ($existingTags) {
#             Write-Host "Tag $Tag already exists."
#         } else {
#             Write-Host "Creating new tag $Tag..."
#             # Create and Push tags to remote
#             git tag -a $Tag -m "Release tag" -s
#             git push --tags
#         }
#         return $Tag
#     } else {
#         # No tag provided, get the latest tag
#         $latestTag = git describe --tags --abbrev=0
#         Write-Host "Using latest tag: $latestTag"
#         return $latestTag
#     }
# }

function New-GitHubReleaseIfNotExists {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)][string]$Tag
    )

    # This code will create the tag in default branch if it does not exists

    # Generate release notes
    # Extract owner/repo from git remote URL (handles HTTPS and SSH, strips .git)
    # if($(git remote get-url origin) -match '[:/]([^/:]+)/([^/]+?)(\.git)?$') { $reponame = "$($matches[1])/$($matches[2])" } else {throw "Wrong  URL: $remoteUrl" }
    # Generate release notes using GitHub api
    # gh api repos/$reponame/releases/generate-notes -f tag_name="$tag" -q .body | Out-File -FilePath "CHANGELOG.md" -Encoding utf8

    $tagInfo = Get-TagAndSuffix -Tag $Tag
    $Prerelease = ($tagInfo.Suffix) ? "--prerelease" : $null

    $releaselist = gh release list --json tagName | ConvertFrom-Json
    if ($releaselist.tagName -contains $Tag) {
        Write-Host "Release $Tag already exists, skipping creation."
    }
    else {
        Write-Host "Creating release $Tag..."
        gh release create $Tag $Prerelease --generate-notes --latest=true
    }

    # Check if the release was created successfully
    Write-Host "Checking if release $Tag successfully created ..."
    $retTag = gh release view $Tag --json tagName | ConvertFrom-Json | Select-Object -ExpandProperty tagName

    return $retTag -eq $Tag
}

function Update-CsprojVersionProperties {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)][string]$Tag,
        [Parameter()][string]$CsProjPath
    )
    
    $tagInfo = Get-TagAndSuffix -Tag $Tag
    
    if (-not $csprojPath) {
        Write-Error "No .csproj file found in the current directory."
        return $false
    }
    
    try {
        $projectContent = [xml](Get-Content -Path $csprojPath)

        if (-Not $projectContent.Project.PropertyGroup.Version) {
            # Create a new PropertyGroup if it doesn't exist
            $propertyGroup = $projectContent.Project.PropertyGroup[0]
            if (-not $propertyGroup) {
                $propertyGroup = $projectContent.CreateElement("PropertyGroup")
                $projectContent.Project.AppendChild($propertyGroup)
            }

            # Create the version elements if they don't exist
            $versionProperties = @('Version', 'VersionSuffix', 'AssemblyVersion', 'FileVersion', 'PackageVersion')
            foreach ($propName in $versionProperties) {
                $propElement = $projectContent.CreateElement($propName)
                $propElement.InnerText = $propName -eq 'VersionSuffix' ? 'dev' : '0.1.0'
                $propertyGroup.AppendChild($propElement)
            }
        }

        # Update version properties
        $propertyGroup = $projectContent.Project.PropertyGroup | Where-Object { $_.Version }
        if (-not $propertyGroup) {
            Write-Error "No PropertyGroup with Version found in the .csproj file."
            return $false
        }
        $propertyGroup.version = $tagInfo.Tag
        $propertyGroup.versionSuffix = $tagInfo.Suffix
        $propertyGroup.AssemblyVersion = $tagInfo.Tag
        $propertyGroup.FileVersion = $tagInfo.Tag
        $propertyGroup.PackageVersion = $tagInfo.Tag

        $projectContent.Save($csprojPath)
        Write-Host "Updated project properties to version $($tagInfo.Tag)$(if ($tagInfo.Suffix) { "-$($tagInfo.Suffix)" })"
        return $true
    }
    catch {
        Write-Error "Failed to update project properties: $_"
        return $false
    }
}

function Get-Runtimes {
    [CmdletBinding()]
    param(
        [switch]$Test
    )

    if ($Test) {
        Write-Host "Running in test mode, returning test runtimes."
        return @{
            "osx-arm64" = "darwin-arm64"
        }
    }
    $ret = @{
        "osx-x64"     = "darwin-amd64"
        "osx-arm64"   = "darwin-arm64"
        # "linux-x86" = "linux-386"   # <-- No longer supported in .NET 6+ or higher
        "linux-arm"   = "linux-arm"
        "linux-x64"   = "linux-amd64"
        "linux-arm64" = "linux-arm64"
        "win-x86"     = "windows-386"
        "win-x64"     = "windows-amd64"
    }

    return $ret
}

function Initialize-Folder([string]$Path, [switch]$Force) {
    # Create new folder if it does not exists
    # if exists and Force is set, recreate it
    if (-not (Test-Path -Path $Path)) {
        Write-Host "Creating directory at $Path"
        New-Item -ItemType Directory -Path $Path
    } elseif ($Force) {
        Write-Host "Recreating existing directory at $Path"
        Remove-Item -Path $Path -Recurse -Force
        $null = New-Item -ItemType Directory -Path $Path 
    } else {
        Write-Host "Directory already exists at $Path, skipping creation."
    }
}

###################################################

function Invoke-BuildAndRelease {

    [CmdletBinding(SupportsShouldProcess)]
    param (
        [Parameter()][string]$RootFolder = ".",
        [Parameter()][string]$Tag,
        [Parameter()][switch]$Force,
        [Parameter()][switch]$SkipBranchCheck
    )

    # Stop on errors
    $ErrorActionPreference = "Stop"

    # Check if we're on the main branch
    # Get current branch
    $currentBranch = git rev-parse --abbrev-ref HEAD

    if ($currentBranch -ne "main" -and -not $SkipBranchCheck) {
        Write-Error "This script must be run from the default branch ($defaultBranch). Current branch: $currentBranch"
        return
    }

    Write-Host "Running on $currentBranch branch, continuing with release process..."

    # Replace direct $runtimes assignment with function call
    $runtimes = Get-Runtimes -Test

    # Create dist and uploads directories if they don't exist
    $csprojPath = $RootFolder | Join-Path -ChildPath "gh-kk" -AdditionalChildPath "gh-kk.csproj"
    $distDir = $RootFolder | Join-Path -ChildPath "dist"
    $uploads = $RootFolder | Join-Path -ChildPath "uploads"

    # initialize dist and uploads directories
    Initialize-Folder -Path $distDir
    Initialize-Folder -Path $uploads -Force:$Force

    # Get or create the tag
    # $Tag = Get-OrCreateTag -Tag $Tag

    # Create release if it does not exist
    if (-not(New-GitHubReleaseIfNotExists -Tag $Tag)) {
        Write-Error "Failed to find or create release for tag $Tag."
    }

    # Update the project properties with the tag information
    if (-not (Update-CsprojVersionProperties -Tag $Tag -CsProjPath $csprojPath)) {
        Write-Error "Failed to update project properties. Exiting."
        return
    }

    # Build for each runtime
    foreach ($runtime in $runtimes.Keys) {
        Write-Host "Building for $runtime..."
    
        # Set extension based on whether it's Windows
        $extension = ""
        if ($runtime.StartsWith("win-")) {
            $extension = ".exe"
        }
    
        $binaryName = "gh-kk$extension"
        $binaryPath = "$distDir/$runtime/$binaryName"

        if (($binaryPath | Test-Path) -and (-not $Force)) {
            Write-Host "Binary already exists at $binaryPath. Use -Force to rebuild."
            continue
        }

        # Publish for the specific runtime
        dotnet publish $csprojPath --configuration Release --runtime $runtime --ucr /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --output "$distDir/$runtime"

        # Find binary file (ignore .pdb files) and copy to uploads with Go platform name
        if (Test-Path -Path $binaryPath) {
            $goPlatform = $runtimes[$runtime]
            $goFileName = "gh-kk-$goPlatform$extension"
            Copy-Item -Path $binaryPath -Destination "$uploads/$goFileName"
            Write-Host "Copied binary to $uploads/$goFileName"
        }
        else {
            Write-Warning "Binary file not found at $binaryPath"
        }
    }

    # restore the csproj file to avoid committing version changes
    Write-Host "Restoring $csprojPath to avoid committing version changes..."
    git restore $csprojPath

    # Check if there are any files in dist and uploads directories
    $uploadFiles = Get-ChildItem -Path $uploads

    # Generate checksums if GPG_FINGERPRINT is set
    $checksumFileName = "checksums.txt"
    $checksumFile = $uploads | Join-Path -ChildPath $checksumFileName
    foreach ($file in (Get-ChildItem -Path $uploadFiles)) {
        Get-FileHash -Path $file.FullName -Algorithm SHA256 | ForEach-Object { "$($_.Hash.ToLower())  $($file.Name)" } | Out-File -Append -FilePath $checksumFile -Encoding utf8
    }

    # Sign checksums with GPG
    # gpg --output checksums.txt.sig --detach-sign checksums.txt


    # Upload release assets to release
    foreach ($file in (Get-ChildItem -Path $uploads)) {
        if ($PSCmdlet.ShouldProcess($file.Name, "Uploading to release")) {
            Write-Host "Uploading $($file.Name) to release..."
            gh release upload $tag $file.FullName --clobber
        } 
    }

}