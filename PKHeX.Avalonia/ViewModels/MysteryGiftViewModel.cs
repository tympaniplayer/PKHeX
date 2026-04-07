using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for a single mystery gift entry.
/// </summary>
public partial class MysteryGiftEntryViewModel : ObservableObject
{
    [ObservableProperty] private Bitmap? _sprite;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isUsed;
    [ObservableProperty] private bool _isEmpty;

    public MysteryGift Gift { get; }
    public int Index { get; }

    public MysteryGiftEntryViewModel(MysteryGift gift, int index)
    {
        Gift = gift;
        Index = index;
        IsEmpty = gift.IsEmpty;
        IsUsed = gift.GiftUsed;

        if (!IsEmpty)
        {
            Sprite = gift.Sprite().ToAvaloniaImage();
            Title = !string.IsNullOrEmpty(gift.CardTitle) ? gift.CardTitle : $"Gift #{index + 1}";
        }
    }
}

/// <summary>
/// ViewModel for the mystery gift album viewer.
/// </summary>
public partial class MysteryGiftViewModel : ObservableObject
{
    public ObservableCollection<MysteryGiftEntryViewModel> Gifts { get; } = [];
    public bool IsSupported { get; }

    public MysteryGiftViewModel(SaveFile sav)
    {
        if (sav is not IMysteryGiftStorageProvider provider)
        {
            IsSupported = false;
            return;
        }

        IsSupported = true;
        var storage = provider.MysteryGiftStorage;
        for (int i = 0; i < storage.GiftCountMax; i++)
        {
            var gift = storage.GetMysteryGift(i);
            Gifts.Add(new MysteryGiftEntryViewModel(gift, i));
        }
    }
}
