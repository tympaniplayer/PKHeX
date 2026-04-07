using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace PKHeX.WinForms;

/// <summary>
/// Extension methods for converting between SkiaSharp and System.Drawing bitmap types.
/// </summary>
public static class SKBitmapExtensions
{
    /// <summary>
    /// Converts an <see cref="SKBitmap"/> to a <see cref="Bitmap"/> for use in WinForms controls.
    /// </summary>
    public static Bitmap ToBitmap(this SKBitmap skBitmap)
    {
        var info = new SKImageInfo(skBitmap.Width, skBitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        using var pixmap = skBitmap.PeekPixels();

        // If the format already matches, use the data directly
        if (skBitmap.ColorType == SKColorType.Bgra8888)
        {
            var bmp = new Bitmap(skBitmap.Width, skBitmap.Height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            var srcSpan = skBitmap.GetPixelSpan();
            Marshal.Copy(srcSpan.ToArray(), 0, bmpData.Scan0, srcSpan.Length);

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        // Convert to BGRA8888 first
        using var converted = new SKBitmap(info);
        skBitmap.CopyTo(converted, SKColorType.Bgra8888);
        return converted.ToBitmap();
    }

    /// <summary>
    /// Converts a nullable <see cref="SKBitmap"/> to a nullable <see cref="Image"/>.
    /// </summary>
    public static Image? ToImage(this SKBitmap? skBitmap)
    {
        return skBitmap?.ToBitmap();
    }

    /// <summary>
    /// Converts an <see cref="SKColor"/> to a <see cref="Color"/>.
    /// </summary>
    public static Color ToDrawingColor(this SKColor c) => Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);

    /// <summary>
    /// Converts a <see cref="Color"/> to an <see cref="SKColor"/>.
    /// </summary>
    public static SKColor ToSKColor(this Color c) => new(c.R, c.G, c.B, c.A);

    /// <summary>
    /// Converts a <see cref="Bitmap"/> to an <see cref="SKBitmap"/>.
    /// </summary>
    public static SKBitmap ToSKBitmap(this Bitmap bitmap)
    {
        var info = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var skBitmap = new SKBitmap(info);

        var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var ptr = skBitmap.GetPixels();
        var length = bmpData.Stride * bitmap.Height;

        unsafe
        {
            Buffer.MemoryCopy((void*)bmpData.Scan0, (void*)ptr, skBitmap.ByteCount, length);
        }

        bitmap.UnlockBits(bmpData);
        return skBitmap;
    }
}
