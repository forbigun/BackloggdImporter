using System;
using BackloggdImporter.Constants;
using BackloggdImporter.Models.Csv;

namespace BackloggdImporter.Models.Backloggd;

/// <summary>
/// Represents a request to create a game log in the Backloggd system.
/// Contains all necessary data for creating a playthrough record.
/// </summary>
internal record GameLogRequest
{
    /// <summary>
    /// Unique game identifier in the Backloggd system.
    /// </summary>
    public required int GameId { get; init; }

    /// <summary>
    /// Game title for display in the log.
    /// </summary>
    public required string GameName { get; init; }

    /// <summary>
    /// Game rating in the Backloggd system (usually from 0 to 10).
    /// </summary>
    public int? Rating { get; init; }

    /// <summary>
    /// Playthrough status
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Platform ID where the game was played. Can be null if not specified.
    /// </summary>
    public string? PlatformId { get; init; }

    /// <summary>
    /// Review text for the game
    /// </summary>
    public string? Review { get; init; }

    /// <summary>
    /// Indicates that the game has been completed
    /// </summary>
    public bool IsPlay { get; init; }

    /// <summary>
    /// Indicates that the game is currently being played
    /// </summary>
    public bool IsPlaying { get; init; }

    /// <summary>
    /// Indicates that the game is on the backlog to be played
    /// </summary>
    public bool IsBacklog { get; init; }

    /// <summary>
    /// Indicates that the game is on the wishlist
    /// </summary>
    public bool IsWishlist { get; init; }

    /// <summary>
    /// Creates a request to log a game based on CSV data.
    /// </summary>
    /// <param name="gameId">Game identifier</param>
    /// <param name="csvGame">Game data from the CSV file</param>
    /// <returns>New instance of GameLogRequest</returns>
    public static GameLogRequest FromCsvEntry(int gameId, CsvGameEntry csvGame)
    {
        ArgumentNullException.ThrowIfNull(csvGame);

        var platformId = string.IsNullOrEmpty(csvGame.Platform)
                             ? null
                             : Platforms.GetId(csvGame.Platform);
        var status = string.IsNullOrEmpty(csvGame.Status)
                         ? GameStatuses.Completed
                         : GameStatuses.IsValidStatus(csvGame.Status)
                             ? csvGame.Status
                             : GameStatuses.Completed;

        return new GameLogRequest
        {
            GameId = gameId,
            GameName = csvGame.Name,
            Rating = csvGame.Rating,
            Review = csvGame.Review,
            PlatformId = platformId,
            Status = status,
            IsPlay = status is GameStatuses.Completed or GameStatuses.Played or GameStatuses.Shelved
                               or GameStatuses.Abandoned or GameStatuses.Retired,
            IsPlaying = status == GameStatuses.Playing,
            IsBacklog = status == GameStatuses.Backlog,
            IsWishlist = status == GameStatuses.Wishlist
        };
    }
}