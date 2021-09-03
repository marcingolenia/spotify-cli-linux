module LyricsApi.Genius

open System.Text.RegularExpressions
open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open Lyrics.Api

type GeniusSearchResult =
    JsonProvider<"""
{
   "response":{
      "hits":[
         {
            "highlights":[],
            "index":"song",
            "type":"song",
            "result":{
               "annotation_count":10,
               "api_path":"/songs/69537",
               "full_title":"Ohne Dich by Rammstein",
               "header_image_thumbnail_url":"https://images.genius.com/b022bdc1e192bdeb837c7c3cac41f793.300x300x1.jpg",
               "header_image_url":"https://images.genius.com/b022bdc1e192bdeb837c7c3cac41f793.1000x1000x1.jpg",
               "id":69537,
               "lyrics_owner_id":33985,
               "lyrics_state":"complete",
               "path":"/Rammstein-ohne-dich-lyrics",
               "pyongs_count":10,
               "song_art_image_thumbnail_url":"https://images.genius.com/b022bdc1e192bdeb837c7c3cac41f793.300x300x1.jpg",
               "song_art_image_url":"https://images.genius.com/b022bdc1e192bdeb837c7c3cac41f793.1000x1000x1.jpg",
               "stats":{
                  "unreviewed_annotations":9,
                  "hot":false,
                  "pageviews":81658
               },
               "title":"Ohne Dich",
               "title_with_featured":"Ohne Dich",
               "url":"https://genius.com/Rammstein-ohne-dich-lyrics",
               "song_art_primary_color":"#3fbf3f",
               "song_art_secondary_color":"#040c04",
               "song_art_text_color":"#fff",
               "primary_artist":{
                  "api_path":"/artists/16045",
                  "header_image_url":"https://images.genius.com/3543a06a5dc0f5bbef79cc3e1d80b16b.1000x275x1.jpg",
                  "id":16045,
                  "image_url":"https://images.genius.com/261aeb8c50a24a787fb1b8e5ba4fa356.1000x1000x1.png",
                  "is_meme_verified":false,
                  "is_verified":false,
                  "name":"Rammstein",
                  "url":"https://genius.com/artists/Rammstein"
               }
            }
         }
      ]
   }
}
""", InferTypesFromValues=false>

let findBy apiKey title artist =
    let song =
        Http.RequestString(
            "https://api.genius.com/search",
            query = [ "q", $"%s{artist} %s{title}" ],
            headers = [ Authorization $"Bearer %s{apiKey}" ]
        )
        |> GeniusSearchResult.Parse
        |> (fun res -> res.Response.Hits.[0].Result)
    let page =
        HtmlDocument
            .Load($"https://genius.com{song.Path}")
            .Body()
    let lyrics =
        page.CssSelect("div#lyrics-root-pin-spacer > div")
        @ page.CssSelect("div.lyrics")
        |> List.map (fun element -> element.InnerText())
        |> List.filter (fun txt -> txt.Length > 0)
        |> List.head
        |> (fun lyrics -> Regex.Replace(lyrics, @"\d+EmbedShare URLCopyEmbedCopy", ""))
    { Title = song.Title
      Lyrics = lyrics
      Artist = song.PrimaryArtist.Name }
