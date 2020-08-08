module Tests
open System
open Spotify.Dbus
open Xunit
open SpotifyBus
open FsUnit

(* 200ms seems to work well. This interval is required to make the tests pass because it takes some time to accept the
D-Bus message and perform actual actions by Spotify. Remember to turn on Spotify ;) *)

[<Fact>]
[<Trait("Category","SpotifyIntegration")>]
let ``GIVEN retrieveCurrentSong WHEN Song is selected in Spotify THEN the title, artist, url, album are retrieved`` () =
    // Act
    let song = retrieveCurrentSong |> Async.RunSynchronously
    // Assert
    song.Title |> should not' (be EmptyString)
    song.Artists |> Seq.head |> should not' (be EmptyString)
    song.Album |> should not' (be EmptyString)
    string song.Url |> should not' (be EmptyString)
    sprintf "%A" song |> Console.WriteLine
    
[<Fact>]
[<Trait("Category","SpotifyIntegration")>]
let ``GIVEN send NextSong WHEN Song is changed THEN it is different then previous song`` () =
    // Arrange
    let songBeforeNext = retrieveCurrentSong |> Async.RunSynchronously
    // Act
    NextSong |> send |> Async.RunSynchronously
    // Assert
    Async.Sleep 500 |> Async.RunSynchronously
    let actualSong = retrieveCurrentSong |> Async.RunSynchronously
    songBeforeNext |> should not' (equal actualSong)
    
[<Fact>]
[<Trait("Category","SpotifyIntegration")>]
let ``GIVEN send Play WHEN Song is Paused THEN the resulting status is Playing`` () =
    // Arrange
    Pause |> send |> Async.RunSynchronously
    // Act
    Play |> send |> Async.RunSynchronously
    // Assert
    Async.Sleep 200 |> Async.RunSynchronously
    getStatus |> Async.RunSynchronously |> should equal Playing

[<Fact>]
[<Trait("Category","SpotifyIntegration")>]
let ``GIVEN send Pause WHEN Song is Playing THEN the resulting status is Paused`` () =
    // Arrange
    Play |> send |> Async.RunSynchronously
    // Act
    Pause |> send |> Async.RunSynchronously
    // Assert
    Async.Sleep 200 |> Async.RunSynchronously
    getStatus |> Async.RunSynchronously |> should equal Paused
    