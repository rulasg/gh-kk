[CmdletBinding()]
param (
    [Parameter(Position = 0)][string]$Path,
    [parameter(Position=1)][string]$OutputPath = '.',
    [Parameter()][ValidateSet('osx-x64', 'osx-arm64', 'linux-x86', 'linux-arm', 'linux-x64', 'linux-arm64', 'win-x86', 'win-x64')]
    [string]$Runtime = 'osx-arm64'
)

$Runtimes = @('osx-x64', 'osx-arm64', 'linux-x86', 'linux-arm', 'linux-x64', 'linux-arm64', 'win-x86', 'win-x64')

# dotnet publish $Path --configuration Release  --ucr  /p:PublishSingleFile=true  /p:IncludeNativeLibrariesForSelfExtract=true  --output $OutputPath  
dotnet publish $Path --configuration Release --ucr /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --output $OutputPath