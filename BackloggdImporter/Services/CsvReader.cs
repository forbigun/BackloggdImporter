using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BackloggdImporter.Models.Csv;
using CsvHelper.Configuration;

namespace BackloggdImporter.Services;

/// <summary>
/// Service for reading game data from a CSV file for import.
/// </summary>
internal static class CsvReader
{
    /// <summary>
    /// Reads game entries from a CSV file.
    /// </summary>
    /// <param name="filePath">Path to the CSV file</param>
    /// <returns>Asynchronous enumeration of game entries</returns>
    /// <exception cref="FileNotFoundException">If the file is not found</exception>
    /// <exception cref="ArgumentException">If the file path is empty or invalid</exception>
    public static async IAsyncEnumerable<CsvGameEntry> ReadGameEntriesAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {filePath} not found.", filePath);
        }

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            DetectDelimiter = true,
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvHelper.CsvReader(reader, csvConfig);

        await foreach (var entry in csv.GetRecordsAsync<CsvGameEntry>())
        {
            yield return entry;
        }
    }
}