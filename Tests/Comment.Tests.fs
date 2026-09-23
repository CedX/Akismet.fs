namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Globalization

/// Tests the features of the `Comment` class.
[<TestClass>]
type CommentTests() =

  [<TestMethod>]
  member _.ToDictionary() =
    // It should return only the author info with a newly created instance.
    let mutable dictionary = Comment(Author "127.0.0.1").ToDictionary()
    Assert.HasCount(1, dictionary)
    Assert.AreEqual("127.0.0.1", dictionary["user_ip"])

    // It should return a non-empty map with an initialized instance.
    let comment = Comment(
      Author("192.168.0.1", Name = "Cédric Belin", UserAgent = "Doom/6.6.6"),
      Content = "A user comment.",
      Date = Some(DateTime.Parse("2000-01-01T00:00:00Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)),
      Referrer = Some(Uri "https://cedric-belin.fr"),
      Type = CommentType.BlogPost
    )

    dictionary <- comment.ToDictionary()
    Assert.HasCount(7, dictionary)
    Assert.AreEqual("Cédric Belin", dictionary["comment_author"])
    Assert.AreEqual("A user comment.", dictionary["comment_content"])
    Assert.AreEqual("2000-01-01T00:00:00.0000000Z", dictionary["comment_date_gmt"])
    Assert.AreEqual("blog-post", dictionary["comment_type"])
    Assert.AreEqual("https://cedric-belin.fr/", dictionary["referrer"])
    Assert.AreEqual("Doom/6.6.6", dictionary["user_agent"])
    Assert.AreEqual("192.168.0.1", dictionary["user_ip"])
