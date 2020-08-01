module Tests

open System
open FSharp.Data
open Xunit
open FSharp.Json
open FsUnit

type Status = {
    Code: int
    Message: string
    Failed: bool
}

type Content = {
    Title: string
    Lyrics: string
    Artist: string
}

type CanadaroResponse = {
    Status: Status
    Content: Content list
}

[<Fact>]
let ``My test`` () =
    let config = JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)
    let responseJson = Http.RequestString "https://api.canarado.xyz/lyrics/killpop"
    let response = Json.deserializeEx<CanadaroResponse> config responseJson
    response.Content |> Seq.iter(fun lyric -> Console.WriteLine lyric.Title)
    response.Content |> should not' (be Empty)