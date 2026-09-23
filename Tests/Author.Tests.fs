namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System

/// Tests the features of the `Author` class.
[<TestClass>]
type AuthorTests() =

  [<TestMethod>]
  member _.ToDictionary() =
    // It should return only the IP address with a newly created instance.
    let mutable dictionary = (Author "127.0.0.1").ToDictionary()
    Assert.HasCount(1, dictionary)
    Assert.AreEqual("127.0.0.1", dictionary["user_ip"])

    // It should return a non-empty map with an initialized instance.
    let author = Author("192.168.0.1",
      Name = "Cédric Belin",
      Email = "contact@cedric-belin.fr",
      Url = Some(Uri "https://cedric-belin.fr"),
      UserAgent = "Mozilla/5.0")

    dictionary <- author.ToDictionary()
    Assert.HasCount(5, dictionary)
    Assert.AreEqual("Cédric Belin", dictionary["comment_author"])
    Assert.AreEqual("contact@cedric-belin.fr", dictionary["comment_author_email"])
    Assert.AreEqual("https://cedric-belin.fr/", dictionary["comment_author_url"])
    Assert.AreEqual("Mozilla/5.0", dictionary["user_agent"])
    Assert.AreEqual("192.168.0.1", dictionary["user_ip"])
