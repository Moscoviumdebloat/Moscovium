
Add-Type -AssemblyName System.Drawing

$source = "$PSScriptRoot\Assets\Illustration.gif"
$dest = "$PSScriptRoot\Assets\WizardImage.bmp"

$bmp = [System.Drawing.Bitmap]::FromFile($source)
$bmp.SelectActiveFrame([System.Drawing.Imaging.FrameDimension]::Time, 0)

# Resize to standard wizard image size? Inno Setup modern wizard sidebar is usually 164x314 (WizardImageFile) or 55x58 (WizardSmallImageFile).
# But standard WizardImageFile can be larger and it scales. Let's just save as BMP.

$bmp.Save($dest, [System.Drawing.Imaging.ImageFormat]::Bmp)

$bmp.Dispose()
Write-Host "Converted $source -> $dest"
