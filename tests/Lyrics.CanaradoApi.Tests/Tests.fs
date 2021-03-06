module Tests

open Lyrics.CanaradoApi
open Xunit
open FsUnit

[<Fact>]
let ``GIVEN title and artist WHEN fetchLyrics matches lyrics THEN list of matching lyrics is returned`` () =
    let (artist, title) = ("Rammstein", "Ohne Dich")
    let lyricsResult = CanaradoApi.fetch title artist
    let ``Ohne dich by Rammstein`` = lyricsResult.Value |> List.head
    ``Ohne dich by Rammstein``.Artist |> should equal artist
    ``Ohne dich by Rammstein``.Title |> should contain title
    ``Ohne dich by Rammstein``.Lyrics.Length |> should be (greaterThan 100)