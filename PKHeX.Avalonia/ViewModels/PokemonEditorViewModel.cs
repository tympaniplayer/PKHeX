using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the Pokémon editor panel. Wraps a PKM entity and exposes
/// editable properties organized by tab (Main, Met, Stats, Moves, OT, Cosmetic).
/// </summary>
public partial class PokemonEditorViewModel : ObservableObject
{
    private PKM _entity;
    private readonly SaveFile _sav;
    private bool _loading;

    public PokemonEditorViewModel(PKM pk, SaveFile sav)
    {
        _entity = pk;
        _sav = sav;
        LoadStrings();
        LoadFromEntity();
    }

    public PKM Entity => _entity;

    // ===== Sprite Preview =====
    [ObservableProperty] private Bitmap? _spritePreview;

    // ===== Main Tab =====
    [ObservableProperty] private ushort _species;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private byte _form;
    [ObservableProperty] private int _abilityIndex;
    [ObservableProperty] private int _natureIndex;
    [ObservableProperty] private int _heldItem;
    [ObservableProperty] private byte _level;
    [ObservableProperty] private uint _exp;
    [ObservableProperty] private byte _friendship;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private bool _isEgg;

    // ===== Met Tab =====
    [ObservableProperty] private ushort _metLocation;
    [ObservableProperty] private int _ball;
    [ObservableProperty] private int _metLevel;
    [ObservableProperty] private int _version;

    // ===== Stats Tab =====
    [ObservableProperty] private int _iv_HP;
    [ObservableProperty] private int _iv_ATK;
    [ObservableProperty] private int _iv_DEF;
    [ObservableProperty] private int _iv_SPA;
    [ObservableProperty] private int _iv_SPD;
    [ObservableProperty] private int _iv_SPE;
    [ObservableProperty] private int _eV_HP;
    [ObservableProperty] private int _eV_ATK;
    [ObservableProperty] private int _eV_DEF;
    [ObservableProperty] private int _eV_SPA;
    [ObservableProperty] private int _eV_SPD;
    [ObservableProperty] private int _eV_SPE;

    // ===== Moves Tab =====
    [ObservableProperty] private int _move1;
    [ObservableProperty] private int _move2;
    [ObservableProperty] private int _move3;
    [ObservableProperty] private int _move4;
    [ObservableProperty] private int _move1_PP;
    [ObservableProperty] private int _move2_PP;
    [ObservableProperty] private int _move3_PP;
    [ObservableProperty] private int _move4_PP;

    // ===== OT Tab =====
    [ObservableProperty] private string _oT_Name = string.Empty;
    [ObservableProperty] private ushort _tID16;
    [ObservableProperty] private ushort _sID16;
    [ObservableProperty] private byte _oT_Gender;
    [ObservableProperty] private int _language;

    // ===== Dropdown sources =====
    public ObservableCollection<ComboItem> SpeciesList { get; } = [];
    public ObservableCollection<ComboItem> NatureList { get; } = [];
    public ObservableCollection<ComboItem> HeldItemList { get; } = [];
    public ObservableCollection<ComboItem> AbilityList { get; } = [];
    public ObservableCollection<ComboItem> MoveList { get; } = [];
    public ObservableCollection<ComboItem> BallList { get; } = [];
    public ObservableCollection<ComboItem> VersionList { get; } = [];
    public ObservableCollection<ComboItem> LanguageList { get; } = [];

    public string[] GenderSymbols => ["♂", "♀", "-"];

    // ===== Computed =====
    public string StatSummary
    {
        get
        {
            var pi = _entity.PersonalInfo;
            var hp  = _entity.Stat_HPCurrent;
            var atk = _entity.Stat_ATK;
            var def = _entity.Stat_DEF;
            var spa = _entity.Stat_SPA;
            var spd = _entity.Stat_SPD;
            var spe = _entity.Stat_SPE;
            return $"HP:{hp}  Atk:{atk}  Def:{def}  SpA:{spa}  SpD:{spd}  Spe:{spe}  BST:{pi.GetBaseStatTotal()}";
        }
    }

    public int MaxIV => _entity.MaxIV;
    public int MaxEV => _entity.MaxEV;

    private void LoadStrings()
    {
        var strings = GameInfo.Strings;
        var filtered = GameInfo.FilteredSources;

        PopulateCombo(SpeciesList, filtered.Species);
        PopulateCombo(NatureList, filtered.Natures);
        PopulateCombo(HeldItemList, filtered.Items);
        PopulateCombo(MoveList, filtered.Moves);
        PopulateCombo(BallList, filtered.Balls);
        PopulateCombo(VersionList, filtered.Games);
        PopulateCombo(LanguageList, GameInfo.LanguageDataSource(_entity.Format, _entity.Context));
    }

    private static void PopulateCombo(ObservableCollection<ComboItem> target, IReadOnlyList<ComboItem> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(item);
    }

    private void RefreshAbilities()
    {
        AbilityList.Clear();
        var pi = _entity.PersonalInfo;
        var names = GameInfo.Strings.abilitylist;
        for (int i = 0; i < pi.AbilityCount; i++)
        {
            var abilIndex = pi.GetAbilityAtIndex(i);
            var name = abilIndex < names.Length ? names[abilIndex] : abilIndex.ToString();
            AbilityList.Add(new ComboItem(name, i));
        }
    }

