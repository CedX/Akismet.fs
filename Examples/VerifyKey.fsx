#r "nuget: Belin.Akismet.FSharp"
open Belin.Akismet

// Verifies an Akismet API key.
async {
  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.AsyncVerifyKey() with
  | Ok true -> printn "The API key is valid."
  | Ok false -> printn "The API key is invalid."
  | Error error -> printn $"An error occurred: {error.Message}"
}
