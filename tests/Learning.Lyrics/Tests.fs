module Tests

open System
open System.Net
open System.Text
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

type CanadaroSuccessResponse = {
    Content: Content list
    Status: Status
}

type CanadaroErrorResponse = {
    Status: Status
}

[<Fact>]
let ``WHEN Lyrics can be found THEN they can be deserialized`` () =
    let config = JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)
    let responseJson = Http.RequestString "https://api.canarado.xyz/lyrics/killpop"
    let response = Json.deserializeEx<CanadaroSuccessResponse> config responseJson
    response.Content |> should not' (be Empty)
    response.Content |> Seq.iter(fun lyric -> Console.WriteLine lyric.Title)
    
[<Fact>]
let ``WHEN lyrics are not found THEN NotFound Status code is retrieved with proper message`` () =
    let config = JsonConfig.create(jsonFieldNaming = Json.lowerCamelCase)
    //try 
    let response = Http.Request ("https://api.canarado.xyz/lyrics/youwontfindthisman_12345", silentHttpErrors = true)
    let responseText = match response.Body with
                        | Text jsonText -> jsonText
                        | Binary binary -> Encoding.UTF8.GetString binary 
    let canadaroResponse = Json.deserializeEx<CanadaroErrorResponse> config responseText
    response.StatusCode |> should equal (int HttpStatusCode.NotFound)
    canadaroResponse.Status.Failed |> should equal true
    canadaroResponse.Status.Code |> should equal 404
    canadaroResponse.Status.Message |> should equal "Song information could not be found, try again?"
