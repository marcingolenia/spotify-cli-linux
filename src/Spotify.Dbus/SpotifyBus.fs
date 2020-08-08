namespace Spotify.Dbus

open System
open System.Collections.Generic
open System.Threading.Tasks
open Tmds.DBus

module SpotifyBus =
    type Signal = Play | Stop | Pause | NextSong | PreviousSong | PlayPause
    type PlaybackStatus = Playing | Paused | Stopped
    type Song = {
        Title : string
        Artists: string[]
        Album: string
        Url: Uri
    }
    
    [<DBusInterface("org.mpris.MediaPlayer2.Player")>]
    type IPlayer =
        inherit IDBusObject 
        abstract member NextAsync : unit -> Task
        abstract member PreviousAsync : unit -> Task
        abstract member PauseAsync : unit -> Task
        abstract member PlayAsync : unit -> Task
        abstract member StopAsync : unit -> Task
        abstract member PlayPauseAsync : unit -> Task
        abstract member GetAsync<'T> : string -> Task<'T>
    let player =
        let player = Connection.Session.CreateProxy<IPlayer>("org.mpris.MediaPlayer2.spotify",
                                                             ObjectPath("/org/mpris/MediaPlayer2"))
        player
        
    let retrieveCurrentSong =
        async {
            let! metadata = player.GetAsync<IDictionary<string, Object>> "Metadata" |> Async.AwaitTask
            return {
                Title = string metadata.["xesam:title"]
                Artists = metadata.["xesam:artist"] :?> string[]
                Album = string metadata.["xesam:album"]
                Url = Uri(string metadata.["xesam:url"])
            }
        }
    
    let getStatus =
        async {
            let! status = (player.GetAsync<string>("PlaybackStatus") |> Async.AwaitTask)
            return match status with
                      | "Playing" -> Playing
                      | "Paused" -> Paused
                      | _ -> Stopped
        }
        
    let send signal =
        match signal with
        | Play -> player.PlayAsync()
        | Stop -> player.StopAsync()
        | Pause -> player.PauseAsync()
        | PlayPause -> player.PlayPauseAsync()
        | PreviousSong -> player.PreviousAsync()
        | NextSong -> player.NextAsync()
        |> Async.AwaitTask
   