#:property TargetFramework=net10.0-windows
#:property UseWPF=true
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=false
#:property PublishTrimmed=false

// Starts Phileas, presses "Compare both methods" and records the window into an animated GIF.
// Run from the repository root (after building the Debug configuration):
//   dotnet run tools/record-demo-gif.cs [path to Phileas.Wpf.exe] [output.gif] [width]
//
// Quality: frames are captured at full size and area-averaged down, one 255-colour palette is built for the whole
// animation, gradients get ordered dithering, and every frame stores only the rectangle that changed.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Automation;

var exePath = Path.GetFullPath(args.Length > 0 ? args[0] : "src/Phileas.Wpf/bin/Debug/net10.0-windows/Phileas.Wpf.exe");
var outPath = Path.GetFullPath(args.Length > 1 ? args[1] : "docs/images/phileas-demo.gif");
var targetWidth = args.Length > 2 ? int.Parse(args[2]) : 1200;
const double recordSeconds = 6.5;
const double pressAtSeconds = 0.8;

using var process = Process.Start(exePath) ?? throw new InvalidOperationException("Could not start " + exePath);
Thread.Sleep(4000);

var window = FindWindow(process.Id);
var handle = new IntPtr(window.Current.NativeWindowHandle);
var compare = window.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
    .Cast<AutomationElement>()
    .First(button => button.Current.Name.Contains("Compare"));
var invoke = (InvokePattern)compare.GetCurrentPattern(InvokePattern.Pattern);

var frames = new List<byte[]>();
var times = new List<double>();
int cropWidth = 0, cropHeight = 0;
var clock = Stopwatch.StartNew();
var pressed = false;
while (clock.Elapsed.TotalSeconds < recordSeconds)
{
    if (!pressed && clock.Elapsed.TotalSeconds > pressAtSeconds)
    {
        invoke.Invoke();
        pressed = true;
    }
    var frameStart = clock.Elapsed.TotalMilliseconds;
    frames.Add(Capture(handle, out cropWidth, out cropHeight));
    times.Add(frameStart);
    Thread.Sleep((int)Math.Max(0, 80 - (clock.Elapsed.TotalMilliseconds - frameStart))); // about 12 frames per second
}

process.CloseMainWindow();
if (!process.WaitForExit(2000)) process.Kill();

Console.WriteLine($"{frames.Count} frames captured at {frames.Count / clock.Elapsed.TotalSeconds:F1} fps ({cropWidth}x{cropHeight})");

var width = targetWidth;
var height = (int)Math.Round(cropHeight * (double)targetWidth / cropWidth);
frames = frames.Select(frame => Downscale(frame, cropWidth, cropHeight, width, height)).ToList();

var palette = BuildPalette(frames, 255);
var indexed = frames.Select(frame => Quantize(frame, width, height, palette)).ToList();
WriteGif(outPath, indexed, times, width, height, palette);
Console.WriteLine($"{new FileInfo(outPath).Length / 1048576.0:F1} MB -> {outPath}");

// ---------------------------------------------------------------------------------------------------------------

static AutomationElement FindWindow(int processId) =>
    AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ProcessIdProperty, processId))
        .Cast<AutomationElement>()
        .OrderByDescending(element => element.Current.BoundingRectangle.Width)
        .First();

