using System.IO;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace PKHeX.Avalonia.Converters;

/// <summary>
/// Converts SkiaSharp bitmaps to Avalonia bitmaps for display in the UI.
/// </summary>
public static class SKBitmapConverter
{
    /// <summary>
    /// Converts an <see cref="SKBitmap"/> to an Avalonia <see cref="Bitmap"/>.
    /// Returns null if the input is null.
    /// </summary>
    public static Bitmap? ToAvaloniaImage(this SKBitmap? skBitmap)
    {
        if (skBitmap is null)
            return null;
        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream(data.ToArray());
        return new Bitmap(stream);
    }
}
