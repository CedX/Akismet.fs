#r "nuget: Belin.Akismet.FSharp"
open Belin.Akismet

// Verifies an Akismet API key.
async {
  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.VerifyKeyAsync() with
  | Ok true -> printfn "The API key is valid."
  | Ok false -> printfn "The API key is invalid."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
