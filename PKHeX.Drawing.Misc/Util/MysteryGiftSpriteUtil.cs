using SkiaSharp;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides utility methods for retrieving and composing sprites for Mystery Gifts.
/// </summary>
public static class MysteryGiftSpriteUtil
{
    /// <summary>
    /// Gets the sprite image for the specified <see cref="MysteryGift"/>.
    /// </summary>
    public static SKBitmap Sprite(this MysteryGift gift) => GetSprite(gift);

    private static SKBitmap GetSprite(MysteryGift gift)
    {
        if (gift.IsEmpty)
            return SpriteUtil.Spriter.None;

        var img = GetBaseImage(gift);
        if (SpriteBuilder.ShowEncounterColor != SpriteBackgroundType.None)
            SpriteUtil.ApplyEncounterColor(gift, img, SpriteBuilder.ShowEncounterColor);
        if (gift.GiftUsed)
            img.ChangeOpacity(0.3);
        return img;
    }

    private static SKBitmap GetBaseImage(MysteryGift gift)
    {
        if (gift is { IsEgg: true, Species: (int)Species.Manaphy }) // Manaphy Egg
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);
        if (gift.IsEntity)
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);

        if (gift.IsItem)
        {
            var item = (ushort)gift.ItemID;
            if (ItemStorage7USUM.GetCrystalHeld(item, out var value))
                item = value;
            return SpriteUtil.GetItemSprite(item) ?? Resources.Bag_Key;
        }
        return PokeSprite.Properties.Resources.b_unknown;
    }
}
