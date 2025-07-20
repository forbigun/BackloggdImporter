namespace BackloggdImporter.Models.Backloggd;

/// <summary>
/// Result of processing a game entry.
/// </summary>
internal enum ProcessingResult
{
    /// <summary>
    /// Game not found in the database.
    /// </summary>
    GameNotFound,

    /// <summary>
    /// Game log successfully created.
    /// </summary>
    Success,

    /// <summary>
    /// Error occurred while creating the game log.
    /// </summary>
    Failed,

    /// <summary>
    /// Game entry skipped due to invalid rating (0 or > 10).
    /// </summary>
    InvalidRating,

    /// <summary>
    /// Log already exists.
    /// </summary>
    AlreadyExists,

    /// <summary>
    /// Game log could not be created due to insufficient data
    /// </summary>
    InsufficientData
}