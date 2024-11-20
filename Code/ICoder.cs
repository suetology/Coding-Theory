namespace Code;

/// <summary>
/// Bendras interfeisas, skirtas kodavimui ir dekodavimui.
/// </summary>
public interface ICoder
{
    /// <summary>
    /// Koduoja BitMessage pranešimą.
    /// </summary>
    /// <param name="message">Pradinis pranešimas</param>
    /// <returns>Užkoduotas pranešimas</returns>
    BitMessage Encode(BitMessage message);

    /// <summary>
    /// Dekoduoja BitMessage pranešimą.
    /// </summary>
    /// <param name="message">Užkoduotas pranešimas</param>
    /// <returns>Dekoduotas pranešimas</returns>
    BitMessage Decode(BitMessage message);
}