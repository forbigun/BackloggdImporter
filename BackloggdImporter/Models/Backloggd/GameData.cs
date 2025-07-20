using System.Text.Json.Serialization;
using BackloggdImporter.JsonConverters;

namespace BackloggdImporter.Models.Backloggd;

internal record GameData(int Id, string Title, int? Year)
{
    [JsonConverter(typeof(NullableIntConverter))]
    public int? Year { get; init; } = Year;
}