namespace Lyrics.CanaradoApi

open System
open System.Text
open FSharp.Data
open FSharp.Json

module CanaradoApi =
    
    type Status = {
        Code: int
        Message: string
        Failed: bool
    }

    type Lyric = {
        Title: string
        Lyrics: string
        Artist: string
    }

    type CanadaroSuccessResponse = {
        Content: Lyric list
        Status: Status
    }

    type CanadaroErrorResponse = {
        Status: Status
    }
    
    type String with
        member x.Equivalent(other) =
            String.Equals(x, other, System.StringComparison.CurrentCultureIgnoreCase)
            
    let private containsArtist (artist :string) lyric  =
        lyric.Artist.IndexOf(artist, StringComparison.InvariantCultureIgnoreCase) > -1

    let fetchLyrics title (artist:string) =
        let config = JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)
        let response = Http.Request (sprintf "https://api.canarado.xyz/lyrics/%s" title, silentHttpErrors = true)
        let responseText = match response.Body with
                            | Text jsonText -> jsonText
                            | Binary binary -> Encoding.UTF8.GetString binary 
        match response.StatusCode with
        | 200 -> Some ((Json.deserializeEx<CanadaroSuccessResponse> config responseText).Content
                     |> List.filter(containsArtist artist))
        | _ -> None