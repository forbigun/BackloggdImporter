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

    public bool HasWrittenEntries { get; private set; }

    public CsvEntryWriter(string filePath)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        var streamWriter = new StreamWriter(filePath, append: false)
        {
            AutoFlush = true
        };
        
        _csvWriter = new CsvWriter(streamWriter, csvConfig);
        _csvWriter.WriteHeader<CsvGameEntry>();
        _csvWriter.NextRecord();
    }

    /// <summary>
    /// Appends a single game entry using
    /// </summary>
    /// <param name="entry">Game entry to append</param>
    public async Task AppendEntryAsync(CsvGameEntry entry)
    {
        _csvWriter.WriteRecord(entry);
        await _csvWriter.NextRecordAsync();

        if (!HasWrittenEntries)
        {
            HasWrittenEntries = true;
        }
    }

    public void Dispose()
    {
        _csvWriter.Dispose();
    }
}