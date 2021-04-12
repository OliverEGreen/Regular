properties {
  $zipFileName = "Json120r3.zip"
  $majorVersion = "12.0"
  $majorWithReleaseVersion = "12.0.3"
  $nugetPrerelease = $null
  $version = GetVersion $majorWithReleaseVersion
  $packageId = "Newtonsoft.Json"
  $signAssemblies = $false
  $signKeyPath = "C:\Development\Releases\newtonsoft.snk"
  $buildDocumentation = $false
  $buildNuGet = $true
  $msbuildVerbosity = 'minimal'
  $treatWarningsAsErrors = $false
  $workingName = if ($workingName) {$workingName} else {"Working"}
  $assemblyVersion = if ($assemblyVersion) {$assemblyVersion} else {$majorVersion + '.0.0'}
  $netCliChannel = "Current"
  $netCliVersion = "3.0.100"
  $nugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

  $baseDir  = resolve-path ..
  $buildDir = "$baseDir\Build"
  $sourceDir = "$baseDir\Src"
  $docDir = "$baseDir\Doc"
  $releaseDir = "$baseDir\Release"
  $workingDir = "$baseDir\$workingName"

  $nugetPath = "$buildDir\Temp\nuget.exe"
  $vswhereVersion = "2.3.2"
  $vswherePath = "$buildDir\Temp\vswhere.$vswhereVersion"
  $nunitConsoleVersion = "3.8.0"
  $nunitConsolePath = "$buildDir\Temp\NUnit.ConsoleRunner.$nunitConsoleVersion"

  $builds = @(
    @{Framework = "netstandard2.0"; TestsFunction = "NetCliTests"; TestFramework = "netcoreapp3.0"; Enabled=$true},
    @{Framework = "netstandard1.3"; TestsFunction = "NetCliTests"; TestFramework = "netcoreapp2.2"; Enabled=$true},
    @{Framework = "netstandard1.0"; TestsFunction = "NetCliTests"; TestFramework = "netcoreapp2.1"; Enabled=$true},
    @{Framework = "net45"; TestsFunction = "NUnitTests"; TestFramework = "net46"; NUnitFramework="net-4.0"; Enabled=$true},
    @{Framework = "net40"; TestsFunction = "NUnitTests"; NUnitFramework="net-4.0"; Enabled=$true},
    @{Framework = "net35"; TestsFunction = "NUnitTests"; NUnitFramework="net-2.0"; Enabled=$true},
    @{Framework = "net20"; TestsFunction = "NUnitTests"; NUnitFramework="net-2.0"; Enabled=$true},
    @{Framework = "portable-net45+win8+wpa81+wp8"; TestsFunction = "NUnitTests"; TestFramework = "net452"; NUnitFramework="net-4.0"; Enabled=$true},
    @{Framework = "portable-net40+win8+wpa81+wp8+sl5"; TestsFunction = "NUnitTests"; TestFramework = "net451"; NUnitFramework="net-4.0"; Enabled=$true}
  )
}

framework '4.6x86'

task default -depends Test,Package

# Ensure a clean working directory
task Clean {
  Write-Host "Setting location to $baseDir"
  Set-Location $baseDir

  if (Test-Path -path $workingDir)
  {
    Write-Host "Deleting existing working directory $workingDir"

    Execute-Command -command { del $workingDir -Recurse -Force }
  }

  Write-Host "Creating working directory $workingDir"
  New-Item -Path $workingDir -ItemType Directory

}

# Build each solution, optionally signed
task Build -depends Clean {
  $script:enabledBuilds = $builds | ? {$_.Enabled}
  Write-Host -ForegroundColor Green "Found $($script:enabledBuilds.Length) enabled builds"

  mkdir "$buildDir\Temp" -Force
  
  EnsureDotNetCli
  EnsureNuGetExists
  EnsureNuGetPackage "vswhere" $vswherePath $vswhereVersion
  EnsureNuGetPackage "NUnit.ConsoleRunner" $nunitConsolePath $nunitConsoleVersion

  $script:msBuildPath = GetMsBuildPath
  Write-Host "MSBuild path $script:msBuildPath"

  NetCliBuild
}

