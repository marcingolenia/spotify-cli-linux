![.NET Core](https://github.com/marcingolenia/spotify-cli-linux/workflows/.NET%20Core/badge.svg?branch=master)

F# spotify-cli-linux
A command line interface to Spotify on Linux.
This project is inspired by the similar project called [spotify-cli-linux](https://github.com/pwittchen/spotify-cli-linux), which does similar things but it is written in Python.

# Building
You will need dotnet core 3.1 SDK to build and run the project. You can download it from here: https://dotnet.microsoft.com/download/dotnet-core/3.1. Then just use standard dotnet commands to build, test and run.

# Publishing the app
dotnet publish -c Release -r linux-x64

# Usage
```
USAGE: dotnet [--help] [--play] [--pause] [--prev] [--next] [--status] [--lyrics]

OPTIONS:

    --play                Spotify play
    --pause               Spotify pause
    --prev                Previous song
    --next                Next song
    --status              Shows song name and artist
    --lyrics              Prints the song lyrics
    --help                display this list of options.
```

Examples:
* `dotnet Spotify.Console.dll --help`
* `dotnet Spotify.Console.dll --lyrics`

# Hint: Add command alias
You can easily set command alias in Unix, for example:

`alias spot='dotnet ~/projects/spotify-linux-published/Spotify.Console.dll'`

From now on you can just type `spot --help`.