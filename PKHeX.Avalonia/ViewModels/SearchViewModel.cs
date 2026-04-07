using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Core.Searching;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for searching/filtering Pokemon across all boxes.
/// </summary>
public partial class SearchViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private int _filterSpecies;
    [ObservableProperty] private int _filterType = -1;
    [ObservableProperty] private bool _filterShinyOnly;
    [ObservableProperty] private int _resultCount;

    public ObservableCollection<SearchResultViewModel> Results { get; } = [];
    public ObservableCollection<ComboItem> SpeciesList { get; } = [];

    public SearchViewModel(SaveFile sav)
    {
        _sav = sav;
        PopulateCombo(SpeciesList, GameInfo.FilteredSources.Species);
    }

    private static void PopulateCombo(ObservableCollection<ComboItem> target, IReadOnlyList<ComboItem> source)
    {
        target.Clear();
        target.Add(new ComboItem("(Any)", 0));
        foreach (var item in source)
            target.Add(item);
    }

    [RelayCommand]
    private void Search()
    {
        Results.Clear();

        for (int box = 0; box < _sav.BoxCount; box++)
        {
            for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
            {
                var pk = _sav.GetBoxSlotAtIndex(box, slot);
                if (pk.Species == 0) continue;
                if (!MatchesFilter(pk)) continue;

                Results.Add(new SearchResultViewModel(pk, _sav, box, slot));
            }
        }

        ResultCount = Results.Count;
    }

    private bool MatchesFilter(PKM pk)
    {
        if (FilterSpecies > 0 && pk.Species != FilterSpecies)
            return false;
        if (FilterShinyOnly && !pk.IsShiny)
            return false;
        if (!string.IsNullOrEmpty(SearchText))
        {
            var name = SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, pk.Format);
            if (!name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                && !pk.Nickname.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        FilterSpecies = 0;
        FilterShinyOnly = false;
        Results.Clear();
        ResultCount = 0;
    }
}

public partial class SearchResultViewModel : ObservableObject
{
    [ObservableProperty] private Bitmap? _sprite;
    [ObservableProperty] private string _summary = string.Empty;
    [ObservableProperty] private string _location = string.Empty;

    public PKM Pokemon { get; }
    public int Box { get; }
    public int Slot { get; }

    public SearchResultViewModel(PKM pk, SaveFile sav, int box, int slot)
    {
        Pokemon = pk;
        Box = box;
        Slot = slot;

        var species = SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, pk.Format);
        Summary = $"{species} Lv.{pk.CurrentLevel}";
        Location = $"Box {box + 1}, Slot {slot + 1}";
        Sprite = pk.Sprite(sav, box, slot).ToAvaloniaImage();
    }
}
