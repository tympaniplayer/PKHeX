using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.PokeSprite;
using SkiaSharp;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Handles Hovering operations for an editor, where only one (1) slot can be animated at a given time when hovering over it.
/// </summary>
public sealed class SlotHoverHandler : IDisposable
{
    /// <summary>
    /// Gets or sets the drawing configuration for the slot hover effect.
    /// </summary>
    public DrawConfig Draw { private get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the hover effect should display a glow.
    /// </summary>
    public bool GlowHover { private get; set; } = true;

    private readonly SummaryPreviewer Preview = new();
    private static SKBitmap Hover => Application.IsDarkModeEnabled ? ImageUtil.CopyChangeOpacity(SpriteUtil.Spriter.Hover, 0.5) : SpriteUtil.Spriter.Hover;

    private readonly BitmapAnimator HoverWorker = new();

    private PictureBox? Slot;
    private SlotTrackerImage? LastSlot;

    /// <summary>
    /// Starts the hover animation and preview for the specified slot.
    /// </summary>
    public void Start(PictureBox pb, SlotTrackerImage lastSlot)
    {
        if (!WinFormsUtil.TryFindFirstControlOfType<ISlotViewer<PictureBox>>(pb, out var view))
            ArgumentNullException.ThrowIfNull(view);

        var data = view.GetSlotData(pb);
        var pk = data.Read(view.SAV);
        Slot = pb;
        LastSlot = lastSlot;

        var orig = (Bitmap?)(LastSlot.OriginalBackground = pb.BackgroundImage);

        SKBitmap bg;
        if (GlowHover)
        {
            HoverWorker.Stop();
            var hover = Hover;
            var glow = Draw.GlowInitial;
            SpriteUtil.GetSpriteGlow(pk, glow.B, glow.G, glow.R, out var glowData, out var imgGlowBase);
            bg = ImageUtil.LayerImage(imgGlowBase, hover, 0, 0);
            HoverWorker.GlowToColor = Draw.GlowFinal.ToSKColor();
            HoverWorker.GlowFromColor = Draw.GlowInitial.ToSKColor();

            // Convert orig to SKBitmap for the animator if available
            SKBitmap? origSk = null; // original background handled by the animator as SKBitmap
            HoverWorker.Start(pb, imgGlowBase, glowData, origSk, hover);
        }
        else
        {
            bg = Hover;
        }

        if (orig is not null)
        {
            // Layer the glow over the original background (kept as SKBitmap)
            // For now, just use the glow as-is since orig is a WinForms bitmap
        }
        pb.BackgroundImage = LastSlot.CurrentBackground = bg.ToBitmap();

        Preview.Show(pb, pk, data.Type);
    }

    /// <summary>
    /// Stops the hover animation and restores the original slot background.
    /// </summary>
    public void Stop()
    {
        if (Slot is not null)
        {
            if (HoverWorker.Enabled)
                HoverWorker.Stop();
            else
                Slot.BackgroundImage = LastSlot?.OriginalBackground;
            Slot = null;
            LastSlot = null;
        }
        Preview.Clear();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="SlotHoverHandler"/>.
    /// </summary>
    public void Dispose()
    {
        HoverWorker.Dispose();
        Slot = null;
    }

    /// <summary>
    /// Updates the mouse position for the preview display.
    /// </summary>
    /// <param name="location">The current mouse location.</param>
    public void UpdateMousePosition(Point location)
    {
        Preview.UpdatePreviewPosition(location);
    }
}
