using SkiaSharp;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Properties;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// 56 high, 68 wide sprite builder
/// </summary>
public sealed class SpriteBuilder5668s : SpriteBuilder
{
    public override int Height => 56;
    public override int Width => 68;

    protected override int ItemShiftX => 2;
    protected override int ItemShiftY => 2;
    protected override int ItemMaxSize => 32;
    protected override int EggItemShiftX => 18;
    protected override int EggItemShiftY => 1;
    public override bool HasFallbackMethod => true;

    protected override string GetSpriteStringSpeciesOnly(ushort species) => 'b' + $"_{species}";
    protected override string GetSpriteAll(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'b' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetSpriteAllSecondary(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'c' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetItemResourceName(int item) => 'b' + $"item_{item}";
    protected override SKBitmap Unknown => Resources.b_unknown;
    protected override SKBitmap GetEggSprite(ushort species) => species == (int)Species.Manaphy ? Resources.b_490_e : Resources.b_egg;

    public override SKBitmap Hover { get; } = Resources.slotHover68;
    public override SKBitmap View { get; } = Resources.slotView68;
    public override SKBitmap Set { get; } = Resources.slotSet68;
    public override SKBitmap Delete { get; } = Resources.slotDel68;
    public override SKBitmap Transparent { get; } = Resources.slotTrans68;
    public override SKBitmap Drag => Resources.slotDrag68;
    public override SKBitmap UnknownItem => Resources.bitem_unk;
    public override SKBitmap None { get; } = Resources.b_0;
    public override SKBitmap ItemTM => Resources.bitem_tm;
    public override SKBitmap ItemTR => Resources.bitem_tr;
    public override SKBitmap ShadowLugia => Resources.b_249x;
}
