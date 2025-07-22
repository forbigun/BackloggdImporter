using System;
using System.IO;
using System.Text.Json;
using BackloggdImporter.Constants;
using BackloggdImporter.Factories;
using BackloggdImporter.Helpers;
using BackloggdImporter.Models.Backloggd;
using BackloggdImporter.Models.Settings;
using BackloggdImporter.Services;

const string configFileName = FileNames.Config;
const string csvFileName = FileNames.Csv;

if (!File.Exists(csvFileName))
{
    ConsolePrinter.WriteError($"File {csvFileName} not found. Please provide a valid CSV file with game entries.");
    Console.ReadKey();
    return;
}

if (!File.Exists(configFileName))
{
    ConsolePrinter.WriteError($"Settings file {configFileName} not found. Please create it with your Backloggd settings.");
    Console.ReadKey();
    return;
}

var configFileContent = await File.ReadAllTextAsync(configFileName);
var config = JsonSerializer.Deserialize<Config>(configFileContent)!;

using var gameProcessor = new GameProcessingService(config);
using var failedCsvWriter = new CsvEntryWriter(FileNames.FailedGamesCsv);

var resiliencePipeline = PollyPipelineFactory.CreateResiliencePipeline();

await foreach (var entry in CsvReader.ReadGameEntriesAsync(csvFileName))
{
    try
    {
        var result = await resiliencePipeline.ExecuteAsync(
                         static async (state, _) =>
                         {
                             var (processor, gameEntry) = state;
                             return await processor.ProcessGameEntryAsync(gameEntry);
                         },
                         (gameProcessor, entry)
                     );
        
        switch (result)
        {
            case ProcessingResult.GameNotFound:
                ConsolePrinter.WriteWarning($"Not found: {entry.Name}");
                await failedCsvWriter.AppendEntryAsync(entry);
                break;
                
            case ProcessingResult.InvalidRating:
                ConsolePrinter.WriteWarning($"[Skipped] {entry.Name} — invalid rating ({entry.Rating}).");
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
                ConsolePrinter.WriteInfo($"{entry.Name} — game log already exists. Skipping.");
                break;
        }
    }
    catch (Exception ex)
    {
        ConsolePrinter.WriteError($"Final error for {entry.Name}: {ex.Message}");
        await failedCsvWriter.AppendEntryAsync(entry);
    }
}

Console.WriteLine();

if (failedCsvWriter.HasWrittenEntries)
{
    ConsolePrinter.WriteInfo($"Some game logs could not be created. Check {FileNames.FailedGamesCsv} for details.");
}

ConsolePrinter.WriteInfo("Processing complete. Press any key to exit.");
Console.ReadKey();
