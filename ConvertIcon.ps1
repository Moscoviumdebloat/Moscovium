
Add-Type -AssemblyName System.Drawing

# Define DestroyIcon to fix GDI handle leak
if (-not ("Win32.NativeMethods" -as [type])) {
    $signature = @"
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);
"@
    Add-Type -MemberDefinition $signature -Namespace "Win32" -Name "NativeMethods"
}

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

# Properly destroy the icon handle to fix the GDI leak
[Win32.NativeMethods]::DestroyIcon($handle) | Out-Null

Write-Host "Success: Converted $source -> $dest"
