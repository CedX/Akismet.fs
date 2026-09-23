namespace Belin.Akismet

open System
open System.Collections.Generic
open System.Net

/// Represents the author of a comment.
type Author(ipAddress: IPAddress) =

  /// The author's mail address. If you set it to `"akismet-guaranteed-spam@example.com"`, Akismet will always return `true`.
  member val Email = "" with get, set

  /// The author's IP address.
  member val IPAddress: IPAddress = ipAddress with get, set

  /// The author's name. If you set it to `"viagra-test-123"`, Akismet will always return `true`.
  member val Name = "" with get, set

  /// The author's role. If you set it to `"administrator"`, Akismet will always return `false`.
  member val Role = "" with get, set

  /// The URL of the author's website.
  member val Url: Uri option = None with get, set

  /// The author's user agent, that is the string identifying the Web browser used to submit comments.
  member val UserAgent = "" with get, set

  /// Creates a new author.
  new(ipAddress: string) = Author(IPAddress.Parse ipAddress)

  /// Converts this author to a dictionary.
  member internal this.ToDictionary() =
    let dictionary = Dictionary<string, string>()
    dictionary.Add("user_ip", this.IPAddress.ToString())
    if not (String.IsNullOrWhiteSpace this.Email) then dictionary.Add("comment_author_email", this.Email)
    if not (String.IsNullOrWhiteSpace this.Name) then dictionary.Add("comment_author", this.Name)
    if not (String.IsNullOrWhiteSpace this.Role) then dictionary.Add("user_role", this.Role)
    match this.Url with None -> () | Some value -> dictionary.Add("comment_author_url", value.ToString())
    if not (String.IsNullOrWhiteSpace this.UserAgent) then dictionary.Add("user_agent", this.UserAgent)
    dictionary

/// Specifies the role of an author.
module AuthorRole =

  /// The author is an administrator.
  [<Literal>]
  let Administrator = "administrator"
