# BackloggdImporter

A console utility for importing game logs into [Backloggd](https://backloggd.com/) from a CSV file.

## Features

- Reads game data from a CSV file
- Automatically searches for games on Backloggd
- Creates logs for each game, including rating, platform, status, and review
- Built-in retry mechanism for handling rate limits and authentication issues

## How it works

- Loads user settings from `config.json`
- Reads a list of games from `backloggd_import.csv`
- For each game entry:
    - Searches for the game on Backloggd (using `ReleaseYear` if provided)
    - If the game is found and a log does not already exist, creates a new log with the specified parameters (rating, platform, status, review, etc.)
    - If the game is not found or the data is invalid (e.g., rating out of range), the entry is written to `failed_games.csv` for later review or retry
- At the end, generates a `failed_games.csv` file containing all entries that could not be imported

You can use `failed_games.csv` to retry only the failed imports later.

## Usage

### Running the project

You can either build the project from source or download a pre-built release.

#### Option 1: Using a pre-built release

1. Download the release archive from the [GitHub Releases](https://github.com/forbigun/BackloggdImporter/releases)
2. Extract the archive to a convenient folder
3. Navigate to the extracted folder
4. Complete the [Create configuration files](#create-configuration-files) step in the folder with the executable file
5. Run the program

**Available release types:**
- **Self-contained versions** - No .NET Runtime required, larger file size
- **Framework-dependent versions** - Require [.NET 9.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) to be installed, smaller file size

#### Option 2: Building from source

**Requires:** [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

1. Build the project:
```sh
dotnet build
```

2. Complete the [Create configuration files](#create-configuration-files) step in the root directory (same level as `README.md`)
3. Run the project:
```sh
dotnet run --project BackloggdImporter
```

## Create configuration files

Before running the program, create these files in the root directory (or next to the executable if using a release build):

### CSV File Format

Create a file named `backloggd_import.csv` with the following structure:

**Required columns:**
- `Name` — game title (**required**; string)

**Optional columns:**
- `Rating` — rating from 1 to 10 (integer)
- `Platform` — platform (string, [see supported platforms](#supported-platforms))
- `Status` — status (string, [see supported statuses](#supported-game-statuses))
- `Review` — review (string, can be empty)
- `ReleaseYear` — release year (integer)

> **Important notes:**
> - If `Status` is empty or missing, the default status `completed` will be used
> - Reviews will be visible to all users on the Backloggd website
> - The utility takes the first search result; specify `ReleaseYear` for games with common names (remasters, remakes, etc.)

**Minimal example:**

| Name                   |
|------------------------|
| The Legend of Zelda    |
| Hades                  |
| Cyberpunk 2077         |

**Full example:**

| Name                                 | Rating | Platform      | Status    | Review                                         | ReleaseYear |
|--------------------------------------|--------|---------------|-----------|------------------------------------------------|-------------|
| The Legend of Zelda: Ocarina of Time | 10     | Nintendo 64   | completed |                                                | 1998        |
| Hades                                | 9      | PC            | completed | Amazing roguelike with incredible storytelling | 2020        |
| Cyberpunk 2077                       | 7      | PlayStation 5 | playing   | Getting better with updates                    | 2020        |
| Untitled Goose Game                  |        |               |           |                                                |             |

### Get your Backloggd session data

1. Log in to [Backloggd](https://backloggd.com/)
2. Open Developer Tools (F12)
3. Go to the Network tab
4. Perform any action on the site (e.g., add a game to your backlog)
5. Find the request and extract:
   - the `_backloggd_session` cookie value
   - the `X-Csrf-token` header value
   - your user ID from the request URL

Create `config.json` with this format:
```json
{
  "SessionCookie": "your_session_cookie_value",
  "CsrfToken": "your_csrf_token_value",
  "UserId": "your_numeric_user_id"
}
```

## Supported Platforms

> Note: The list of supported platforms is compiled from various open sources. Not all platforms have been tested in practice, so some mappings may not work as expected.

The utility supports automatic platform mapping for:

- **PC/Windows:** PC, Windows
- **PlayStation:** PlayStation, PS1, PlayStation 2, PS2, PlayStation 3, PS3, PlayStation 4, PS4, PlayStation 5, PS5, PSP, PlayStation Vita, Vita
- **Xbox:** Xbox, Xbox 360, Xbox One, Xbox Series X, Xbox Series S, Xbox Series
- **Nintendo:** Nintendo 64, N64, Wii, Wii U, Nintendo DS, DS, Nintendo 3DS, 3DS, GameCube, Game Boy, Game Boy Color, Game Boy Advance, GBA, Nintendo Switch, Switch, NES, SNES
- **Mobile:** Android, iOS, iPhone, iPad
- **Other:** Linux, Mac, SteamOS, Steam Deck, Dreamcast, Sega Genesis, Sega Saturn, Arcade, Commodore 64, Amstrad CPC, Atari 2600, Amiga, Stadia

## Supported Game Statuses

- `completed` — Game is fully completed
- `played` — Game was played (but not necessarily completed)
- `playing` — Currently playing this game
- `backlog` — Game is in backlog (planned to play)
- `wishlist` — Game is in wishlist (want to acquire)
- `shelved` — Game is shelved (temporarily not playing)
- `abandoned` — Game is abandoned (do not plan to return)
- `retired` — Game is retired (finished all interaction)

## Dependencies

- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [Polly](https://github.com/App-vNext/Polly)
