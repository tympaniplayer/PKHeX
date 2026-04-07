using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the Trainer Info editor.
/// </summary>
public partial class TrainerEditorViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty] private string _trainerName;
    [ObservableProperty] private ushort _tID16;
    [ObservableProperty] private ushort _sID16;
    [ObservableProperty] private uint _money;
    [ObservableProperty] private int _playedHours;
    [ObservableProperty] private int _playedMinutes;
    [ObservableProperty] private int _playedSeconds;
    [ObservableProperty] private int _language;
    [ObservableProperty] private byte _gender;

    public string GameVersion { get; }
    public string SaveType { get; }

    public TrainerEditorViewModel(SaveFile sav)
    {
        _sav = sav;
        GameVersion = sav.Version.ToString();
        SaveType = sav.GetType().Name;

        _trainerName = sav.OT;
        _tID16 = sav.TID16;
        _sID16 = sav.SID16;
        _money = (uint)sav.Money;
        _playedHours = sav.PlayedHours;
        _playedMinutes = sav.PlayedMinutes;
        _playedSeconds = sav.PlayedSeconds;
        _language = sav.Language;
        _gender = sav.Gender;
    }

    [RelayCommand]
    private void Apply()
    {
        _sav.OT = TrainerName;
        _sav.TID16 = TID16;
        _sav.SID16 = SID16;
        _sav.Money = (int)Money;
        _sav.PlayedHours = (ushort)PlayedHours;
        _sav.PlayedMinutes = (byte)PlayedMinutes;
        _sav.PlayedSeconds = (byte)PlayedSeconds;
        _sav.Language = Language;
        _sav.Gender = Gender;
    }
}