# Optional build documentation, add files to final zip
task Package -depends Build {
  foreach ($build in $script:enabledBuilds)
  {
    $finalDir = $build.Framework

    $sourcePath = "$sourceDir\Newtonsoft.Json\bin\Release\$finalDir"

    if (!(Test-Path -path $sourcePath))
    {
      throw "Could not find $sourcePath"
    }

    robocopy $sourcePath $workingDir\Package\Bin\$finalDir *.dll *.pdb *.xml /NFL /NDL /NJS /NC /NS /NP /XO /XF *.CodeAnalysisLog.xml | Out-Default
  }

  if ($buildNuGet)
  {
    Write-Host -ForegroundColor Green "Copy NuGet package"

    mkdir $workingDir\NuGet
    move -Path $sourceDir\Newtonsoft.Json\bin\Release\*.nupkg -Destination $workingDir\NuGet
    move -Path $sourceDir\Newtonsoft.Json\bin\Release\*.snupkg -Destination $workingDir\NuGet
  }

  Write-Host "Build documentation: $buildDocumentation"

  if ($buildDocumentation)
  {
    $mainBuild = $script:enabledBuilds | where { $_.Framework -eq "net45" } | select -first 1
    $mainBuildFinalDir = $mainBuild.Framework
    $documentationSourcePath = "$workingDir\Package\Bin\$mainBuildFinalDir"
    $docOutputPath = "$workingDir\Documentation\"
    Write-Host -ForegroundColor Green "Building documentation from $documentationSourcePath"
    Write-Host "Documentation output to $docOutputPath"

    # Sandcastle has issues when compiling with .NET 4 MSBuild
    exec { & $script:msBuildPath "/t:Clean;Rebuild" "/v:$msbuildVerbosity" "/p:Configuration=Release" "/p:DocumentationSourcePath=$documentationSourcePath" "/p:OutputPath=$docOutputPath" "/m" "$docDir\doc.shfbproj" | Out-Default } "Error building documentation. Check that you have Sandcastle, Sandcastle Help File Builder and HTML Help Workshop installed."

    move -Path $workingDir\Documentation\LastBuild.log -Destination $workingDir\Documentation.log
  }

  Copy-Item -Path $docDir\readme.txt -Destination $workingDir\Package\
  Copy-Item -Path $docDir\license.txt -Destination $workingDir\Package\

  robocopy $sourceDir $workingDir\Package\Source\Src /MIR /NFL /NDL /NJS /NC /NS /NP /XD bin obj TestResults AppPackages .vs artifacts /XF *.suo *.user *.lock.json | Out-Default
  robocopy $buildDir $workingDir\Package\Source\Build /MIR /NFL /NDL /NJS /NC /NS /NP /XD Temp /XF runbuild.txt | Out-Default
  robocopy $docDir $workingDir\Package\Source\Doc /MIR /NFL /NDL /NJS /NC /NS /NP | Out-Default

  Compress-Archive -Path $workingDir\Package\* -DestinationPath $workingDir\$zipFileName
}

task Test -depends Build {
  foreach ($build in $script:enabledBuilds)
  {
    Write-Host "Calling $($build.TestsFunction)"
    & $build.TestsFunction $build
  }
}

