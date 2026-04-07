using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for a single party slot.
/// </summary>
public partial class PartySlotViewModel : ObservableObject
{
    [ObservableProperty] private Bitmap? _sprite;
    [ObservableProperty] private string _summary = string.Empty;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private int _currentHP;
    [ObservableProperty] private int _maxHP;
    [ObservableProperty] private string _statusText = string.Empty;

    public PKM Pokemon { get; }
    public int Index { get; }

    public PartySlotViewModel(PKM pk, SaveFile sav, int index)
    {
        Pokemon = pk;
        Index = index;
        IsEmpty = pk.Species == 0;

        if (!IsEmpty)
        {
            Sprite = pk.Sprite(sav).ToAvaloniaImage();
            var species = SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, pk.Format);
            Summary = $"{species} Lv.{pk.CurrentLevel}";
            CurrentHP = pk.Stat_HPCurrent;
            MaxHP = pk.Stat_HPMax;

            var status = (StatusCondition)pk.Status_Condition;
            StatusText = status == StatusCondition.None ? string.Empty : status.ToString();
        }
    }
}

/// <summary>
/// ViewModel for the party viewer.
/// </summary>
public partial class PartyViewModel : ObservableObject
{
    public ObservableCollection<PartySlotViewModel> Slots { get; } = [];

    public PartyViewModel(SaveFile sav)
    {
        var party = sav.PartyData;
        for (int i = 0; i < party.Count; i++)
            Slots.Add(new PartySlotViewModel(party[i], sav, i));
    }
}
