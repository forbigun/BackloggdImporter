using System.Collections.Generic;

namespace BackloggdImporter.Models.Backloggd;

internal record LogData(IReadOnlyCollection<Playthrough> Playthroughs);