function NetCliBuild()
{
  $projectPath = "$sourceDir\Newtonsoft.Json.sln"
  $libraryFrameworks = ($script:enabledBuilds | Select-Object @{Name="Framework";Expression={$_.Framework}} | select -expand Framework) -join ";"
  $testFrameworks = ($script:enabledBuilds | Select-Object @{Name="Resolved";Expression={if ($_.TestFramework -ne $null) { $_.TestFramework } else { $_.Framework }}} | select -expand Resolved) -join ";"

  $additionalConstants = switch($signAssemblies) { $true { "SIGNED" } default { "" } }

  Write-Host -ForegroundColor Green "Restoring packages for $libraryFrameworks in $projectPath"
  Write-Host

  exec { & $script:msBuildPath "/t:restore" "/v:$msbuildVerbosity" "/p:Configuration=Release" "/p:LibraryFrameworks=`"$libraryFrameworks`"" "/p:TestFrameworks=`"$testFrameworks`"" "/m" $projectPath | Out-Default } "Error restoring $projectPath"

  Write-Host -ForegroundColor Green "Building $libraryFrameworks $assemblyVersion in $projectPath"
  Write-Host

  exec { & $script:msBuildPath "/t:build" "/v:$msbuildVerbosity" $projectPath "/p:Configuration=Release" "/p:LibraryFrameworks=`"$libraryFrameworks`"" "/p:TestFrameworks=`"$testFrameworks`"" "/p:AssemblyOriginatorKeyFile=$signKeyPath" "/p:SignAssembly=$signAssemblies" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "/p:AdditionalConstants=$additionalConstants" "/p:GeneratePackageOnBuild=$buildNuGet" "/p:ContinuousIntegrationBuild=true" "/p:PackageId=$packageId" "/p:VersionPrefix=$majorWithReleaseVersion" "/p:VersionSuffix=$nugetPrerelease" "/p:AssemblyVersion=$assemblyVersion" "/p:FileVersion=$version" "/m" }
}

function EnsureDotnetCli()
{
  Write-Host "Downloading dotnet-install.ps1"

  # https://stackoverflow.com/questions/36265534/invoke-webrequest-ssl-fails
  [Net.ServicePointManager]::SecurityProtocol = 'TLS12'
  Invoke-WebRequest `
    -Uri "https://dot.net/v1/dotnet-install.ps1" `
    -OutFile "$buildDir\Temp\dotnet-install.ps1"

  exec { & $buildDir\Temp\dotnet-install.ps1 -Channel $netCliChannel -Version $netCliVersion | Out-Default }
}

function EnsureNuGetExists()
{
  if (!(Test-Path $nugetPath))
  {
    Write-Host "Couldn't find nuget.exe. Downloading from $nugetUrl to $nugetPath"
    (New-Object System.Net.WebClient).DownloadFile($nugetUrl, $nugetPath)
  }
}

function EnsureNuGetPackage($packageName, $packagePath, $packageVersion)
{
  if (!(Test-Path $packagePath))
  {
    Write-Host "Couldn't find $packagePath. Downloading with NuGet"
    exec { & $nugetPath install $packageName -OutputDirectory $buildDir\Temp -Version $packageVersion -ConfigFile "$sourceDir\nuget.config" | Out-Default } "Error restoring $packagePath"
  }
}

function GetMsBuildPath()
{
  $path = & $vswherePath\tools\vswhere.exe -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
  if (!($path))
  {
    throw "Could not find Visual Studio install path"
  }

  $msBuildPath = join-path $path 'MSBuild\15.0\Bin\MSBuild.exe'
  if (Test-Path $msBuildPath)
  {
    return $msBuildPath
  }

  $msBuildPath = join-path $path 'MSBuild\Current\Bin\MSBuild.exe'
  if (Test-Path $msBuildPath)
  {
    return $msBuildPath
  }

  throw "Could not find MSBuild path"
}

function NetCliTests($build)
{
  $projectPath = "$sourceDir\Newtonsoft.Json.Tests\Newtonsoft.Json.Tests.csproj"
  $location = "$sourceDir\Newtonsoft.Json.Tests"
  $testDir = if ($build.TestFramework -ne $null) { $build.TestFramework } else { $build.Framework }

  try
  {
    Set-Location $location

    exec { dotnet --version | Out-Default }

    Write-Host -ForegroundColor Green "Running tests for $testDir"
    Write-Host "Location: $location"
    Write-Host "Project path: $projectPath"
    Write-Host

    exec { dotnet test $projectPath -f $testDir -c Release -l trx -r $workingDir --no-restore --no-build | Out-Default }
  }
  finally
  {
    Set-Location $baseDir
  }
}

function NUnitTests($build)
{
  $testDir = if ($build.TestFramework -ne $null) { $build.TestFramework } else { $build.Framework }
  $framework = $build.NUnitFramework
  $testRunDir = "$sourceDir\Newtonsoft.Json.Tests\bin\Release\$testDir"

  Write-Host -ForegroundColor Green "Running NUnit tests $testDir"
  Write-Host
  try
  {
    Set-Location $testRunDir
    exec { & $nunitConsolePath\tools\nunit3-console.exe "$testRunDir\Newtonsoft.Json.Tests.dll" --framework=$framework --result=$workingDir\$testDir.xml --out=$workingDir\$testDir.txt | Out-Default } "Error running $testDir tests"
  }
  finally
  {
    Set-Location $baseDir
  }
}

function GetVersion($majorVersion)
{
    $now = [DateTime]::Now

    $year = $now.Year - 2000
    $month = $now.Month
    $totalMonthsSince2000 = ($year * 12) + $month
    $day = $now.Day
    $minor = "{0}{1:00}" -f $totalMonthsSince2000, $day

    $hour = $now.Hour
    $minute = $now.Minute
    $revision = "{0:00}{1:00}" -f $hour, $minute

    return $majorVersion + "." + $minor
}

function Edit-XmlNodes {
    param (
        [xml] $doc,
        [string] $xpath = $(throw "xpath is a required parameter"),
        [string] $value = $(throw "value is a required parameter")
    )

    $nodes = $doc.SelectNodes($xpath)
    $count = $nodes.Count

    Write-Host "Found $count nodes with path '$xpath'"

    foreach ($node in $nodes) {
        if ($node -ne $null) {
            if ($node.NodeType -eq "Element")
            {
                $node.InnerXml = $value
            }
            else
            {
                $node.Value = $value
            }
        }
    }
}

function Execute-Command($command) {
    $currentRetry = 0
    $success = $false
    do {
        try
        {
            & $command
            $success = $true
        }
        catch [System.Exception]
        {
            if ($currentRetry -gt 5) {
                throw $_.Exception.ToString()
            } else {
                write-host "Retry $currentRetry"
                Start-Sleep -s 1
            }
            $currentRetry = $currentRetry + 1
        }
    } while (!$success)
}
# SIG # Begin signature block
# MIIfugYJKoZIhvcNAQcCoIIfqzCCH6cCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCAFOLflRSfDqJqm
# p+/RxrEXXyRYuk3c3tqy45V478mreaCCDfMwggPFMIICraADAgECAhACrFwmagtA
# m48LefKuRiV3MA0GCSqGSIb3DQEBBQUAMGwxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xKzApBgNV
# BAMTIkRpZ2lDZXJ0IEhpZ2ggQXNzdXJhbmNlIEVWIFJvb3QgQ0EwHhcNMDYxMTEw
# MDAwMDAwWhcNMzExMTEwMDAwMDAwWjBsMQswCQYDVQQGEwJVUzEVMBMGA1UEChMM
# RGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSswKQYDVQQD
# EyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5jZSBFViBSb290IENBMIIBIjANBgkqhkiG
# 9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxszlc+b71LvlLS0ypt/lgT/JzSVJtnEqw9WU
# NGeiChywX2mmQLHEt7KP0JikqUFZOtPclNY823Q4pErMTSWC90qlUxI47vNJbXGR
# fmO2q6Zfw6SE+E9iUb74xezbOJLjBuUIkQzEKEFV+8taiRV+ceg1v01yCT2+OjhQ
# W3cxG42zxyRFmqesbQAUWgS3uhPrUQqYQUEiTmVhh4FBUKZ5XIneGUpX1S7mXRxT
# LH6YzRoGFqRoc9A0BBNcoXHTWnxV215k4TeHMFYE5RG0KYAS8Xk5iKICEXwnZreI
# t3jyygqoOKsKZMK/Zl2VhMGhJR6HXRpQCyASzEG7bgtROLhLywIDAQABo2MwYTAO
# BgNVHQ8BAf8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQUsT7DaQP4
# v0cB1JgmGggC72NkK8MwHwYDVR0jBBgwFoAUsT7DaQP4v0cB1JgmGggC72NkK8Mw
# DQYJKoZIhvcNAQEFBQADggEBABwaBpfc15yfPIhmBghXIdshR/gqZ6q/GDJ2QBBX
# wYrzetkRZY41+p78RbWe2UwxS7iR6EMsjrN4ztvjU3lx1uUhlAHaVYeaJGT2imbM
# 3pw3zag0sWmbI8ieeCIrcEPjVUcxYRnvWMWFL04w9qAxFiPI5+JlFjPLvxoboD34
# yl6LMYtgCIktDAZcUrfE+QqY0RVfnxK+fDZjOL1EpH/kJisKxJdpDemM4sAQV7jI
# dhKRVfJIadi8KgJbD0TUIDHb9LpwJl2QYJ68SxcJL7TLHkNoyQcnwdJc9+ohuWgS
# nDycv578gFybY83sR6olJ2egN/MAgn1U16n46S4To3foH0owggSRMIIDeaADAgEC
# AhAHsEGNpR4UjDMbvN63E4MjMA0GCSqGSIb3DQEBCwUAMGwxCzAJBgNVBAYTAlVT
# MRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5j
# b20xKzApBgNVBAMTIkRpZ2lDZXJ0IEhpZ2ggQXNzdXJhbmNlIEVWIFJvb3QgQ0Ew
# HhcNMTgwNDI3MTI0MTU5WhcNMjgwNDI3MTI0MTU5WjBaMQswCQYDVQQGEwJVUzEY
# MBYGA1UEChMPLk5FVCBGb3VuZGF0aW9uMTEwLwYDVQQDEyguTkVUIEZvdW5kYXRp
# b24gUHJvamVjdHMgQ29kZSBTaWduaW5nIENBMIIBIjANBgkqhkiG9w0BAQEFAAOC
# AQ8AMIIBCgKCAQEAwQqv4aI0CI20XeYqTTZmyoxsSQgcCBGQnXnufbuDLhAB6GoT
# NB7HuEhNSS8ftV+6yq8GztBzYAJ0lALdBjWypMfL451/84AO5ZiZB3V7MB2uxgWo
# cV1ekDduU9bm1Q48jmR4SVkLItC+oQO/FIA2SBudVZUvYKeCJS5Ri9ibV7La4oo7
# BJChFiP8uR+v3OU33dgm5BBhWmth4oTyq22zCfP3NO6gBWEIPFR5S+KcefUTYmn2
# o7IvhvxzJsMCrNH1bxhwOyMl+DQcdWiVPuJBKDOO/hAKIxBG4i6ryQYBaKdhDgaA
# NSCik0UgZasz8Qgl8n0A73+dISPumD8L/4mdywIDAQABo4IBPzCCATswHQYDVR0O
# BBYEFMtck66Im/5Db1ZQUgJtePys4bFaMB8GA1UdIwQYMBaAFLE+w2kD+L9HAdSY
# JhoIAu9jZCvDMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAKBggrBgEFBQcDAzAS
# BgNVHRMBAf8ECDAGAQH/AgEAMDQGCCsGAQUFBwEBBCgwJjAkBggrBgEFBQcwAYYY
# aHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEsGA1UdHwREMEIwQKA+oDyGOmh0dHA6
# Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEhpZ2hBc3N1cmFuY2VFVlJvb3RD
# QS5jcmwwPQYDVR0gBDYwNDAyBgRVHSAAMCowKAYIKwYBBQUHAgEWHGh0dHBzOi8v
# d3d3LmRpZ2ljZXJ0LmNvbS9DUFMwDQYJKoZIhvcNAQELBQADggEBALNGxKTz6gq6
# clMF01GjC3RmJ/ZAoK1V7rwkqOkY3JDl++v1F4KrFWEzS8MbZsI/p4W31Eketazo
# Nxy23RT0zDsvJrwEC3R+/MRdkB7aTecsYmMeMHgtUrl3xEO3FubnQ0kKEU/HBCTd
# hR14GsQEccQQE6grFVlglrew+FzehWUu3SUQEp9t+iWpX/KfviDWx0H1azilMX15
# lzJUxK7kCzmflrk5jCOCjKqhOdGJoQqstmwP+07qXO18bcCzEC908P+TYkh0z9gV
# rlj7tyW9K9zPVPJZsLRaBp/QjMcH65o9Y1hD1uWtFQYmbEYkT1K9tuXHtQYx1Rpf
# /dC8Nbl4iukwggWRMIIEeaADAgECAhAKcaGwwpb1x5BlRwo8IFN+MA0GCSqGSIb3
# DQEBCwUAMFoxCzAJBgNVBAYTAlVTMRgwFgYDVQQKEw8uTkVUIEZvdW5kYXRpb24x
# MTAvBgNVBAMTKC5ORVQgRm91bmRhdGlvbiBQcm9qZWN0cyBDb2RlIFNpZ25pbmcg
# Q0EwHhcNMTgxMDI1MDAwMDAwWhcNMjExMDI5MTIwMDAwWjCBjDEUMBIGA1UEBRML
# NjAzIDM4OSAwNjgxCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJ3YTEQMA4GA1UEBxMH
# UmVkbW9uZDEjMCEGA1UEChMaSnNvbi5ORVQgKC5ORVQgRm91bmRhdGlvbikxIzAh
# BgNVBAMTGkpzb24uTkVUICguTkVUIEZvdW5kYXRpb24pMIIBIjANBgkqhkiG9w0B
# AQEFAAOCAQ8AMIIBCgKCAQEAuN7uHXlw69xJiDtuvtaEmG2+W4tcwqucLUj7xf+D
# W5mTiqIa4uSUZ1upBEQmpNnjHV6iKvMzDLjMsjNEhgMnkoL+6HBvhy5GOyKpG/T3
# NHxX6bUN3JTExwZUD/CpXHIh31n3KSthNRlXyZod+5hWuC93UJxzNcXtR7iYR39x
# 5oIx4IEdq0xyXt9PclKu/V6R9D7Fjllx9aKlTg/hnPoMKF35P7kxMv5ULagSv3HO
# JkKa70biuqv8625wBE9HNA9S5LXNFMk2ZuUhYTxLk1qYx58me+NU6VUvoDsqzLx+
# GXTAkl6r1kSeDMskOe4fXevoCkPm1BFZHd68MmH6tIuMLQIDAQABo4ICHjCCAhow
# HwYDVR0jBBgwFoAUy1yTroib/kNvVlBSAm14/KzhsVowHQYDVR0OBBYEFF4wRcl3
# CrbuKXpbC4oTKXnW5tsmMDQGA1UdEQQtMCugKQYIKwYBBQUHCAOgHTAbDBlVUy1X
# QVNISU5HVE9OLTYwMyAzODkgMDY4MA4GA1UdDwEB/wQEAwIHgDATBgNVHSUEDDAK
# BggrBgEFBQcDAzCBmQYDVR0fBIGRMIGOMEWgQ6BBhj9odHRwOi8vY3JsMy5kaWdp
# Y2VydC5jb20vTkVURm91bmRhdGlvblByb2plY3RzQ29kZVNpZ25pbmdDQS5jcmww
# RaBDoEGGP2h0dHA6Ly9jcmw0LmRpZ2ljZXJ0LmNvbS9ORVRGb3VuZGF0aW9uUHJv
# amVjdHNDb2RlU2lnbmluZ0NBLmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwDATAq
# MCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeB
# DAEEATCBhAYIKwYBBQUHAQEEeDB2MCQGCCsGAQUFBzABhhhodHRwOi8vb2NzcC5k
# aWdpY2VydC5jb20wTgYIKwYBBQUHMAKGQmh0dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0
# LmNvbS9ORVRGb3VuZGF0aW9uUHJvamVjdHNDb2RlU2lnbmluZ0NBLmNydDAMBgNV
# HRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IBAQCgwZvcU0JuY6FZlCQFuHPKr2Ay
# dS1d+SAqK7gT3RCtdzzPJBt8VWhdaEMoHvAbsEdLFPdY9+iIL0LxdiQHUzvE6c37
# gvPpuCsM6ZzM/T4OmUEcKCadYQAssYJ5F2PdvZv+3Rlddp2RtBN3WmNEfbOCRFRB
# jvrR6Oxep8ZJyJAv4fyJrFVSNj/jS7IkyYLQJChC70Rh6UF7JChyEZaG15/NPUUB
# jIq8u5phqDvIGFyem1BV5oeoC6AA2wqcRwobgaq7v8GIaAcVA902B3aLEEKFH1HL
# z1yOs+AEcP+Sf3LXQPM4jITKLXQFhHVGUqSOwIW/RDWMl4gk1eEzLh2Nw4HyMYIR
# HTCCERkCAQEwbjBaMQswCQYDVQQGEwJVUzEYMBYGA1UEChMPLk5FVCBGb3VuZGF0
# aW9uMTEwLwYDVQQDEyguTkVUIEZvdW5kYXRpb24gUHJvamVjdHMgQ29kZSBTaWdu
# aW5nIENBAhAKcaGwwpb1x5BlRwo8IFN+MA0GCWCGSAFlAwQCAQUAoIG0MBkGCSqG
# SIb3DQEJAzEMBgorBgEEAYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3
# AgEVMC8GCSqGSIb3DQEJBDEiBCAN5uQ0SQoY5BXYOppuUAP380IoQULE3OKGaozn
# ffPxZzBIBgorBgEEAYI3AgEMMTowOKASgBAASgBzAG8AbgAuAE4ARQBUoSKAIGh0
# dHBzOi8vd3d3Lm5ld3RvbnNvZnQuY29tL2pzb24gMA0GCSqGSIb3DQEBAQUABIIB
# AIzuwg1bThrffE0wEU/TD2wn+a9kHt4acU01MNM76wA46TvgFQjbB1iVkktOD2Rv
# uusdVoM4Nyij/8S/bYOA1WoDfJz0KOw7Jz5ktCcSTbaXiLvYOhEdqmwWez2HfmpW
# tZiV4Cedni+mfFJsgoaWsk0UrHHrGOpSWefYv9KCuLMFzC+QX9WhBe9ah5kUlamn
# QM+LTC0DD9j0Nefnjl+gCfgUCrJ3Zo2fK2EFYX/3NHHhZTUj1lp0bK7xh8dIv5wR
# LalhVc+Vrma20HKNvcoJMmucso/QnpnB7AWte7VFlLUh+jz9/ZRs6skhkbqVkV0T
# v2ZpEa+WxJP8tAPZXtyVIL6hgg7JMIIOxQYKKwYBBAGCNwMDATGCDrUwgg6xBgkq
# hkiG9w0BBwKggg6iMIIOngIBAzEPMA0GCWCGSAFlAwQCAQUAMHgGCyqGSIb3DQEJ
# EAEEoGkEZzBlAgEBBglghkgBhv1sBwEwMTANBglghkgBZQMEAgEFAAQgyEw3aQzM
# iz5CnKdP5jYGRUS98RQ5FtDhBhonXyOyIfECEQDc1DcM8QLahpnMTcNJ+Au1GA8y
# MDE5MTEwOTAwNTY1OFqgggu7MIIGgjCCBWqgAwIBAgIQBM0/hWiudsYbsP5xYMyn
# bTANBgkqhkiG9w0BAQsFADByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNl
# cnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhEaWdp
# Q2VydCBTSEEyIEFzc3VyZWQgSUQgVGltZXN0YW1waW5nIENBMB4XDTE5MTAwMTAw
# MDAwMFoXDTMwMTAxNzAwMDAwMFowTDELMAkGA1UEBhMCVVMxFzAVBgNVBAoTDkRp
# Z2lDZXJ0LCBJbmMuMSQwIgYDVQQDExtUSU1FU1RBTVAtU0hBMjU2LTIwMTktMTAt
# MTUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDpZDWc+qmYZWQb5Bfc
# uCk2zGcJWIVNMODJ/+U7PBEoUK8HMeJdCRjC9omMaQgEI+B3LZ0V5bjooWqO/9Su
# 0noW7/hBtR05dcHPL6esRX6UbawDAZk8Yj5+ev1FlzG0+rfZQj6nVZvfWk9YAqgy
# aSITvouCLcaYq2ubtMnyZREMdA2y8AiWdMToskiioRSl+PrhiXBEO43v+6T0w7m9
# FCzrDCgnJYCrEEsWEmALaSKMTs3G1bJlWSHgfCwSjXAOj4rK4NPXszl3UNBCLC56
# zpxnejh3VED/T5UEINTryM6HFAj+HYDd0OcreOq/H3DG7kIWUzZFm1MZSWKdegKb
# lRSjAgMBAAGjggM4MIIDNDAOBgNVHQ8BAf8EBAMCB4AwDAYDVR0TAQH/BAIwADAW
# BgNVHSUBAf8EDDAKBggrBgEFBQcDCDCCAb8GA1UdIASCAbYwggGyMIIBoQYJYIZI
# AYb9bAcBMIIBkjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29t
# L0NQUzCCAWQGCCsGAQUFBwICMIIBVh6CAVIAQQBuAHkAIAB1AHMAZQAgAG8AZgAg
# AHQAaABpAHMAIABDAGUAcgB0AGkAZgBpAGMAYQB0AGUAIABjAG8AbgBzAHQAaQB0
# AHUAdABlAHMAIABhAGMAYwBlAHAAdABhAG4AYwBlACAAbwBmACAAdABoAGUAIABE
# AGkAZwBpAEMAZQByAHQAIABDAFAALwBDAFAAUwAgAGEAbgBkACAAdABoAGUAIABS
# AGUAbAB5AGkAbgBnACAAUABhAHIAdAB5ACAAQQBnAHIAZQBlAG0AZQBuAHQAIAB3
# AGgAaQBjAGgAIABsAGkAbQBpAHQAIABsAGkAYQBiAGkAbABpAHQAeQAgAGEAbgBk
# ACAAYQByAGUAIABpAG4AYwBvAHIAcABvAHIAYQB0AGUAZAAgAGgAZQByAGUAaQBu
# ACAAYgB5ACAAcgBlAGYAZQByAGUAbgBjAGUALjALBglghkgBhv1sAxUwHwYDVR0j
# BBgwFoAU9LbhIB3+Ka7S5GGlsqIlssgXNW4wHQYDVR0OBBYEFFZTD8HGB6dN19hu
# V3KAUEzk7J7BMHEGA1UdHwRqMGgwMqAwoC6GLGh0dHA6Ly9jcmwzLmRpZ2ljZXJ0
# LmNvbS9zaGEyLWFzc3VyZWQtdHMuY3JsMDKgMKAuhixodHRwOi8vY3JsNC5kaWdp
# Y2VydC5jb20vc2hhMi1hc3N1cmVkLXRzLmNybDCBhQYIKwYBBQUHAQEEeTB3MCQG
# CCsGAQUFBzABhhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wTwYIKwYBBQUHMAKG
# Q2h0dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFNIQTJBc3N1cmVk
# SURUaW1lc3RhbXBpbmdDQS5jcnQwDQYJKoZIhvcNAQELBQADggEBAC6DoUQFSgTj
# uTJS+tmB8Bq7+AmNI7k92JKh5kYcSi9uejxjbjcXoxq/WCOyQ5yUg045CbAs6Mfh
# 4szty3lrzt4jAUftlVSB4IB7ErGvAoapOnNq/vifwY3RIYzkKYLDigtgAAKdH0fE
# n7QKaFN/WhCm+CLm+FOSMV/YgoMtbRNCroPBEE6kJPRHnN4PInJ3XH9P6TmYK1eS
# RNfvbpPZQ8cEM2NRN1aeRwQRw6NYVCHY4o5W10k/V/wKnyNee/SUjd2dGrvfeiqm
# 0kWmVQyP9kyK8pbPiUbcMbKRkKNfMzBgVfX8azCsoe3kR04znmdqKLVNwu1bl4L4
# y6kIbFMJtPcwggUxMIIEGaADAgECAhAKoSXW1jIbfkHkBdo2l8IVMA0GCSqGSIb3
# DQEBCwUAMGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAX
# BgNVBAsTEHd3dy5kaWdpY2VydC5jb20xJDAiBgNVBAMTG0RpZ2lDZXJ0IEFzc3Vy
# ZWQgSUQgUm9vdCBDQTAeFw0xNjAxMDcxMjAwMDBaFw0zMTAxMDcxMjAwMDBaMHIx
# CzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3
# dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0IFNIQTIgQXNzdXJlZCBJ
# RCBUaW1lc3RhbXBpbmcgQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQC90DLuS82Pf92puoKZxTlUKFe2I0rEDgdFM1EQfdD5fU1ofue2oPSNs4jkl79j
# IZCYvxO8V9PD4X4I1moUADj3Lh477sym9jJZ/l9lP+Cb6+NGRwYaVX4LJ37AovWg
# 4N4iPw7/fpX786O6Ij4YrBHk8JkDbTuFfAnT7l3ImgtU46gJcWvgzyIQD3XPcXJO
# Cq3fQDpct1HhoXkUxk0kIzBdvOw8YGqsLwfM/fDqR9mIUF79Zm5WYScpiYRR5oLn
# RlD9lCosp+R1PrqYD4R/nzEU1q3V8mTLex4F0IQZchfxFwbvPc3WTe8GQv2iUypP
# hR3EHTyvz9qsEPXdrKzpVv+TAgMBAAGjggHOMIIByjAdBgNVHQ4EFgQU9LbhIB3+
# Ka7S5GGlsqIlssgXNW4wHwYDVR0jBBgwFoAUReuir/SSy4IxLVGLp6chnfNtyA8w
# EgYDVR0TAQH/BAgwBgEB/wIBADAOBgNVHQ8BAf8EBAMCAYYwEwYDVR0lBAwwCgYI
# KwYBBQUHAwgweQYIKwYBBQUHAQEEbTBrMCQGCCsGAQUFBzABhhhodHRwOi8vb2Nz
# cC5kaWdpY2VydC5jb20wQwYIKwYBBQUHMAKGN2h0dHA6Ly9jYWNlcnRzLmRpZ2lj
# ZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJRFJvb3RDQS5jcnQwgYEGA1UdHwR6MHgw
# OqA4oDaGNGh0dHA6Ly9jcmw0LmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJ
# RFJvb3RDQS5jcmwwOqA4oDaGNGh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9EaWdp
# Q2VydEFzc3VyZWRJRFJvb3RDQS5jcmwwUAYDVR0gBEkwRzA4BgpghkgBhv1sAAIE
# MCowKAYIKwYBBQUHAgEWHGh0dHBzOi8vd3d3LmRpZ2ljZXJ0LmNvbS9DUFMwCwYJ
# YIZIAYb9bAcBMA0GCSqGSIb3DQEBCwUAA4IBAQBxlRLpUYdWac3v3dp8qmN6s3jP
# BjdAhO9LhL/KzwMC/cWnww4gQiyvd/MrHwwhWiq3BTQdaq6Z+CeiZr8JqmDfdqQ6
# kw/4stHYfBli6F6CJR7Euhx7LCHi1lssFDVDBGiy23UC4HLHmNY8ZOUfSBAYX4k4
# YU1iRiSHY4yRUiyvKYnleB/WCxSlgNcSR3CzddWThZN+tpJn+1Nhiaj1a5bA9Fhp
# DXzIAbG5KHW3mWOFIoxhynmUfln8jA/jb7UBJrZspe6HUSHkWGCbugwtK22ixH67
# xCUrRwIIfEmuE7bhfEJCKMYYVs9BNLZmXbZ0e/VWMyIvIjayS6JKldj1po5SMYIC
# TTCCAkkCAQEwgYYwcjELMAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IElu
# YzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTExMC8GA1UEAxMoRGlnaUNlcnQg
# U0hBMiBBc3N1cmVkIElEIFRpbWVzdGFtcGluZyBDQQIQBM0/hWiudsYbsP5xYMyn
# bTANBglghkgBZQMEAgEFAKCBmDAaBgkqhkiG9w0BCQMxDQYLKoZIhvcNAQkQAQQw
# HAYJKoZIhvcNAQkFMQ8XDTE5MTEwOTAwNTY1OFowKwYLKoZIhvcNAQkQAgwxHDAa
# MBgwFgQUAyW9UF7aljAtwi9PoB5MKL4oNMUwLwYJKoZIhvcNAQkEMSIEIFN+JZtH
# 5wll6DaZoYc8dl5kYBaJLTCwtkn4s0Y8Xd0WMA0GCSqGSIb3DQEBAQUABIIBAM2P
# g89sUWxpkqckUI+U0Hhg1sv1U+WM//JSUDDxI08HcMAx5HDhk7vFqzSUCYAp5W9o
# 575CZuWkzsVxO4sbVDmbkuNlSGSb3X2hDacFRKPOHRB70g51oY1HzDPQkhqJ4zFv
# +UFkWFFYwk2Y5dFjBzLE4mlTyMIIp5UePYRavBiFvTRiL3b8/GzUvW2klxjOptd8
# TGOs333KYInYRXa56kTfJdtSkCTMl3XoWdooxjFMJ+HjbxpUOq3QSnxVX2mrzqsS
# dOBis1zJN6IXcf+txgUCAAkZVYIlw3qbiwkT2VAnZCOCMn8+jKGvyU5p+BPOPMGw
# n3ti40SJkYKBqodHXrc=
# SIG # End signature block
