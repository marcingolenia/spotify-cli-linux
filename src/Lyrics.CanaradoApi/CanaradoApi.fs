﻿namespace Lyrics.CanaradoApi

open System
open System.Text
open FSharp.Data
open FSharp.Json

module CanaradoApi =
    type Status =
        { Code: int
          Message: string
          Failed: bool }
    type Lyric =
        { Title: string
          Lyrics: string
          Artist: string }
    type CanadaroSuccessResponse =
        { Content: Lyric list
          Status: Status }
    type CanadaroErrorResponse =
        { Status: Status }
    type String with
        member x.Equivalent(other) = String.Equals(x, other, System.StringComparison.CurrentCultureIgnoreCase)

    let fetchByTitle title =
        let config = JsonConfig.create (jsonFieldNaming = Json.lowerCamelCase)
        let response = Http.Request(sprintf "https://api.canarado.xyz/lyrics/%s" title, silentHttpErrors = true)
        let responseText =
            match response.Body with
            | Text jsonText -> jsonText
            | Binary binary -> Encoding.UTF8.GetString binary
        match response.StatusCode with
        | 200 -> Some((Json.deserializeEx<CanadaroSuccessResponse> config responseText).Content)
        | _ -> None

    let private applyArtistFilter artist lyrics =
        let filteredLyrics = lyrics |> List.filter (fun lyric -> lyric.Artist.Equivalent artist)
        match filteredLyrics with
        | [] -> Some lyrics
        | _ -> Some filteredLyrics

    let fetch title (artist: string) =
        (fetchByTitle title) |> Option.bind (applyArtistFilter artist)
