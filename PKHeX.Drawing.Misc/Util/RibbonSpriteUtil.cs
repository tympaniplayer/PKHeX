using SkiaSharp;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving ribbon sprite images.
/// </summary>
public static class RibbonSpriteUtil
{
    /// <summary>
    /// Gets the ribbon sprite image for the specified <see cref="RibbonIndex"/>.
    /// </summary>
    public static SKBitmap? GetRibbonSprite(RibbonIndex ribbon)
    {
        var name = $"Ribbon{ribbon}";
        return GetRibbonSprite(name);
    }

    /// <summary>
    /// Gets the ribbon sprite image for the specified ribbon name.
    /// </summary>
    public static SKBitmap? GetRibbonSprite(string name)
    {
        var resource = name.Replace("CountG3", "G3").ToLowerInvariant();
        return GetResourceBitmap(resource);
    }

    /// <summary>
    /// Gets the ribbon sprite image for the specified ribbon name, maximum value, and current value.
    /// </summary>
    public static SKBitmap? GetRibbonSprite(string name, int max, int value)
    {
        var resource = GetRibbonSpriteName(name, max, value);
        return GetResourceBitmap(resource);
    }

    private static SKBitmap? GetResourceBitmap(string name)
    {
        var obj = Resources.ResourceManager.GetObject(name);
        if (obj is byte[] bytes)
            return SKBitmap.Decode(bytes);
        return null;
    }

    private static string GetRibbonSpriteName(string name, int max, int value)
    {
        if (max != 4) // Memory
        {
            var sprite = name.ToLowerInvariant();
            if (value >= max)
                return sprite + "2";
            return sprite;
        }

        // Count ribbons
        string n = name.Replace("Count", string.Empty).ToLowerInvariant();
        return value switch
        {
            2 => n + "super",
            3 => n + "hyper",
            4 => n + "master",
            _ => n,
        };
    }
}
