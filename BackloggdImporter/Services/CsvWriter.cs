using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using BackloggdImporter.Models.Csv;
using CsvHelper;
using CsvHelper.Configuration;

namespace BackloggdImporter.Services;

/// <summary>
/// Service for writing game data to CSV files.
/// </summary>
internal class CsvEntryWriter : IDisposable
{
    private readonly CsvWriter _csvWriter;

    private CsvEntryWriter(string filePath)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        var streamWriter = new StreamWriter(filePath, append: false);
        _csvWriter = new CsvWriter(streamWriter, csvConfig);
        _csvWriter.WriteHeader<CsvGameEntry>();
        _csvWriter.NextRecord();
    }

    public static CsvEntryWriter CreateInstance(string filePath)
    {
        return new CsvEntryWriter(filePath);
    }

    /// <summary>
    /// Appends a single game entry using
    /// </summary>
    /// <param name="entry">Game entry to append</param>
    public async Task AppendEntryAsync(CsvGameEntry entry)
    {
        _csvWriter.WriteRecord(entry);
        await _csvWriter.NextRecordAsync();
    }

    public void Dispose()
    {
        _csvWriter.Dispose();
    }
}