using System.Collections.Generic;

namespace BackloggdImporter.Models.Backloggd;

internal record GameSearchResult(IReadOnlyCollection<Suggestion> Suggestions);