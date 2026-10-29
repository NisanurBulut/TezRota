# Starts Phileas, presses "Compare both methods" and records the window into an animated GIF.
# Usage: powershell -STA -File tools/record-demo-gif.ps1 [-Exe <path to Phileas.Wpf.exe>] [-Out docs/images/phileas-demo.gif]
param(
    [string]$Exe = (Join-Path $PSScriptRoot '..\src\Phileas.Wpf\bin\Debug\net10.0-windows\Phileas.Wpf.exe'),
    [string]$Out = (Join-Path $PSScriptRoot '..\docs\images\phileas-demo.gif'),
    [int]$Width = 700,
    [double]$RecordSeconds = 6.0,
    [int]$MinFrameMs = 100
)

Add-Type -AssemblyName System.Drawing, PresentationCore, WindowsBase, UIAutomationClient, UIAutomationTypes
Add-Type @"
using System; using System.Runtime.InteropServices;
public class Win { [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr dc, uint f);
 [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
 [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr o);
 public struct RECT { public int L, T, R, B; } }
"@

$Exe = (Resolve-Path $Exe).Path
$AE = [System.Windows.Automation.AutomationElement]

function Find-Window($processId) {
    $condition = New-Object System.Windows.Automation.PropertyCondition $AE::ProcessIdProperty, $processId
    $AE::RootElement.FindAll([System.Windows.Automation.TreeScope]::Children, $condition) |
        Sort-Object { $_.Current.BoundingRectangle.Width } -Descending | Select-Object -First 1
}

function Capture-Frame($handle, $width) {
    $rect = New-Object Win+RECT
    [Win]::GetWindowRect($handle, [ref]$rect) | Out-Null
    $full = New-Object System.Drawing.Bitmap ($rect.R - $rect.L), ($rect.B - $rect.T)
    $graphics = [System.Drawing.Graphics]::FromImage($full)
    $dc = $graphics.GetHdc()
    [Win]::PrintWindow($handle, $dc, 2) | Out-Null
    $graphics.ReleaseHdc($dc)
    $graphics.Dispose()
    $height = [int]($full.Height * $width / $full.Width)
    $small = New-Object System.Drawing.Bitmap $width, $height
    $g = [System.Drawing.Graphics]::FromImage($small)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.DrawImage($full, 0, 0, $width, $height)
    $g.Dispose()
    $full.Dispose()
    $hBitmap = $small.GetHbitmap()
    $source = [System.Windows.Interop.Imaging]::CreateBitmapSourceFromHBitmap($hBitmap, [IntPtr]::Zero, [System.Windows.Int32Rect]::Empty, [System.Windows.Media.Imaging.BitmapSizeOptions]::FromEmptyOptions())
    $source.Freeze()
    [Win]::DeleteObject($hBitmap) | Out-Null
    $small.Dispose()
    $source
}

$process = Start-Process $Exe -PassThru
Start-Sleep -Seconds 4
$window = Find-Window $process.Id
$handle = [IntPtr]$window.Current.NativeWindowHandle
$buttons = $window.FindAll([System.Windows.Automation.TreeScope]::Descendants, (New-Object System.Windows.Automation.PropertyCondition -ArgumentList $AE::ControlTypeProperty, ([System.Windows.Automation.ControlType]::Button)))
$compare = $buttons | Where-Object { $_.Current.Name -like '*Compare*' } | Select-Object -First 1
$invoke = $compare.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)

$frames = New-Object System.Collections.ArrayList
$times = New-Object System.Collections.ArrayList
$clock = [System.Diagnostics.Stopwatch]::StartNew()
$pressed = $false
while ($clock.Elapsed.TotalSeconds -lt $RecordSeconds) {
    if (-not $pressed -and $clock.Elapsed.TotalSeconds -gt 0.8) {
        $invoke.Invoke()
        $pressed = $true
    }
    [void]$frames.Add((Capture-Frame $handle $Width))
    [void]$times.Add($clock.Elapsed.TotalMilliseconds)
    $spare = $MinFrameMs - ($clock.Elapsed.TotalMilliseconds - $times[$times.Count - 1])
    if ($spare -gt 0) { Start-Sleep -Milliseconds $spare }
}
$process.CloseMainWindow() | Out-Null
Start-Sleep -Milliseconds 500
if (-not $process.HasExited) { $process.Kill() }

# Hold the first frame a little and the last one longer so the result can be read
$encoder = New-Object System.Windows.Media.Imaging.GifBitmapEncoder
$delays = New-Object System.Collections.ArrayList
for ($i = 0; $i -lt $frames.Count; $i++) {
    $delay = if ($i -lt $frames.Count - 1) { $times[$i + 1] - $times[$i] } else { 2500 }
    [void]$delays.Add([uint16][Math]::Max(2, [Math]::Round($delay / 10)))
    $encoder.Frames.Add([System.Windows.Media.Imaging.BitmapFrame]::Create($frames[$i]))
}
$stream = New-Object System.IO.MemoryStream
$encoder.Save($stream)
$bytes = $stream.ToArray()

# Make the GIF loop forever: insert the NETSCAPE2.0 extension after the header, screen descriptor and global colour table
$offset = 13
if ($bytes[10] -band 0x80) { $offset += 3 * [Math]::Pow(2, ($bytes[10] -band 0x07) + 1) }
$loop = [byte[]](0x21, 0xFF, 0x0B) + [System.Text.Encoding]::ASCII.GetBytes('NETSCAPE2.0') + [byte[]](0x03, 0x01, 0x00, 0x00, 0x00)
$result = $bytes[0..($offset - 1)] + $loop + $bytes[$offset..($bytes.Length - 1)]
# Write each frame's delay (in 1/100 s) into its Graphic Control Extension block (21 F9 04 flags delayLo delayHi transparent 00)
$found = 0
for ($k = 0; $k -lt $result.Length - 8 -and $found -lt $delays.Count; $k++) {
    if ($result[$k] -eq 0x21 -and $result[$k + 1] -eq 0xF9 -and $result[$k + 2] -eq 0x04 -and $result[$k + 7] -eq 0x00) {
        $result[$k + 4] = $delays[$found] -band 0xFF
        $result[$k + 5] = $delays[$found] -shr 8
        $found++
    }
}
if ($found -ne $delays.Count) { Write-Warning "Patched $found of $($delays.Count) frame delays" }
[System.IO.File]::WriteAllBytes($Out, [byte[]]$result)
"{0} frames, {1:N1} MB -> {2}" -f $frames.Count, ($result.Length / 1MB), $Out
