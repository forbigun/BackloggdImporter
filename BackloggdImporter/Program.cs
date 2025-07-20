using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BackloggdImporter.Constants;
using BackloggdImporter.Helpers;
using BackloggdImporter.Models.Backloggd;
using BackloggdImporter.Models.Settings;
using BackloggdImporter.Services;

var delayBetweenRequests = TimeSpan.FromSeconds(2);
var delayIfRateLimitExceeded = TimeSpan.FromMinutes(1);

const string configFileName = FileNames.Config;
const string csvFileName = FileNames.Csv;

if (!File.Exists(csvFileName))
{
    ConsolePrinter.WriteError($"File {csvFileName} not found.");

    return;
}

if (!File.Exists(configFileName))
{
    ConsolePrinter.WriteError($"Settings file {configFileName} not found.");
    return;
}

var configFileContent = await File.ReadAllTextAsync(configFileName);
var config = JsonSerializer.Deserialize<Config>(configFileContent)!;

using var gameProcessor = new GameProcessingService(config);

using var failedCsvWriter = CsvEntryWriter.CreateInstance(FileNames.FailedGamesCsv);

await foreach (var entry in CsvReader.ReadGameEntriesAsync(csvFileName))
{
    try
    {
        var result = await gameProcessor.ProcessGameEntryAsync(entry);

        switch (result)
        {
            case ProcessingResult.GameNotFound:
                ConsolePrinter.WriteWarning($"Not found: {entry.Name}");
                await failedCsvWriter.AppendEntryAsync(entry);
                break;
            case ProcessingResult.InvalidRating:
                ConsolePrinter.WriteWarning($"[Skipped] {entry.Name} — no rating");
                await failedCsvWriter.AppendEntryAsync(entry);
                break;
            case ProcessingResult.Success:
                ConsolePrinter.WriteSuccess($"{entry.Name} — game log successfully created");
                break;
            case ProcessingResult.Failed:
                ConsolePrinter.WriteError($"{entry.Name} — failed to create game log");
                await failedCsvWriter.AppendEntryAsync(entry);
                break;
            case ProcessingResult.AlreadyExists:
                ConsolePrinter.WriteWarning($"{entry.Name} — game log already exists. Skipping.");
                break;
        }
    }
    catch (HttpRequestException ex)
    {
        TimeSpan delay;
        if (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            ConsolePrinter.WriteWarning($"Rate limit exceeded. Retrying {entry.Name} after 1 minute...");
            delay = delayIfRateLimitExceeded;
        }
        else
        {
            ConsolePrinter.WriteError($"HTTP error {ex.StatusCode} for {entry.Name}: {ex.Message}");
            delay = delayBetweenRequests;
        }
        
        await failedCsvWriter.AppendEntryAsync(entry);
        await Task.Delay(delay);
    }
    catch (Exception ex)
    {
        ConsolePrinter.WriteError($"Error {entry.Name}: {ex.Message}");
        await failedCsvWriter.AppendEntryAsync(entry);
    }
}