static byte[] Capture(IntPtr handle, out int cropWidth, out int cropHeight)
{
    NativeMethods.GetWindowRect(handle, out var outer);
    NativeMethods.DwmGetWindowAttribute(handle, 9, out var visible, Marshal.SizeOf<NativeMethods.Rect>());

    int fullWidth = outer.Right - outer.Left, fullHeight = outer.Bottom - outer.Top;
    var header = new NativeMethods.BitmapInfo
    {
        Size = 40, Width = fullWidth, Height = -fullHeight, Planes = 1, BitCount = 32,
    };
    var screen = NativeMethods.GetDC(IntPtr.Zero);
    var memory = NativeMethods.CreateCompatibleDC(screen);
    var bitmap = NativeMethods.CreateDIBSection(memory, ref header, 0, out var bits, IntPtr.Zero, 0);
    var previous = NativeMethods.SelectObject(memory, bitmap);
    NativeMethods.PrintWindow(handle, memory, 2);

    var full = new byte[fullWidth * fullHeight * 4];
    Marshal.Copy(bits, full, 0, full.Length);
    NativeMethods.SelectObject(memory, previous);
    NativeMethods.DeleteObject(bitmap);
    NativeMethods.DeleteDC(memory);
    NativeMethods.ReleaseDC(IntPtr.Zero, screen);

    // Crop to the visible frame (drops the invisible resize border).
    int cropLeft = visible.Left - outer.Left, cropTop = visible.Top - outer.Top;
    cropWidth = visible.Right - visible.Left;
    cropHeight = visible.Bottom - visible.Top;
    var cropped = new byte[cropWidth * cropHeight * 4];
    for (var y = 0; y < cropHeight; y++)
    {
        Buffer.BlockCopy(full, ((cropTop + y) * fullWidth + cropLeft) * 4, cropped, y * cropWidth * 4, cropWidth * 4);
    }
    return cropped;
}

// Area averaging: every target pixel is the weighted mean of the source pixels it covers.
static byte[] Downscale(byte[] source, int sourceWidth, int sourceHeight, int width, int height)
{
    var result = new byte[width * height * 4];
    double scaleX = (double)sourceWidth / width, scaleY = (double)sourceHeight / height;
    for (var y = 0; y < height; y++)
    {
        double y0 = y * scaleY, y1 = y0 + scaleY;
        for (var x = 0; x < width; x++)
        {
            double x0 = x * scaleX, x1 = x0 + scaleX, b = 0, g = 0, r = 0, weightSum = 0;
            for (var sy = (int)y0; sy < Math.Ceiling(y1) && sy < sourceHeight; sy++)
            {
                var wy = Math.Min(y1, sy + 1) - Math.Max(y0, sy);
                for (var sx = (int)x0; sx < Math.Ceiling(x1) && sx < sourceWidth; sx++)
                {
                    var weight = wy * (Math.Min(x1, sx + 1) - Math.Max(x0, sx));
                    var index = (sy * sourceWidth + sx) * 4;
                    b += source[index] * weight;
                    g += source[index + 1] * weight;
                    r += source[index + 2] * weight;
                    weightSum += weight;
                }
            }
            var target = (y * width + x) * 4;
            result[target] = (byte)(b / weightSum + 0.5);
            result[target + 1] = (byte)(g / weightSum + 0.5);
            result[target + 2] = (byte)(r / weightSum + 0.5);
        }
    }
    return result;
}

// Median cut. Colours are grouped into 5-bit bins and each bin counts with the square root of its pixels, so small
// areas (a green highlight, a thin line) get palette entries too instead of being swallowed by the big flat areas.
static (byte R, byte G, byte B)[] BuildPalette(List<byte[]> frames, int colours)
{
    var bins = new Dictionary<int, (long Count, long R, long G, long B)>();
    for (var f = 0; f < frames.Count; f += 2)
    {
        var frame = frames[f];
        for (var i = 0; i < frame.Length; i += 4 * 3)
        {
            int b = frame[i], g = frame[i + 1], r = frame[i + 2];
            var key = (r >> 3) << 10 | (g >> 3) << 5 | (b >> 3);
            bins.TryGetValue(key, out var bin);
            bins[key] = (bin.Count + 1, bin.R + r, bin.G + g, bin.B + b);
        }
    }

    var samples = new List<int>();
    foreach (var bin in bins.Values)
    {
        var colour = (int)(bin.R / bin.Count) << 16 | (int)(bin.G / bin.Count) << 8 | (int)(bin.B / bin.Count);
        for (var copies = (int)Math.Max(1, Math.Sqrt(bin.Count)); copies > 0; copies--) samples.Add(colour);
    }

    var boxes = new List<int[]> { samples.ToArray() };
    while (boxes.Count < colours)
    {
        var index = -1;
        var bestRange = 0;
        for (var i = 0; i < boxes.Count; i++)
        {
            if (boxes[i].Length < 2) continue;
            var range = Ranges(boxes[i]).Max();
            if (range > bestRange) { bestRange = range; index = i; }
        }
        if (index < 0) break;

        var box = boxes[index];
        var channel = Array.IndexOf(Ranges(box), bestRange);
        var shift = channel == 0 ? 16 : channel == 1 ? 8 : 0;
        Array.Sort(box, (a, b) => ((a >> shift) & 255).CompareTo((b >> shift) & 255));
        var half = box.Length / 2;
        boxes[index] = box[..half];
        boxes.Add(box[half..]);
    }

    return [.. boxes.Select(box => (
        R: (byte)(box.Average(c => (c >> 16) & 255) + 0.5),
        G: (byte)(box.Average(c => (c >> 8) & 255) + 0.5),
        B: (byte)(box.Average(c => c & 255) + 0.5)))];

    static int[] Ranges(int[] box)
    {
        int minR = 255, maxR = 0, minG = 255, maxG = 0, minB = 255, maxB = 0;
        foreach (var c in box)
        {
            int r = (c >> 16) & 255, g = (c >> 8) & 255, b = c & 255;
            minR = Math.Min(minR, r); maxR = Math.Max(maxR, r);
            minG = Math.Min(minG, g); maxG = Math.Max(maxG, g);
            minB = Math.Min(minB, b); maxB = Math.Max(maxB, b);
        }
        return [maxR - minR, maxG - minG, maxB - minB];
    }
}