    /// <summary>
    /// Loads all ViewModel properties from the backing PKM entity.
    /// </summary>
    public void LoadFromEntity()
    {
        _loading = true;
        try
        {
            var pk = _entity;

            // Main
            Species = pk.Species;
            Nickname = pk.Nickname;
            Form = pk.Form;
            RefreshAbilities();
            AbilityIndex = pk.AbilityNumber >> 1; // 1/2/4 → 0/1/2
            NatureIndex = (int)pk.Nature;
            HeldItem = pk.HeldItem;
            Level = (byte)pk.CurrentLevel;
            Exp = pk.EXP;
            Friendship = (byte)pk.CurrentFriendship;
            IsShiny = pk.IsShiny;
            IsEgg = pk.IsEgg;

            // Met
            MetLocation = pk.MetLocation;
            Ball = pk.Ball;
            MetLevel = pk.MetLevel;
            Version = (int)pk.Version;

            // Stats
            IV_HP = pk.IV_HP; IV_ATK = pk.IV_ATK; IV_DEF = pk.IV_DEF;
            IV_SPA = pk.IV_SPA; IV_SPD = pk.IV_SPD; IV_SPE = pk.IV_SPE;
            EV_HP = pk.EV_HP; EV_ATK = pk.EV_ATK; EV_DEF = pk.EV_DEF;
            EV_SPA = pk.EV_SPA; EV_SPD = pk.EV_SPD; EV_SPE = pk.EV_SPE;

            // Moves
            Move1 = pk.Move1; Move2 = pk.Move2; Move3 = pk.Move3; Move4 = pk.Move4;
            Move1_PP = pk.Move1_PP; Move2_PP = pk.Move2_PP; Move3_PP = pk.Move3_PP; Move4_PP = pk.Move4_PP;

            // OT
            OT_Name = pk.OriginalTrainerName;
            TID16 = pk.TID16; SID16 = pk.SID16;
            OT_Gender = pk.OriginalTrainerGender;
            Language = pk.Language;

            // Sprite
            RefreshSprite();
        }
        finally
        {
            _loading = false;
        }
    }

    /// <summary>
    /// Writes all ViewModel properties back to the backing PKM entity.
    /// </summary>
    [RelayCommand]
    public void ApplyToEntity()
    {
        var pk = _entity;

        pk.Species = Species;
        pk.Nickname = Nickname;
        pk.Form = Form;
        pk.Nature = (Nature)NatureIndex;
        pk.StatNature = (Nature)NatureIndex;
        pk.HeldItem = HeldItem;
        pk.CurrentLevel = Level;
        pk.EXP = Exp;
        pk.CurrentFriendship = Friendship;
        pk.IsEgg = IsEgg;

        pk.MetLocation = MetLocation;
        pk.Ball = Ball;
        pk.MetLevel = MetLevel;
        pk.Version = (GameVersion)Version;

        pk.IV_HP = IV_HP; pk.IV_ATK = IV_ATK; pk.IV_DEF = IV_DEF;
        pk.IV_SPA = IV_SPA; pk.IV_SPD = IV_SPD; pk.IV_SPE = IV_SPE;
        pk.EV_HP = EV_HP; pk.EV_ATK = EV_ATK; pk.EV_DEF = EV_DEF;
        pk.EV_SPA = EV_SPA; pk.EV_SPD = EV_SPD; pk.EV_SPE = EV_SPE;

        pk.Move1 = (ushort)Move1; pk.Move2 = (ushort)Move2;
        pk.Move3 = (ushort)Move3; pk.Move4 = (ushort)Move4;
        pk.Move1_PP = Move1_PP; pk.Move2_PP = Move2_PP;
        pk.Move3_PP = Move3_PP; pk.Move4_PP = Move4_PP;

        pk.OriginalTrainerName = OT_Name;
        pk.TID16 = TID16; pk.SID16 = SID16;
        pk.OriginalTrainerGender = OT_Gender;
        pk.Language = Language;

        pk.RefreshChecksum();
        RefreshSprite();
    }

    [RelayCommand]
    private void SetMaxIVs()
    {
        IV_HP = IV_ATK = IV_DEF = IV_SPA = IV_SPD = IV_SPE = MaxIV;
    }

    [RelayCommand]
    private void ClearEVs()
    {
        EV_HP = EV_ATK = EV_DEF = EV_SPA = EV_SPD = EV_SPE = 0;
    }

    private void RefreshSprite()
    {
        var sprite = _entity.Sprite(_sav);
        SpritePreview = sprite.ToAvaloniaImage();
        OnPropertyChanged(nameof(StatSummary));
    }

    partial void OnSpeciesChanged(ushort value)
    {
        if (_loading) return;
        _entity.Species = value;
        RefreshAbilities();
        RefreshSprite();
    }

    partial void OnFormChanged(byte value)
    {
        if (_loading) return;
        _entity.Form = value;
        RefreshAbilities();
        RefreshSprite();
    }

    partial void OnIsShinyChanged(bool value)
    {
        if (_loading) return;
        if (value)
            CommonEdits.SetShiny(_entity);
        else
            CommonEdits.SetUnshiny(_entity);
        RefreshSprite();
    }
}
