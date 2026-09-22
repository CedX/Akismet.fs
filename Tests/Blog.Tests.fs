namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting

/// Tests the features of the <see cref="T:Belin.Akismet.Blog"/> class.
[<TestClass>]
type BlogTests() =

  [<TestMethod>]
  member _.ToDictionary() =
    // It should return only the blog URL with a newly created instance.
    let dictionary = Blog "https://github.com/CedX/Akismet.fs" |> Blog.op_Explicit
    Assert.HasCount(1, dictionary)
    Assert.AreEqual("https://github.com/CedX/Akismet.fs", dictionary["blog"])

    // It should return a non-empty map with an initialized instance.
    // dictionary = (Dictionary<string, string>) Blog("https://github.com/CedX/Akismet.fs") { Charset = Encoding.UTF8, Languages = ["en", "fr"] }
    // Assert.HasCount(3, dictionary)
    // Assert.AreEqual("https://github.com/CedX/Akismet.fs", dictionary["blog"])
    // Assert.AreEqual("utf-8", dictionary["blog_charset"])
    // Assert.AreEqual("en,fr", dictionary["blog_lang"])