// Ordered dithering is position-based, so a pixel that does not change keeps the same index in the next frame.
static byte[] Quantize(byte[] frame, int width, int height, (byte R, byte G, byte B)[] palette)
{
    int[] bayer = [0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5];
    var result = new byte[width * height];
    var cache = new Dictionary<int, byte>();
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            var offset = (bayer[(y & 3) * 4 + (x & 3)] - 7.5) * 0.6;
            var source = (y * width + x) * 4;
            var b = Math.Clamp((int)(frame[source] + offset), 0, 255);
            var g = Math.Clamp((int)(frame[source + 1] + offset), 0, 255);
            var r = Math.Clamp((int)(frame[source + 2] + offset), 0, 255);
            var key = (r >> 2) << 12 | (g >> 2) << 6 | (b >> 2);
            if (!cache.TryGetValue(key, out var best))
            {
                var bestDistance = int.MaxValue;
                for (var p = 0; p < palette.Length; p++)
                {
                    int dr = r - palette[p].R, dg = g - palette[p].G, db = b - palette[p].B;
                    var distance = dr * dr * 3 + dg * dg * 4 + db * db * 2;
                    if (distance < bestDistance) { bestDistance = distance; best = (byte)p; }
                }
                cache[key] = best;
            }
            result[y * width + x] = best;
        }
    }
    return result;
}

static void WriteGif(string path, List<byte[]> frames, List<double> times, int width, int height, (byte R, byte G, byte B)[] palette)
{
    const byte transparent = 255;
    using var stream = File.Create(path);
    using var writer = new BinaryWriter(stream);

    writer.Write("GIF89a"u8);
    writer.Write((ushort)width);
    writer.Write((ushort)height);
    writer.Write((byte)0xF7); // global colour table, 256 entries
    writer.Write((byte)0);
    writer.Write((byte)0);
    for (var i = 0; i < 256; i++)
    {
        var colour = i < palette.Length ? palette[i] : ((byte)0, (byte)0, (byte)0);
        writer.Write(colour.Item1);
        writer.Write(colour.Item2);
        writer.Write(colour.Item3);
    }
    writer.Write([(byte)0x21, 0xFF, 0x0B]);
    writer.Write("NETSCAPE2.0"u8);
    writer.Write([(byte)3, 1, 0, 0, 0]); // loop forever

    byte[]? previous = null;
    for (var f = 0; f < frames.Count; f++)
    {
        var frame = frames[f];
        int left = 0, top = 0, right = width - 1, bottom = height - 1;
        if (previous is not null)
        {
            left = width; top = height; right = -1; bottom = -1;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (frame[y * width + x] == previous[y * width + x]) continue;
                    if (x < left) left = x;
                    if (x > right) right = x;
                    if (y < top) top = y;
                    if (y > bottom) bottom = y;
                }
            }
            if (right < 0) { left = top = right = bottom = 0; } // nothing changed: a 1 x 1 pixel keeps the timing
        }

        int regionWidth = right - left + 1, regionHeight = bottom - top + 1;
        var region = new byte[regionWidth * regionHeight];
        for (var y = 0; y < regionHeight; y++)
        {
            for (var x = 0; x < regionWidth; x++)
            {
                var index = (top + y) * width + left + x;
                region[y * regionWidth + x] = previous is not null && frame[index] == previous[index] ? transparent : frame[index];
            }
        }

        var delay = f < frames.Count - 1 ? times[f + 1] - times[f] : 2500;
        writer.Write([(byte)0x21, 0xF9, 4, (byte)(previous is null ? 0x04 : 0x05)]); // do not dispose; transparency from frame 2
        writer.Write((ushort)Math.Max(2, Math.Round(delay / 10)));
        writer.Write(transparent);
        writer.Write((byte)0);

        writer.Write((byte)0x2C);
        writer.Write((ushort)left);
        writer.Write((ushort)top);
        writer.Write((ushort)regionWidth);
        writer.Write((ushort)regionHeight);
        writer.Write((byte)0);
        writer.Write((byte)8);
        WriteLzw(writer, region);
        writer.Write((byte)0);
        previous = frame;
    }
    writer.Write((byte)0x3B);
}

