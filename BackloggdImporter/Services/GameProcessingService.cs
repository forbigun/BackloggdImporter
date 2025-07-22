using System;
using System.Threading.Tasks;
using BackloggdImporter.Models.Backloggd;
using BackloggdImporter.Models.Csv;
using BackloggdImporter.Models.Settings;

namespace BackloggdImporter.Services;

/// <summary>
/// Service for processing game entries - searching and creating logs.
/// </summary>
internal class GameProcessingService(Config config) : IDisposable
{
    private readonly BacklogService _backlogService = new(config);

    /// <summary>
    /// Processes a single game entry: searches for the game and creates a log.
    /// </summary>
    /// <param name="entry">Game entry from CSV</param>
    /// <returns>Result of game processing</returns>
    public async Task<ProcessingResult> ProcessGameEntryAsync(CsvGameEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.Rating is <= 0 or > 10)
        {
            return ProcessingResult.InvalidRating;
        }

        var gameId = await _backlogService.FindGameIdAsync(entry.Name, entry.ReleaseYear);
        if (!gameId.HasValue)
        {
            return ProcessingResult.GameNotFound;
        }

        var hasLog = await _backlogService.HasGameLogAsync(gameId.Value);
        if (hasLog)
        {
            return ProcessingResult.AlreadyExists;
        }

        var request = GameLogRequest.FromCsvEntry(gameId.Value, entry);
        var isSuccess = await _backlogService.CreateGameLogAsync(request);

        return isSuccess
                   ? ProcessingResult.Success
                   : ProcessingResult.Failed;
    }

    public void Dispose()
    {
        _backlogService.Dispose();
    }
}