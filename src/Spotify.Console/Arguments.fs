module Spotify.Dbus.Arguments

open Argu

type Arguments =
    | [<First>] Play
    | [<First>] Pause
    | [<First>] Prev
    | [<First>] Next
    | [<First>] Status
    | [<First>] Lyrics

    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Play -> "Spotify play"
            | Pause -> "Spotify pause"
            | Prev -> "Previous song"
            | Next -> "Next song"
            | Status -> "Shows song name and artist"
            | Lyrics -> "Prints the song lyrics"
