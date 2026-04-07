using SkiaSharp;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving type sprite images for Pokémon types.
/// </summary>
public static class TypeSpriteUtil
{
    private static SKBitmap? Get(string name)
    {
        var obj = Resources.ResourceManager.GetObject(name);
        if (obj is byte[] bytes)
            return SKBitmap.Decode(bytes);
        return null;
    }

    /// <summary>
    /// Gets the wide type sprite image for the specified type and generation.
    /// </summary>
    public static SKBitmap? GetTypeSpriteWide(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_wide_{type:00}");
    }

    /// <summary>
    /// Gets the icon type sprite image for the specified type and generation.
    /// </summary>
    public static SKBitmap? GetTypeSpriteIcon(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_{type:00}");
    }

    /// <summary>
    /// Gets the small icon type sprite image for the specified type and generation.
    /// </summary>
    public static SKBitmap? GetTypeSpriteIconSmall(byte type, byte generation = Latest.Generation)
    {
        if (generation <= 2)
            type = (byte)((MoveType)type).GetMoveTypeGeneration(generation);
        return Get($"type_icon_s_{type:00}");
    }

    /// <summary>
    /// Gets the gem type sprite image for the specified type.
    /// </summary>
    public static SKBitmap? GetTypeSpriteGem(byte type)
    {
        return Get($"gem_{type:00}");
    }
}
