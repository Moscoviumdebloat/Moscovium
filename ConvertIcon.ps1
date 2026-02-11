
Add-Type -AssemblyName System.Drawing

$source = "$PSScriptRoot\Assets\Illustration.gif"
$dest = "$PSScriptRoot\Assets\Logo.ico"
$backup = "$PSScriptRoot\Assets\Logo.ico.bak"

if (Test-Path $dest) {
    Copy-Item $dest $backup -Force
    Write-Host "Backed up $dest to $backup"
}

$bmp = [System.Drawing.Bitmap]::FromFile($source)
# Select first frame
$bmp.SelectActiveFrame([System.Drawing.Imaging.FrameDimension]::Time, 0)
# Create Icon
$handle = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($handle)

$fs = New-Object System.IO.FileStream($dest, [System.IO.FileMode]::Create)
$icon.Save($fs)
$fs.Close()

$icon.Dispose()
$bmp.Dispose()
# Note: $handle leak is negligible for this one-off script
Write-Host "Success: Converted $source -> $dest"
