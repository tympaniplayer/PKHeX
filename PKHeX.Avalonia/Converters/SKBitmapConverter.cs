using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using SkiaSharp;

namespace PKHeX.Avalonia.Converters;

/// <summary>
/// Converts SkiaSharp bitmaps to Avalonia bitmaps for display in the UI.
/// </summary>
public static class SKBitmapConverter
{
    /// <summary>
    /// Converts an <see cref="SKBitmap"/> to an Avalonia <see cref="Bitmap"/>.
    /// </summary>
    public static Bitmap ToAvaloniaImage(this SKBitmap skBitmap)
    {
        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream(data.ToArray());
        return new Bitmap(stream);
    }

    /// <summary>
    /// Converts a nullable <see cref="SKBitmap"/> to a nullable Avalonia <see cref="Bitmap"/>.
    /// </summary>
    public static Bitmap? ToAvaloniaImage(this SKBitmap? skBitmap)
    {
        return skBitmap is null ? null : skBitmap.ToAvaloniaImage();
    }
}
