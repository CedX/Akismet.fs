namespace Belin.Akismet

open System
open System.Collections.Generic
open System.Text

/// Represents the front page or home URL transmitted when making requests.
type Blog(url: Uri) =

  /// The character encoding for the values included in comments.
  member val Charset: Encoding option = None with get, set

  /// The languages in use on the blog or site, in ISO 639-1 format.
  member val Languages = ResizeArray<string>() with get, set

  /// The blog or site URL.
  member val Url: Uri = url with get, set

  /// Creates a new blog.
  new(url: string) = Blog(Uri(url, UriKind.Absolute))

  /// Converts this blog to a dictionary.
  member internal this.ToDictionary() =
    let dictionary = Dictionary<string, string>()
    dictionary.Add ("blog", this.Url.ToString())
    match this.Charset with None -> () | Some value -> dictionary.Add ("blog_charset", value.WebName)
    if this.Languages.Count > 0 then dictionary.Add ("blog_lang", this.Languages |> String.concat ",")
    dictionary
