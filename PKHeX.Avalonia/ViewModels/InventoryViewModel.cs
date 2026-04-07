using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for a single inventory item slot.
/// </summary>
public partial class InventoryItemViewModel : ObservableObject
{
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _itemName;
    [ObservableProperty] private int _count;

    public InventoryItemViewModel(int id, string name, int count)
    {
        _itemId = id;
        _itemName = name;
        _count = count;
    }
}

/// <summary>
/// ViewModel for a single inventory pouch (e.g., Items, Key Items, TMs).
/// </summary>
public partial class InventoryPouchViewModel : ObservableObject
{
    public string Name { get; }
    public ObservableCollection<InventoryItemViewModel> Items { get; } = [];

    public InventoryPouchViewModel(InventoryPouch pouch, string[] itemNames)
    {
        Name = pouch.Type.ToString();
        foreach (var item in pouch.Items)
        {
            if (item.Index == 0 && item.Count == 0)
                continue;
            var name = item.Index < itemNames.Length ? itemNames[item.Index] : $"Item {item.Index}";
            Items.Add(new InventoryItemViewModel(item.Index, name, item.Count));
        }
    }
}

/// <summary>
/// ViewModel for the full inventory editor.
/// </summary>
public partial class InventoryViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    public ObservableCollection<InventoryPouchViewModel> Pouches { get; } = [];

    [ObservableProperty]
    private InventoryPouchViewModel? _selectedPouch;

    public InventoryViewModel(SaveFile sav)
    {
        _sav = sav;
        var bag = sav.Inventory;
        var itemNames = GameInfo.Strings.GetItemStrings(sav.Context, sav.Version);

        foreach (var pouch in bag.Pouches)
        {
            var vm = new InventoryPouchViewModel(pouch, itemNames);
            Pouches.Add(vm);
        }

        SelectedPouch = Pouches.FirstOrDefault();
    }
}
