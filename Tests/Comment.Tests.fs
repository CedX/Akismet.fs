namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Globalization

/// Tests the features of the `Comment` class.
[<TestClass>]
type CommentTests () =

  [<TestMethod>]
  member _.ToDictionary () =
    // It should return only the author info with a newly created instance.
    let mutable dictionary = Comment(Author "127.0.0.1").ToDictionary()
    dictionary.Count |> Should.be 1
    dictionary["user_ip"] |> Should.be "127.0.0.1"

    // It should return a non-empty map with an initialized instance.
    let comment = Comment(
      Author("192.168.0.1", Name = "Cédric Belin", UserAgent = "Doom/6.6.6"),
      Content = "A user comment.",
      Date = Some(DateTime.Parse("2000-01-01T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)),
      Referrer = Some(Uri "https://cedric-belin.fr"),
      Type = CommentType.BlogPost
    )

    dictionary <- comment.ToDictionary()
    dictionary.Count |> Should.be 7
    dictionary["comment_author"] |> Should.be "Cédric Belin"
    dictionary["comment_content"] |> Should.be "A user comment."
    dictionary["comment_date_gmt"] |> Should.be "2000-01-01T00:00:00.0000000Z"
    dictionary["comment_type"] |> Should.be "blog-post"
    dictionary["referrer"] |> Should.be "https://cedric-belin.fr/"
    dictionary["user_agent"] |> Should.be "Doom/6.6.6"
    dictionary["user_ip"] |> Should.be "192.168.0.1"
