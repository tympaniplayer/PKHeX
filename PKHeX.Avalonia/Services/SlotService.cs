using System;
using System.IO;
using PKHeX.Core;

namespace PKHeX.Avalonia.Services;

/// <summary>
/// Service for clipboard and file-based PKM slot operations.
/// </summary>
public static class SlotService
{
    private static PKM? _clipboard;

    /// <summary>
    /// Copies a PKM to the internal clipboard.
    /// </summary>
    public static void CopySlot(PKM pk)
    {
        _clipboard = pk.Clone();
    }

    /// <summary>
    /// Returns the PKM on the clipboard, or null.
    /// </summary>
    public static PKM? PasteSlot() => _clipboard?.Clone();

    /// <summary>
    /// Whether the clipboard has a PKM.
    /// </summary>
    public static bool HasClipboard => _clipboard is not null;

    /// <summary>
    /// Exports a PKM to a file.
    /// </summary>
    public static void ExportPKM(PKM pk, string path)
    {
        var data = pk.DecryptedPartyData;
        File.WriteAllBytes(path, data);
    }

    /// <summary>
    /// Imports a PKM from a file.
    /// </summary>
    public static PKM? ImportPKM(string path, SaveFile sav)
    {
        var data = File.ReadAllBytes(path);
        var pk = EntityFormat.GetFromBytes(data);
        if (pk is null)
            return null;

        // Convert to the save file's format if needed
        if (pk.GetType() != sav.PKMType)
        {
            var converted = EntityConverter.ConvertToType(pk, sav.PKMType, out _);
            return converted;
        }
        return pk;
    }

    /// <summary>
    /// Deletes (clears) a box slot.
    /// </summary>
    public static void DeleteSlot(SaveFile sav, int box, int slot)
    {
        sav.SetBoxSlotAtIndex(sav.BlankPKM, box, slot);
    }

    /// <summary>
    /// Sets a PKM into a box slot.
    /// </summary>
    public static void SetSlot(SaveFile sav, PKM pk, int box, int slot)
    {
        sav.SetBoxSlotAtIndex(pk, box, slot);
    }

    /// <summary>
    /// Swaps two box slots.
    /// </summary>
    public static void SwapSlots(SaveFile sav, int box1, int slot1, int box2, int slot2)
    {
        var pk1 = sav.GetBoxSlotAtIndex(box1, slot1);
        var pk2 = sav.GetBoxSlotAtIndex(box2, slot2);
        sav.SetBoxSlotAtIndex(pk2, box1, slot1);
        sav.SetBoxSlotAtIndex(pk1, box2, slot2);
    }
}
