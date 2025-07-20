using System.Collections.Frozen;

namespace BackloggdImporter.Constants;

/// <summary>
/// Contains constants for game statuses in the Backloggd system.
/// </summary>
internal static class GameStatuses
{
    /// <summary>
    /// The game is fully completed
    /// </summary>
    public const string Completed = "completed";

    /// <summary>
    /// The game was played (but not necessarily completed)
    /// </summary>
    public const string Played = "played";

    /// <summary>
    /// Currently playing this game
    /// </summary>
    public const string Playing = "playing";

    /// <summary>
    /// The game is in backlog (planned to play)
    /// </summary>
    public const string Backlog = "backlog";

    /// <summary>
    /// The game is in wishlist (want to acquire)
    /// </summary>
    public const string Wishlist = "wishlist";

    /// <summary>
    /// The game is shelved (temporarily not playing, but plan to return)
    /// </summary>
    public const string Shelved = "shelved";

    /// <summary>
    /// The game is abandoned (do not plan to return)
    /// </summary>
    public const string Abandoned = "abandoned";

    /// <summary>
    /// The game is retired (finished all interaction with it)
    /// </summary>
    public const string Retired = "retired";

    /// <summary>
    /// All possible game statuses in the Backloggd system
    /// </summary>
    private static readonly FrozenSet<string> AllStatuses = new[]
    {
        Completed,
        Played,
        Playing,
        Backlog,
        Wishlist,
        Shelved,
        Abandoned,
        Retired
    }.ToFrozenSet();

    /// <summary>
    /// Checks if the status is valid.
    /// </summary>
    /// <param name="status">Status to check</param>
    /// <returns>true if the status is valid</returns>
    public static bool IsValidStatus(string status)
    {
        return AllStatuses.Contains(status);
    }
}