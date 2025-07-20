using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BackloggdImporter.Constants;
using BackloggdImporter.Models.Backloggd;
using BackloggdImporter.Models.Settings;

namespace BackloggdImporter.Services;

internal class BacklogService(Config config) : IDisposable
{
    private readonly HttpClient _httpClient = HttpClientFactory.Create(config);

    /// <summary>
    /// Searches for a game ID by its title.
    /// </summary>
    /// <param name="title">Game title</param>
    /// <param name="releaseYear">Release year</param>
    public async Task<int?> FindGameIdAsync(string title, int? releaseYear)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var encodedTitle = Uri.EscapeDataString(title);
        var url = $"autocomplete.json?query={encodedTitle}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var searchResponse = await response.Content.ReadFromJsonAsync<GameSearchResult>();

        return releaseYear.HasValue
                   ? searchResponse?.Suggestions
                       .FirstOrDefault(s => !s.Data.Year.HasValue || s.Data.Year == releaseYear)
                       ?.Data.Id
                   : searchResponse?.Suggestions.FirstOrDefault()?.Data.Id;
    }

    /// <summary>
    /// Creates a log for a game
    /// </summary>
    /// <param name="request">Game log creation request</param>
    public async Task<bool> CreateGameLogAsync(GameLogRequest request)
    {
        var formData = BuildFormData(request);
        using var body = new FormUrlEncodedContent(formData);

        var url = $"api/user/{config.UserId}/log/{request.GameId}";
        var response = await _httpClient.PostAsync(url, body);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Checks if a log has already been created for the game with the specified <paramref name="gameId"/>.
    /// </summary>
    /// <param name="gameId">Game ID</param>
    public async Task<bool> HasGameLogAsync(int gameId)
    {
        if (gameId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(gameId), "Game ID must be a positive integer.");
        }

        var url = $"log/edit/{gameId}";

        var logData = await _httpClient.GetFromJsonAsync<LogData>(url);

        return logData!.Playthroughs.Count > 0;
    }

    private static Dictionary<string, string?> BuildFormData(GameLogRequest request)
    {
        return new Dictionary<string, string?>
        {
            ["game_id"] = request.GameId.ToString(),
            ["playthroughs[0][id]"] = BackloggdConstants.DefaultNewLogId.ToString(),
            ["playthroughs[0][title]"] = $"{request.GameName} Log",
            ["playthroughs[0][rating]"] = request.Rating.ToString(),
            ["playthroughs[0][review]"] = request.Review,
            ["playthroughs[0][platform]"] = request.PlatformId ?? string.Empty,
            ["playthroughs[0][sync_sessions]"] = "true",
            ["playthroughs[0][is_master]"] = "false",
            ["playthroughs[0][is_replay]"] = "false",
            ["log[is_play]"] = request.IsPlay.ToString().ToLower(),
            ["log[is_playing]"] = request.IsPlaying.ToString().ToLower(),
            ["log[is_backlog]"] = request.IsBacklog.ToString().ToLower(),
            ["log[is_wishlist]"] = request.IsWishlist.ToString().ToLower(),
            ["log[status]"] = request.Status,
            ["modal_type"] = "quick"
        };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}