static void WriteLzw(BinaryWriter output, byte[] pixels)
{
    const int minCodeSize = 8, clear = 256, end = 257;
    var block = new byte[255];
    int blockLength = 0, bitBuffer = 0, bitCount = 0, codeSize = minCodeSize + 1, next = end + 1;
    var table = new Dictionary<int, int>();

    void Flush()
    {
        output.Write((byte)blockLength);
        output.Write(block, 0, blockLength);
        blockLength = 0;
    }

    void Emit(int code)
    {
        bitBuffer |= code << bitCount;
        bitCount += codeSize;
        while (bitCount >= 8)
        {
            block[blockLength++] = (byte)bitBuffer;
            bitBuffer >>= 8;
            bitCount -= 8;
            if (blockLength == 255) Flush();
        }
    }

    Emit(clear);
    var prefix = (int)pixels[0];
    for (var i = 1; i < pixels.Length; i++)
    {
        var colour = pixels[i];
        var key = prefix << 8 | colour;
        if (table.TryGetValue(key, out var code))
        {
            prefix = code;
            continue;
        }

        Emit(prefix);
        if (next > (1 << codeSize) - 1 && codeSize < 12) codeSize++;
        if (next < 4096)
        {
            table[key] = next++;
        }
        else
        {
            Emit(clear);
            table.Clear();
            next = end + 1;
            codeSize = minCodeSize + 1;
        }
        prefix = colour;
    }
    Emit(prefix);
    Emit(end);
    if (bitCount > 0) block[blockLength++] = (byte)bitBuffer;
    if (blockLength > 0) Flush();
}

static class NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    public struct BitmapInfo
    {
        public uint Size;
        public int Width, Height;
        public ushort Planes, BitCount;
        public uint Compression, SizeImage;
        public int XPelsPerMeter, YPelsPerMeter;
        public uint ColorsUsed, ColorsImportant, Reserved;
    }

    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr window, IntPtr dc, uint flags);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr window, out Rect rect);
    [DllImport("user32.dll")] public static extern IntPtr GetDC(IntPtr window);
    [DllImport("user32.dll")] public static extern int ReleaseDC(IntPtr window, IntPtr dc);
    [DllImport("gdi32.dll")] public static extern IntPtr CreateCompatibleDC(IntPtr dc);
    [DllImport("gdi32.dll")] public static extern IntPtr CreateDIBSection(IntPtr dc, ref BitmapInfo info, uint usage, out IntPtr bits, IntPtr section, uint offset);
    [DllImport("gdi32.dll")] public static extern IntPtr SelectObject(IntPtr dc, IntPtr obj);
    [DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr obj);
    [DllImport("gdi32.dll")] public static extern bool DeleteDC(IntPtr dc);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr window, int attribute, out Rect value, int size);
}
