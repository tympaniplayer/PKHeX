using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// Application settings for the Avalonia UI.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isDarkMode;
    [ObservableProperty] private SpriteBuilderMode _spriteMode = SpriteBuilderMode.SpritesClassic5668;
    [ObservableProperty] private bool _showLegalityIndicator = true;
    [ObservableProperty] private bool _showEncounterColor = true;
    [ObservableProperty] private string _language = "en";

    partial void OnSpriteModeChanged(SpriteBuilderMode value)
    {
        SpriteUtil.ChangeMode(value);
    }
}
