using SkiaSharp;
using PKHeX.Core;
using QRCoder;

namespace PKHeX.Drawing.Misc;

/// <summary>
/// Provides methods for generating QR codes from various PKHeX data types.
/// </summary>
public static class QREncode
{
    /// <summary>
    /// Generates a QR code bitmap from a <see cref="DataMysteryGift"/> object.
    /// </summary>
    public static SKBitmap GenerateQRCode(DataMysteryGift mg) => GenerateQRCode(QRMessageUtil.GetMessage(mg));

    /// <summary>
    /// Generates a QR code bitmap from a <see cref="PKM"/> object.
    /// </summary>
    public static SKBitmap GenerateQRCode(PKM pk) => GenerateQRCode(QRMessageUtil.GetMessage(pk));

    /// <summary>
    /// Generates a QR code bitmap for a Generation 7 PKM with additional options.
    /// </summary>
    public static SKBitmap GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int copies = 1)
        => GenerateQRCode(QRMessageUtil.GetMessage(pk7, box, slot, copies), ppm: 4);

    /// <summary>
    /// Generates a QR code bitmap from a message string.
    /// </summary>
    private static SKBitmap GenerateQRCode(string msg, int ppm = 4)
    {
        using var data = QRCodeGenerator.GenerateQrCode(msg, QRCodeGenerator.ECCLevel.Q);
        var code = new PngByteQRCode(data);
        var pngBytes = code.GetGraphic(ppm);
        return SKBitmap.Decode(pngBytes);
    }
}
