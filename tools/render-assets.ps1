# Renders the Phileas logo (src/Phileas.Wpf/Assets/Logo.xaml) to phileas.ico and the splash screen PNG.
# Run with: powershell -STA -File tools/render-assets.ps1
Add-Type -AssemblyName PresentationCore, PresentationFramework, WindowsBase

$root = Split-Path $PSScriptRoot -Parent
$assets = Join-Path $root 'src\Phileas.Wpf\Assets'
$dictionary = [System.Windows.Markup.XamlReader]::Load([System.IO.File]::OpenRead((Join-Path $assets 'Logo.xaml')))
$logo = $dictionary['LogoImage']

function Save-Png($visual, $width, $height, $path) {
    $bitmap = New-Object System.Windows.Media.Imaging.RenderTargetBitmap $width, $height, 96, 96, ([System.Windows.Media.PixelFormats]::Pbgra32)
    $bitmap.Render($visual)
    $encoder = New-Object System.Windows.Media.Imaging.PngBitmapEncoder
    $encoder.Frames.Add([System.Windows.Media.Imaging.BitmapFrame]::Create($bitmap))
    $stream = [System.IO.File]::Create($path)
    $encoder.Save($stream)
    $stream.Close()
}

function New-LogoVisual($size) {
    $visual = New-Object System.Windows.Media.DrawingVisual
    $context = $visual.RenderOpen()
    $context.DrawImage($logo, (New-Object System.Windows.Rect 0, 0, $size, $size))
    $context.Close()
    $visual
}

# Icon with PNG-compressed entries
$sizes = 16, 32, 48, 64, 128, 256
$pngs = @()
foreach ($size in $sizes) {
    $temp = [System.IO.Path]::GetTempFileName()
    Save-Png (New-LogoVisual $size) $size $size $temp
    $pngs += , ([System.IO.File]::ReadAllBytes($temp))
    Remove-Item $temp
}
$ico = New-Object System.IO.MemoryStream
$writer = New-Object System.IO.BinaryWriter $ico
$writer.Write([uint16]0)
$writer.Write([uint16]1)
$writer.Write([uint16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $dimension = if ($sizes[$i] -ge 256) { 0 } else { $sizes[$i] }
    $writer.Write([byte]$dimension)
    $writer.Write([byte]$dimension)
    $writer.Write([byte]0)
    $writer.Write([byte]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]32)
    $writer.Write([uint32]$pngs[$i].Length)
    $writer.Write([uint32]$offset)
    $offset += $pngs[$i].Length
}
foreach ($png in $pngs) { $writer.Write($png) }
$writer.Flush()
[System.IO.File]::WriteAllBytes((Join-Path $assets 'phileas.ico'), $ico.ToArray())

# Splash screen: dark rounded card with logo, name and a loading hint
$width = 520
$height = 320
$splash = New-Object System.Windows.Media.DrawingVisual
$context = $splash.RenderOpen()
$background = New-Object System.Windows.Media.LinearGradientBrush ([System.Windows.Media.Color]::FromRgb(0x35, 0x63, 0xB8)), ([System.Windows.Media.Color]::FromRgb(0x4B, 0x84, 0x66)), 20
$border = New-Object System.Windows.Media.Pen ([System.Windows.Media.Brushes]::White), 0
$context.DrawRoundedRectangle($background, $border, (New-Object System.Windows.Rect 1, 1, ($width - 2), ($height - 2)), 24, 24)
$context.DrawImage($logo, (New-Object System.Windows.Rect (($width - 128) / 2), 36, 128, 128))

function Draw-Text($text, $size, $weight, $color, $y) {
    $typeface = New-Object System.Windows.Media.Typeface 'Segoe UI'
    $formatted = New-Object System.Windows.Media.FormattedText $text, ([System.Globalization.CultureInfo]::InvariantCulture), ([System.Windows.FlowDirection]::LeftToRight), $typeface, $size, $color, 1.0
    $formatted.SetFontWeight($weight)
    $context.DrawText($formatted, (New-Object System.Windows.Point (($width - $formatted.Width) / 2), $y))
}
Draw-Text 'Phileas' 34 ([System.Windows.FontWeights]::Bold) ([System.Windows.Media.Brushes]::White) 176
Draw-Text 'Route optimisation - Simulated Annealing vs Tabu Search' 12 ([System.Windows.FontWeights]::Normal) ([System.Windows.Media.Brushes]::WhiteSmoke) 226
Draw-Text 'Loading...' 12 ([System.Windows.FontWeights]::SemiBold) ([System.Windows.Media.Brushes]::White) 270
$context.Close()
Save-Png $splash $width $height (Join-Path $assets 'splash.png')
