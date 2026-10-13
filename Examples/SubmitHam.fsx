#r "../Binaries/Belin.Akismet.FSharp.dll" // TODO "nuget: Belin.Akismet.FSharp"
open Belin.Akismet

// Submits ham to the Akismet service.
async {
  let author = Author("192.168.123.456", UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:156.0) Gecko/20100101 Firefox/156.0")
  let comment = Comment(author, Content = "I'm testing out the Service API.")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.SubmitHamAsync comment with
  | Ok () -> printfn "The comment was successfully submitted as ham."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
