using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Services;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSaveFile))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private SaveFile? _saveFile;

    [ObservableProperty] private BoxViewModel? _boxViewModel;
    [ObservableProperty] private PartyViewModel? _partyViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEditor))]
    private PokemonEditorViewModel? _editorViewModel;

    [ObservableProperty] private string _statusMessage = "No save file loaded.";
    [ObservableProperty] private int _currentBox;

    // Active sub-editor panel (shown in center content area)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSubEditorOpen))]
    private string? _activeSubEditor;

    [ObservableProperty] private TrainerEditorViewModel? _trainerEditor;
    [ObservableProperty] private InventoryViewModel? _inventoryEditor;
    [ObservableProperty] private MysteryGiftViewModel? _mysteryGiftEditor;
    [ObservableProperty] private SearchViewModel? _searchEditor;
    [ObservableProperty] private SettingsViewModel _settings = new();

    public bool HasSaveFile => SaveFile is not null;
    public bool HasEditor => EditorViewModel is not null;
    public bool IsSubEditorOpen => ActiveSubEditor is not null;

    public string Title => SaveFile is not null
        ? $"PKHeX (Avalonia) — {SaveFile.Version} [{SaveFile.OT}]"
        : "PKHeX (Avalonia)";

    partial void OnSaveFileChanged(SaveFile? value)
    {
        if (value is null)
        {
            BoxViewModel = null;
            PartyViewModel = null;
            EditorViewModel = null;
            ActiveSubEditor = null;
            StatusMessage = "No save file loaded.";
            return;
        }

        SpriteUtil.Initialize(value);
        CurrentBox = 0;
        BoxViewModel = new BoxViewModel(value, CurrentBox);
        PartyViewModel = new PartyViewModel(value);
        EditorViewModel = null;
        ActiveSubEditor = null;
        StatusMessage = $"Loaded {value.Version} — {value.OT} ({value.GetType().Name})";
    }

    partial void OnCurrentBoxChanged(int value)
    {
        if (SaveFile is not null)
            BoxViewModel = new BoxViewModel(SaveFile, value);
    }

    // ===== File Commands =====

    [RelayCommand]
    private async Task OpenFileAsync(Window window)
    {
        var storage = window.StorageProvider;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Save File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Save Files") { Patterns = ["*"] },
            ],
        });

        if (files.Count == 0)
            return;

        var file = files[0];
        var path = file.TryGetLocalPath();
        if (path is null)
        {
            StatusMessage = "Cannot read non-local files.";
            return;
        }

        try
        {
            var sav = SaveUtil.GetSaveFile(path);
            if (sav is null)
            {
                StatusMessage = $"Unrecognized save file: {System.IO.Path.GetFileName(path)}";
                return;
            }
            SaveFile = sav;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SaveCurrentFile()
    {
        if (SaveFile is null)
            return;

        var data = SaveFile.Write();
        var path = SaveFile.Metadata.FilePath;
        if (path is null)
        {
            StatusMessage = "No file path associated with save.";
            return;
        }

        try
        {
            System.IO.File.WriteAllBytes(path, data);
            StatusMessage = $"Saved to {System.IO.Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    // ===== Box Navigation =====

    [RelayCommand]
    private void NextBox()
    {
        if (SaveFile is null) return;
        CurrentBox = (CurrentBox + 1) % SaveFile.BoxCount;
    }

    [RelayCommand]
    private void PreviousBox()
    {
        if (SaveFile is null) return;
        CurrentBox = (CurrentBox - 1 + SaveFile.BoxCount) % SaveFile.BoxCount;
    }

    // ===== Pokemon Editor =====

    [RelayCommand]
    private void SelectSlot(SlotViewModel? slot)
    {
        if (slot is null || SaveFile is null || slot.IsEmpty)
            return;

        ActiveSubEditor = null;
        EditorViewModel = new PokemonEditorViewModel(slot.Pokemon, SaveFile);
        StatusMessage = $"Editing: {slot.Summary}";
    }

    [RelayCommand]
    private void CloseEditor()
    {
        EditorViewModel = null;
        if (SaveFile is not null)
        {
            BoxViewModel = new BoxViewModel(SaveFile, CurrentBox);
            PartyViewModel = new PartyViewModel(SaveFile);
        }
    }

    // ===== Sub-Editor Navigation =====

    [RelayCommand]
    private void OpenTrainerEditor()
    {
        if (SaveFile is null) return;
        EditorViewModel = null;
        TrainerEditor = new TrainerEditorViewModel(SaveFile);
        ActiveSubEditor = "Trainer";
        StatusMessage = "Editing trainer info...";
    }

    [RelayCommand]
    private void OpenInventory()
    {
        if (SaveFile is null) return;
        EditorViewModel = null;
        InventoryEditor = new InventoryViewModel(SaveFile);
        ActiveSubEditor = "Inventory";
        StatusMessage = "Viewing inventory...";
    }

    [RelayCommand]
    private void OpenMysteryGifts()
    {
        if (SaveFile is null) return;
        EditorViewModel = null;
        MysteryGiftEditor = new MysteryGiftViewModel(SaveFile);
        ActiveSubEditor = "MysteryGifts";
        StatusMessage = "Viewing mystery gifts...";
    }

    [RelayCommand]
    private void OpenSearch()
    {
        if (SaveFile is null) return;
        EditorViewModel = null;
        SearchEditor = new SearchViewModel(SaveFile);
        ActiveSubEditor = "Search";
        StatusMessage = "Search across all boxes...";
    }

    [RelayCommand]
    private void CloseSubEditor()
    {
        ActiveSubEditor = null;
        TrainerEditor = null;
        InventoryEditor = null;
        MysteryGiftEditor = null;
        SearchEditor = null;
        StatusMessage = HasSaveFile ? "Ready." : "No save file loaded.";
    }

    // ===== Slot Context Menu Commands =====

    [RelayCommand]
    private void CopySlot(SlotViewModel? slot)
    {
        if (slot is null || slot.IsEmpty) return;
        SlotService.CopySlot(slot.Pokemon);
        StatusMessage = $"Copied {slot.Summary} to clipboard.";
    }

    [RelayCommand]
    private void PasteSlot(SlotViewModel? slot)
    {
        if (slot is null || SaveFile is null || !SlotService.HasClipboard) return;
        var pk = SlotService.PasteSlot();
        if (pk is null) return;

        // Convert if needed
        if (pk.GetType() != SaveFile.PKMType)
        {
            var converted = EntityConverter.ConvertToType(pk, SaveFile.PKMType, out _);
            if (converted is null) { StatusMessage = "Conversion failed."; return; }
            pk = converted;
        }

        SlotService.SetSlot(SaveFile, pk, CurrentBox, slot.Pokemon.Species == 0 ? FindSlotIndex(slot) : FindSlotIndex(slot));
        BoxViewModel = new BoxViewModel(SaveFile, CurrentBox);
        StatusMessage = "Pasted from clipboard.";
    }

    [RelayCommand]
    private void DeleteSlot(SlotViewModel? slot)
    {
        if (slot is null || slot.IsEmpty || SaveFile is null) return;
        var index = FindSlotIndex(slot);
        if (index < 0) return;
        SlotService.DeleteSlot(SaveFile, CurrentBox, index);
        BoxViewModel = new BoxViewModel(SaveFile, CurrentBox);
        StatusMessage = $"Deleted {slot.Summary}.";
    }

    private int FindSlotIndex(SlotViewModel slot)
    {
        if (BoxViewModel is null) return -1;
        for (int i = 0; i < BoxViewModel.Slots.Count; i++)
        {
            if (ReferenceEquals(BoxViewModel.Slots[i], slot))
                return i;
        }
        return -1;
    }

    [RelayCommand]
    private async Task ExportSlotAsync(Window window)
    {
        if (EditorViewModel is null || SaveFile is null) return;
        var pk = EditorViewModel.Entity;

        var storage = window.StorageProvider;
        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export PKM",
            SuggestedFileName = $"{pk.Species:000} - {pk.Nickname}.{pk.Extension}",
        });

        if (file is null) return;
        var path = file.TryGetLocalPath();
        if (path is null) return;

        SlotService.ExportPKM(pk, path);
        StatusMessage = $"Exported to {System.IO.Path.GetFileName(path)}";
    }

    [RelayCommand]
    private async Task ImportSlotAsync(Window window)
    {
        if (SaveFile is null) return;

        var storage = window.StorageProvider;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import PKM",
            AllowMultiple = false,
        });

        if (files.Count == 0) return;
        var path = files[0].TryGetLocalPath();
        if (path is null) return;

        var pk = SlotService.ImportPKM(path, SaveFile);
        if (pk is null) { StatusMessage = "Failed to import PKM."; return; }

        EditorViewModel = new PokemonEditorViewModel(pk, SaveFile);
        StatusMessage = $"Imported {pk.Nickname}. Click Apply to save to slot.";
    }
}
