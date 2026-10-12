namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System

/// Tests the features of the `Author` class.
[<TestClass>]
type AuthorTests() =

  [<TestMethod>]
  member _.ToDictionary () =
    // It should return only the IP address with a newly created instance.
    let mutable dictionary = (Author "127.0.0.1").ToDictionary()
    dictionary.Count |> Should.be 1
    dictionary["user_ip"] |> Should.be "127.0.0.1"

    // It should return a non-empty map with an initialized instance.
    let author = Author("192.168.0.1",
      Name = "Cédric Belin",
      Email = "contact@cedric-belin.fr",
      Url = Some(Uri "https://cedric-belin.fr"),
      UserAgent = "Mozilla/5.0")

    dictionary <- author.ToDictionary()
    dictionary.Count |> Should.be 5
    dictionary["comment_author"] |> Should.be "Cédric Belin"
    dictionary["comment_author_email"] |> Should.be "contact@cedric-belin.fr"
    dictionary["comment_author_url"] |> Should.be "https://cedric-belin.fr/"
    dictionary["user_agent"] |> Should.be "Mozilla/5.0"
    dictionary["user_ip"] |> Should.be "192.168.0.1"
