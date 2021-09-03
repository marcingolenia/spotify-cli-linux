open System
open Argu
open Lyrics.Api
open Spotify.Dbus
open Arguments

let formatLyric (lyric: Lyric) =
    sprintf "%s - %s %s%s %s" lyric.Artist lyric.Title Environment.NewLine lyric.Lyrics Environment.NewLine

let errorHandler =
    ProcessExiter(
        colorizer =
            function
            | ErrorCode.HelpText -> None
            | _ -> Some ConsoleColor.Red
    )

let execute command =
    async {
        match command with
        | Play ->
            do! SpotifyBus.Play |> SpotifyBus.send
            return None
        | Pause ->
            do! SpotifyBus.Pause |> SpotifyBus.send
            return None
        | Next ->
            do! SpotifyBus.NextSong |> SpotifyBus.send
            return None
        | Prev ->
            do! SpotifyBus.PreviousSong |> SpotifyBus.send
            return None
        | Status ->
            let! status = SpotifyBus.retrieveCurrentSong
            return Some(sprintf "%s - %s" (status.Artists |> String.concat " feat ") status.Title)
        | Lyrics ->
            let! status = SpotifyBus.retrieveCurrentSong
            let lyrics =
                (LyricsApi.Genius.findBy
                    (Environment.GetEnvironmentVariable "GENIUS_API_KEY")
                    status.Title
                    status.Artists.[0])
            return Some(formatLyric lyrics)
    }

[<EntryPoint>]
let main argv =
    let parser =
        ArgumentParser.Create<Arguments>(errorHandler = errorHandler)
    let command =
        (parser.Parse argv).GetAllResults()
        |> List.tryHead
    match command with
    | Some command ->
        try
            match execute command |> Async.RunSynchronously with
            | Some text -> printfn "%s" text
            | None -> ()
        with
        | ex -> printfn "Couldn't connect to Spotify, is it running?"
    | None -> printfn "%s" <| parser.PrintUsage()
    0
