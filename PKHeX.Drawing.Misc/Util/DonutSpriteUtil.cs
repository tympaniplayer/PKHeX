using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using System;
using SkiaSharp;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving and composing sprites for Donuts.
/// </summary>
public static class DonutSpriteUtil
{
    /// <summary>
    /// Gets the sprite image for the specified <see cref="Donut9a"/>.
    /// </summary>
    public static SKBitmap? Sprite(this Donut9a donut) => GetDonutImage(donut);

    public static SKBitmap? StarSprite => GetResourceBitmap("star");
    public static SKBitmap? GetDonutFlavorImage(string donut) => GetResourceBitmap(donut);
    public static SKBitmap? GetFlavorProfileImage() => GetResourceBitmap("flavorprofile");

    private static SKBitmap? GetResourceBitmap(string name)
    {
        var obj = Resources.ResourceManager.GetObject(name);
        if (obj is byte[] bytes)
            return SKBitmap.Decode(bytes);
        return null;
    }

    private static SKBitmap? GetDonutImage(Donut9a donut)
    {
        if (donut.Donut is >= 198 and <= 202)
            return GetSpecialDonutImage(donut);

        ReadOnlySpan<string> flavors = ["sweet", "spicy", "sour", "bitter", "fresh", "mix"];
        var variant = donut.Donut % 6;
        var stars = donut.Stars;
        var flavor = flavors[variant];
        var resource = $"donut_{flavor}{stars:00}";
        return GetResourceBitmap(resource);
    }

    private static SKBitmap? GetSpecialDonutImage(Donut9a donut) => donut.Donut switch
    {
        198 => Resources.donut_uni491, // Bad Dreams Cruller
        199 => Resources.donut_uni383, // Omega Old-Fashioned Donut
        200 => Resources.donut_uni382, // Alpha Old-Fashioned Donut
        201 => Resources.donut_uni384, // Delta Old-Fashioned Donut
        202 => Resources.donut_uni807, // Plasma-Glazed Donut
        _ => null,
    };
}
