using SkiaSharp;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving player sprite images from save files.
/// </summary>
public static class PlayerSpriteUtil
{
    /// <summary>
    /// Gets the player sprite image for the specified <see cref="SaveFile"/>.
    /// </summary>
    public static SKBitmap? Sprite(this SaveFile sav) => GetSprite(sav);

    private static SKBitmap? GetSprite(SaveFile sav)
    {
        if (sav is IMultiplayerSprite ms)
        {
            // Gen6 only
            string file = $"tr_{ms.MultiplayerSpriteID:00}";
            var obj = Resources.ResourceManager.GetObject(file);
            if (obj is byte[] bytes)
                return SKBitmap.Decode(bytes);
            return Resources.tr_00;
        }
        return null;
    }
}
