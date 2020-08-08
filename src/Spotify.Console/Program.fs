open System
open Argu
open Lyrics.CanaradoApi
open Spotify.Dbus
open Arguments

let formatLyric (lyric: CanaradoApi.Lyric) =
    sprintf "%s - %s %s %s %s" lyric.Artist lyric.Title Environment.NewLine lyric.Lyrics Environment.NewLine

let retrieveLyrics title artist =
    let lyrics = CanaradoApi.fetch title artist
    match lyrics with
    | Some lyrics -> ("", lyrics) ||> List.fold (fun state lyric -> state + formatLyric lyric)
    | None -> "Lyrics were not found :("

let errorHandler = ProcessExiter (colorizer = function | ErrorCode.HelpText -> None| _ -> Some ConsoleColor.Red)

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
            return Some(sprintf "%s - %s" (status.Artists |> Array.fold (+) ", ") status.Title)
        | Lyrics ->
            let! status = SpotifyBus.retrieveCurrentSong
            return Some(retrieveLyrics status.Title status.Artists.[0])
    }

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments>(errorHandler = errorHandler)
    let command = (parser.Parse argv).GetAllResults() |> List.head
    try 
        match execute command |> Async.RunSynchronously with
        | Some text -> printfn "%s" text
        | None -> ()
    with | ex -> printfn "Couldn't connect to Spotify, is it running?"
    0
