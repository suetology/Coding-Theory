namespace Code.Configuration;

/// <summary>
/// Klasė, kuri atitinka pranešimo nustatymus, nuskaitomus iš 'configuration.json' failo.
/// </summary>
public class MessageConfiguration 
{
    public string? Type { get;set; }

    public string? Content { get;set; }
}