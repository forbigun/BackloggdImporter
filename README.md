# BackloggdImporter

A console utility for importing game logs into
the [Backloggd](https://backloggd.com/) platform from a CSV file.

## Features

- Reads game data from a CSV file
- Automatically searches for games on Backloggd
- Creates logs for each game, including rating, platform, status, and review

## Build

-

Requires [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

To build and run the project:

```sh
 dotnet build
 dotnet run --project BackloggdImporter
```

## Dependencies

- [CsvHelper](https://joshclose.github.io/CsvHelper/) — CSV parsing library

Dependencies are automatically restored when building the project.

## Usage

### 1. Prepare your CSV file

The CSV file should be named `backloggd_import.csv` and placed in the same
directory as the executable.

**Required columns:**

- `Name` — Game title (string)
- `Rating` — Game rating from 1-10 (integer)
- `Platform` — Gaming platform (string, see supported platforms below)
- `Status` — Game status (string, see supported statuses below; if left empty,
  the log will be created with status `completed` by default)
- `Review` — Optional review text (string, can be empty)

**Example CSV:**

| Name                                 | Rating | Platform      | Status    | Review                                         |
|--------------------------------------|--------|---------------|-----------|------------------------------------------------|
| The Legend of Zelda: Ocarina of Time | 10     | Nintendo 64   | completed | Absolute masterpiece of game design!           |
| Hades                                | 9      | PC            | completed | Amazing roguelike with incredible storytelling |
| Cyberpunk 2077                       | 7      | PlayStation 5 | playing   | Getting better with updates                    |

### 2. Get your Backloggd session data

You need to extract session data from your browser:

1. Log in to [Backloggd](https://backloggd.com/)
2. Open Developer Tools (F12)
3. Go to the Network tab
4. Perform any action on the site (like adding a game to backlog)
5. Find the request and extract:
    - `_backloggd_session` cookie value
    - `X-Csrf-token` header value
    - Your user ID from the request URL

### 3. Create config file

Create a `config.json` file with your session data:

```json
{
  "SessionCookie": "your_session_cookie_value",
  "CsrfToken": "your_csrf_token_value",
  "UserId": "your_numeric_user_id"
}
```

### 4. Build and run

```sh
dotnet build
dotnet run --project BackloggdImporter
```

### 5. Output

The utility will:

- Process each game entry from the CSV
- Display colored status messages for each game
- Show a summary of failed imports at the end
- Wait 10 seconds between requests to avoid rate limiting

## Supported Platforms

The utility supports automatic platform mapping for:

- **PC/Windows:** PC, Windows
- **PlayStation:** PlayStation, PS1, PlayStation 2, PS2, PlayStation 3, PS3,
  PlayStation 4, PS4, PlayStation 5, PS5, PSP, PlayStation Vita, Vita
- **Xbox:** Xbox, Xbox 360, Xbox One, Xbox Series X, Xbox Series S, Xbox Series
- **Nintendo:** Nintendo 64, N64, Wii, Wii U, Nintendo DS, DS, Nintendo 3DS,
  3DS, GameCube, Game Boy, Game Boy Color, Game Boy Advance, GBA, Nintendo
  Switch, Switch, NES, SNES
- **Mobile:** Android, iOS, iPhone, iPad
- **Other:** Linux, Mac, SteamOS, Steam Deck, Dreamcast, Sega Genesis, Sega
  Saturn, Arcade, Commodore 64, Amstrad CPC, Atari 2600, Amiga, Stadia

## Supported Game Statuses

- `completed` — Game is fully completed
- `played` — Game was played (but not necessarily completed)
- `playing` — Currently playing this game
- `backlog` — Game is in backlog (planned to play)
- `wishlist` — Game is in wishlist (want to acquire)
- `shelved` — Game is shelved (temporarily not playing)
- `abandoned` — Game is abandoned (do not plan to return)
- `retired` — Game is retired (finished all interaction)

## Troubleshooting

**"File not found" errors:**

- Ensure `backloggd_import.csv` and `config.json` are in the same directory as
  the executable

**"Not found" for many games:**

- Check that game names in CSV match Backloggd's database exactly
- Try shorter or alternative names for games

**Authentication errors:**

- Verify your session cookie and CSRF token are current
- Re-extract session data if you logged out or session expired

**Rate limiting:**

- The utility includes a 10-second delay between requests
- If you get rate limited, wait and try again later