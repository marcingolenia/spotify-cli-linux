module Tests

open System
open LyricsApi
open Xunit
open FsUnit

[<Fact>]
let ``GIVEN title and artist WHEN fetchLyrics matches lyrics THEN list of matching lyrics is returned`` () =
    let artist, title = "Rammstein", "Ohne Dich"
    let lyrics = Genius.findBy (Environment.GetEnvironmentVariable "GENIUS_API_KEY") title artist
    let ``Ohne dich by Rammstein`` = lyrics
    ``Ohne dich by Rammstein``.Artist |> should equal artist
    ``Ohne dich by Rammstein``.Title |> should contain title
    ``Ohne dich by Rammstein``.Lyrics.Length |> should be (greaterThan 100)
    ``Ohne dich by Rammstein``.Lyrics |> should contain "Mit dir bin ich auch allein"