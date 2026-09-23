namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Text

/// Tests the features of the `Blog` class.
[<TestClass>]
type BlogTests() =

  [<TestMethod>]
  member _.ToDictionary() =
    // It should return only the blog URL with a newly created instance.
    let mutable dictionary = (Blog "https://github.com/CedX/Akismet.fs").ToDictionary()
    Assert.HasCount(1, dictionary)
    Assert.AreEqual("https://github.com/CedX/Akismet.fs", dictionary["blog"])

    // It should return a non-empty map with an initialized instance.
    dictionary <- Blog("https://github.com/CedX/Akismet.fs", Charset = Some Encoding.UTF8, Languages = ["en"; "fr"]).ToDictionary()
    Assert.HasCount(3, dictionary)
    Assert.AreEqual("https://github.com/CedX/Akismet.fs", dictionary["blog"])
    Assert.AreEqual("utf-8", dictionary["blog_charset"])
    Assert.AreEqual("en,fr", dictionary["blog_lang"])
