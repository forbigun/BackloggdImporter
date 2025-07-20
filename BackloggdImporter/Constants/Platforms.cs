using System.Collections.Generic;

namespace BackloggdImporter.Constants;

/// <summary>
/// Contains a mapping of game platform names to their identifiers in the Backloggd system.
/// </summary>
internal static class Platforms
{
    private static readonly Dictionary<string, string> PlatformMap = new()
    {
        ["Linux"] = "3",
        ["Nintendo 64"] = "4",
        ["N64"] = "4",
        ["Wii"] = "5",
        ["PC"] = "6",
        ["Windows"] = "6",
        ["PlayStation"] = "7",
        ["PS1"] = "7",
        ["PlayStation 2"] = "8",
        ["PS2"] = "8",
        ["PlayStation 3"] = "9",
        ["PS3"] = "9",
        ["Xbox"] = "11",
        ["Xbox 360"] = "12",
        ["Mac"] = "14",
        ["Amiga"] = "16",
        ["NES"] = "18",
        ["SNES"] = "19",
        ["Nintendo DS"] = "20",
        ["DS"] = "20",
        ["GameCube"] = "21",
        ["Game Boy"] = "22",
        ["Game Boy Color"] = "23",
        ["Game Boy Advance"] = "24",
        ["GBA"] = "24",
        ["Amstrad CPC"] = "25",
        ["Atari 2600"] = "28",
        ["Sega Genesis"] = "29",
        ["Sega Saturn"] = "32",
        ["Android"] = "34",
        ["Nintendo 3DS"] = "37",
        ["3DS"] = "37",
        ["PSP"] = "38",
        ["iOS"] = "39",
        ["iPhone"] = "39",
        ["iPad"] = "39",
        ["Wii U"] = "41",
        ["PlayStation Vita"] = "46",
        ["Vita"] = "46",
        ["PlayStation 4"] = "48",
        ["PS4"] = "48",
        ["Xbox One"] = "49",
        ["Arcade"] = "52",
        ["Commodore 64"] = "64",
        ["SteamOS"] = "92",
        ["Dreamcast"] = "106",
        ["Nintendo Switch"] = "130",
        ["Switch"] = "130",
        ["PlayStation 5"] = "167",
        ["PS5"] = "167",
        ["Xbox Series X"] = "169",
        ["Xbox Series S"] = "169",
        ["Xbox Series"] = "169",
        ["Stadia"] = "170",
        ["Steam Deck"] = "171"
    };

    /// <summary>
    /// Returns the platform ID for the given platform name, or null if not found.
    /// </summary>
    /// <param name="platformName">Platform name</param>
    /// <returns>Platform ID as string or null</returns>
    public static string? GetId(string platformName)
    {
        return string.IsNullOrWhiteSpace(platformName)
                   ? null
                   : PlatformMap.GetValueOrDefault(platformName);
    }
}