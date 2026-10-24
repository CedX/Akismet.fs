#r "nuget: Belin.Akismet.FSharp"
open Belin.Akismet

// Submits spam to the Akismet service.
async {
  let author = Author("192.168.123.456", UserAgent = "Spam Bot/6.6.6")
  let comment = Comment(author, Content = "Spam!")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.SubmitSpamAsync comment with
  | Ok () -> printfn "The comment was successfully submitted as spam."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
