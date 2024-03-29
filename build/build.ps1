# -------------------------------------------------------------------------------------------------
# COMMON FUNCTIONS
# -------------------------------------------------------------------------------------------------
function Print {
    param (
        [string]$Category,
        [string]$Message
    )

    if ($Category) {
        Write-Host "[$Category] $Message" -ForegroundColor Green
    } else {
        Write-Host "$Message" -ForegroundColor Green
    }
}

function AssertLastExitCode {
    if ($LASTEXITCODE -ne 0) {
        exit 1
    }
}

# -------------------------------------------------------------------------------------------------
# LOGO
# -------------------------------------------------------------------------------------------------
$logo = (Invoke-WebRequest "https://raw.githubusercontent.com/FantasticFiasco/logo/main/logo.raw").toString();
Print -Message $logo

# -------------------------------------------------------------------------------------------------
# VARIABLES
# -------------------------------------------------------------------------------------------------
$git_sha = "$env:APPVEYOR_REPO_COMMIT".TrimStart("0").substring(0, 7)
$is_tagged_build = If ("$env:APPVEYOR_REPO_TAG" -eq "true") { $true } Else { $false }
$is_pull_request = If ("$env:APPVEYOR_PULL_REQUEST_NUMBER" -eq "") { $false } Else { $true }
Print "info" "git sha: $git_sha"
Print "info" "is git tag: $is_tagged_build"
Print "info" "is pull request: $is_pull_request"

# -------------------------------------------------------------------------------------------------
# BUILD
# -------------------------------------------------------------------------------------------------
Print "build" "build started"
Print "build" "dotnet cli v$(dotnet --version)"

[xml]$build_props = Get-Content -Path .\Directory.Build.props
$version_prefix = $build_props.Project.PropertyGroup.VersionPrefix
Print "info" "build props version prefix: $version_prefix"
$version_suffix = $build_props.Project.PropertyGroup.VersionSuffix
Print "info" "build props version suffix: $version_suffix"

if ($is_tagged_build) {
    Print "build" "build"
    dotnet build -c Release
    AssertLastExitCode

    Print "build" "pack"
    New-Item -ItemType Directory -Path .\artifacts
    dotnet pack -c Release --no-build
    AssertLastExitCode
    Move-Item -Path .\src\Serilog.Sinks.Udp\bin\Release\*.nupkg -Destination .\artifacts
    Move-Item -Path .\src\Serilog.Sinks.Udp\bin\Release\*.snupkg -Destination .\artifacts
} else {
    # Use git tag if version suffix isn't specified
    if ($version_suffix -eq "") {
        $version_suffix = $git_sha
    }

    Print "build" "build"
    dotnet build -c Release --version-suffix=$version_suffix
    AssertLastExitCode

    Print "build" "pack"
    New-Item -ItemType Directory -Path .\artifacts
    dotnet pack -c Release --version-suffix=$version_suffix --no-build
    AssertLastExitCode
    Move-Item -Path .\src\Serilog.Sinks.Udp\bin\Release\*.nupkg -Destination .\artifacts
    Move-Item -Path .\src\Serilog.Sinks.Udp\bin\Release\*.snupkg -Destination .\artifacts
}

# -------------------------------------------------------------------------------------------------
# TEST
# -------------------------------------------------------------------------------------------------
Print "test" "test started"

# Test
foreach ($test in Get-ChildItem test/*Tests)
{
    Push-Location $test

    Print "test" "testing project in $test"

    & dotnet test -c Release
    AssertLastExitCode

    Pop-Location
}
