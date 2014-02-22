Param (
    $parameters = @{},
    $srcFolder,
    $scriptFolder,
    $projectName,
    $projectVersion
)

# get script variables
$nugetApiKey = $parameters["NuGetApiKey-secure"]

# update package version in nuspec file
Write-Output "Updating version in nuspec file"
$nuspecPath = "$scriptFolder\NLog.Targets.Chatwork.nuspec"
[xml]$xml = Get-Content $nuspecPath
$xml.package.metadata.version = $projectVersion
$xml.Save($nuspecPath)

# build NuGet package
Write-Output "Building NuGet package"
."$srcFolder\.nuget\NuGet.exe" pack $scriptFolder\NLog.Targets.Chatwork.nuspec

# publish NuGet package
Write-Output "Publishing NuGet package"
."$srcFolder\.nuget\NuGet.exe" push $srcFolder\NLog.Targets.Chatwork.$projectVersion.nupkg $nugetApiKey
