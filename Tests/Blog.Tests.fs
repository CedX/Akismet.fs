namespace Belin.Akismet

open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Text

/// Tests the features of the `Blog` class.
[<TestClass>]
type BlogTests() =

  [<TestMethod>]
  member _.ToDictionary () =
    // It should return only the blog URL with a newly created instance.
    let mutable dictionary = (Blog "https://github.com/CedX/Akismet.fs").ToDictionary()
    dictionary.Count |> Should.be 1
    dictionary["blog"] |> Should.be "https://github.com/CedX/Akismet.fs"

    // It should return a non-empty map with an initialized instance.
    dictionary <- Blog("https://github.com/CedX/Akismet.fs", Charset = Some Encoding.UTF8, Languages = ["en"; "fr"]).ToDictionary()
    dictionary.Count |> Should.be 3
    dictionary["blog"] |> Should.be "https://github.com/CedX/Akismet.fs"
    dictionary["blog_charset"] |> Should.be "utf-8"
    dictionary["blog_lang"] |> Should.be "en,fr"
