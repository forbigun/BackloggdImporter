namespace BackloggdImporter.Models.Settings;

internal record Config
{
    public required string SessionCookie { get; init; }
    public required string CsrfToken { get; init; }
    public required string UserId { get; init; }
}