namespace Belin.Akismet

open System
open System.Collections.Generic
open System.Text

/// Represents the front page or home URL transmitted when making requests.
type Blog(url: Uri) =

  /// Creates a new blog.
  new(url: string) = Blog(Uri(url, UriKind.Absolute))

  /// The character encoding for the values included in comments.
  member val Charset: Encoding option = None with get, set

  /// The languages in use on the blog or site, in ISO 639-1 format.
  member val Languages: string list = [] with get, set

  /// The blog or site URL.
  member val Url: Uri = url with get, set

  /// Converts the specified blog to a dictionary.
  static member op_Explicit(blog: Blog): Dictionary<string, string> =
    let dictionary = Dictionary<string, string>()
    dictionary.Add ("blog", blog.Url.ToString())

    match blog.Charset with
    | None -> ()
    | Some encoding -> dictionary.Add ("blog_charset", encoding.WebName)

    if not blog.Languages.IsEmpty then dictionary.Add ("blog_lang", blog.Languages |> String.concat ",")
    dictionary
