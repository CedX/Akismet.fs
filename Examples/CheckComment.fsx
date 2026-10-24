#r "nuget: Belin.Akismet.FSharp"
open Belin.Akismet
open System
open System.Text

// Checks a comment against the Akismet service.
async {
  let author = Author(
    "192.168.123.456",
    Email = "john.doe@domain.com",
    Name = "John Doe",
    Role = "guest",
    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:156.0) Gecko/20100101 Firefox/156.0"
  )

  let comment = Comment(
    author,
    Content = "A user comment.",
    Date = Some DateTime.Now,
    Referrer = Some (Uri "https://github.com/CedX/Akismet.fs"),
    Type = CommentType.ContactForm
  )

  let blog = Blog("https://www.yourblog.com", Charset = Some Encoding.UTF8, Languages = ["fr"])
  use client = new Client("123YourAPIKey", blog)

  match! client.CheckCommentAsync comment with
  | Ok Ham -> printfn "The comment is ham."
  | Ok Spam | Ok PervasiveSpam -> printfn "The comment is spam."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
