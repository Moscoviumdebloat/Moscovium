param(
    [string]$Version = "3.6.2"
)

# 1. Clear previous outputs
if (Test-Path "Releases") {
    Remove-Item "Releases" -Recurse -Force
}
if (Test-Path "PublishOutput") {
    Remove-Item "PublishOutput" -Recurse -Force
}

# 2. Publish Project
Write-Host "Publishing project..." -ForegroundColor Cyan
dotnet publish MoscoviumThree.csproj -c Release -r win-x64 --self-contained -o .\PublishOutput

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed!"
    exit 1
}

# 3. Create Velopack Release
Write-Host "Packaging with Velopack..." -ForegroundColor Cyan

# Find the vpk command
$vpk = Get-Command "vpk" -ErrorAction SilentlyContinue
if (-not $vpk) {
    Write-Error "vpk tool not found! Run: dotnet tool install -g vpk"
    exit 1
}

vpk pack -u Moscovium -v $Version -p .\PublishOutput -e MoscoviumThree.exe -o .\Releases -i Assets\Logo.ico -t "Moscovium"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Velopack packaging failed!"
    exit 1
}

Write-Host "Success! Release files are located in .\Releases" -ForegroundColor Green
Write-Host "Upload the contents of this folder to a GitHub Release to publish the update." -ForegroundColor Yellow
