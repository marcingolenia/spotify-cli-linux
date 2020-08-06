open System
open Argu
open Spotify.Dbus

type Arguments =
    | [<First>] Play
    | [<First>] Pause
    | [<First>] Prev
    | [<First>] Next
    | [<First>] Status
    | [<First>] Song
    | [<First>] Artist
    | [<First>] Album
    | [<First>] Lyrics
    
    interface IArgParserTemplate with
        member arg.Usage =
            match arg with
            | Play -> "Spotify play"
            | Pause -> "Spotify pause"
            | Prev -> "Previous song"
            | Next -> "Next song"
            | Status -> "Shows song name and artist"
            | Song -> "Shows the song name"
            | Artist -> "Shows the song artist"
            | Album -> "Shows the song album name"
            | Lyrics -> "Prints the song lyrics"
            
let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)

let execute command =
    async {
        match command with
            | Play -> do! SpotifyBus.Play |> SpotifyBus.send
                      return None
            | _ -> do! SpotifyBus.NextSong |> SpotifyBus.send
                   return None
      }

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments>(errorHandler = errorHandler)
    let command = (parser.Parse argv).GetAllResults() |> List.head
    execute command |> Async.RunSynchronously
    0
    
