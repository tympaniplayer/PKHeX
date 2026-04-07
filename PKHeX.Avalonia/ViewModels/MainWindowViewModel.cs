using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSaveFile))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private SaveFile? _saveFile;

    [ObservableProperty]
    private BoxViewModel? _boxViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEditor))]
    private PokemonEditorViewModel? _editorViewModel;

    [ObservableProperty]
    private string _statusMessage = "No save file loaded.";

    [ObservableProperty]
    private int _currentBox;

    public bool HasSaveFile => SaveFile is not null;
    public bool HasEditor => EditorViewModel is not null;

    public string Title => SaveFile is not null
        ? $"PKHeX (Avalonia) — {SaveFile.Version} [{SaveFile.OT}]"
        : "PKHeX (Avalonia)";

    partial void OnSaveFileChanged(SaveFile? value)
    {
        if (value is null)
        {
            BoxViewModel = null;
            StatusMessage = "No save file loaded.";
            return;
        }

        SpriteUtil.Initialize(value);
        CurrentBox = 0;
        BoxViewModel = new BoxViewModel(value, CurrentBox);
        StatusMessage = $"Loaded {value.Version} — {value.OT} ({value.GetType().Name})";
    }

    partial void OnCurrentBoxChanged(int value)
    {
        if (SaveFile is not null)
            BoxViewModel = new BoxViewModel(SaveFile, value);
    }

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

    [RelayCommand]
    private void SelectSlot(SlotViewModel? slot)
    {
        if (slot is null || SaveFile is null || slot.IsEmpty)
            return;

        EditorViewModel = new PokemonEditorViewModel(slot.Pokemon, SaveFile);
        StatusMessage = $"Editing: {slot.Summary}";
    }

    [RelayCommand]
    private void CloseEditor()
    {
        EditorViewModel = null;
        // Refresh box to show any changes
        if (SaveFile is not null)
            BoxViewModel = new BoxViewModel(SaveFile, CurrentBox);
    }
}
