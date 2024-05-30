# This script originally (c) 2016 Serilog Contributors - license Apache 2.0
# major modification that were made by Six-Sided Software:
# - removed script parts that dealt with AppVeyor
# - disabled the section that dealth with tests

param(
    [string]$version
)

echo "build: Build started"

Push-Location $PSScriptRoot

if(Test-Path .\artifacts) {
	echo "build: Cleaning .\artifacts"
	Remove-Item .\artifacts -Force -Recurse
}

& dotnet restore --no-cache
if($LASTEXITCODE -ne 0) { exit 1 }    

$branch = $(git symbolic-ref --short -q HEAD)
$revision = @{ $true = ""; $false = "local" }[$version -ne $NULL -and $version -ne ""];
$suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "main" -and $revision -ne "local"]

echo "build: branch is $branch"
echo "build: revision is $revision"
echo "build: Version suffix is $suffix"

foreach ($src in ls src/*) {
    Push-Location $src

    echo "build: Packaging project in $src"

    if ($suffix) {
        & dotnet publish -c Release -o ./obj/publish --version-suffix=$suffix
        & dotnet pack -c Release -o ..\..\artifacts --no-build --version-suffix=$suffix
    } else {
        & dotnet publish -c Release -o ./obj/publish
        & dotnet pack -c Release -o ..\..\artifacts --no-build
    }
    if($LASTEXITCODE -ne 0) { exit 1 }    

    Pop-Location
}

#foreach ($test in ls test/*.Tests) {
#    Push-Location $test
#
#    echo "build: Testing project in $test"
#
#    & dotnet test -c Release
#    if($LASTEXITCODE -ne 0) { exit 3 }
#
#    Pop-Location
#}

Pop-Location