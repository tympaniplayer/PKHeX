using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    [ObservableProperty]
    private string _boxName = string.Empty;

    [ObservableProperty]
    private Bitmap? _wallpaper;

    public ObservableCollection<SlotViewModel> Slots { get; } = [];

    public BoxViewModel(SaveFile sav, int box)
    {
        BoxName = sav.GetBoxName(box);

        // Load wallpaper
        var wp = sav.WallpaperImage(box);
        Wallpaper = wp.ToAvaloniaImage();

        // Load slots (30 per box, 5 columns x 6 rows)
        int slotCount = sav.BoxSlotCount;
        for (int slot = 0; slot < slotCount; slot++)
        {
            var pk = sav.GetBoxSlotAtIndex(box, slot);
            Slots.Add(new SlotViewModel(pk, sav, box, slot));
        }
    }
}

public partial class SlotViewModel : ObservableObject
{
    [ObservableProperty]
    private Bitmap? _sprite;

    [ObservableProperty]
    private string _summary = string.Empty;

    [ObservableProperty]
    private bool _isEmpty;

    public PKM Pokemon { get; }

    public SlotViewModel(PKM pk, SaveFile sav, int box, int slot)
    {
        Pokemon = pk;
        IsEmpty = pk.Species == 0;

        if (!IsEmpty)
        {
            var skSprite = pk.Sprite(sav, box, slot);
            Sprite = skSprite.ToAvaloniaImage();
            Summary = $"{SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, pk.Format)} Lv.{pk.CurrentLevel}";
        }
    }
}
