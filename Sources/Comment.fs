namespace Belin.Akismet

open System

/// Represents a comment submitted by an author.
type Comment(author: Author) =

  /// The comment's author.
  member val Author: Author = author with get, set

  /// The comment's content.
  member val Content = "" with get, set

  /// The context in which this comment was posted.
  member val Context = ResizeArray<string>() with get, set

  /// The UTC timestamp of the creation of the comment.
  member val Date: DateTime option = None with get, set

  /// The permanent location of the entry the comment is submitted to.
  member val Permalink: Uri option = None with get, set

  /// The UTC timestamp of the publication time for the post, page or thread on which the comment was posted.
  member val PostModified: DateTime option = None with get, set

  /// A string describing why the content is being rechecked.
  member val RecheckReason = "" with get, set

  /// The URL of the webpage that linked to the entry being requested.
  member val Referrer: Uri option = None with get, set

  /// The comment's type.
  member val Type = "" with get, set

  /// Converts this comment to a dictionary.
  member internal this.ToDictionary() =
    let dictionary = this.Author.ToDictionary()
    if not (String.IsNullOrWhiteSpace this.Content) then dictionary.Add ("comment_content", this.Content)
    // TODO if this.Context.Count > 0 then dictionary.Add ("comment_context", this.Context |> String.concat ",")
    match this.Date with None -> () | Some value -> dictionary.Add ("comment_date_gmt", value.ToUniversalTime().ToString "o")
    match this.Permalink with None -> () | Some value -> dictionary.Add ("permalink", value.ToString())
    match this.PostModified with None -> () | Some value -> dictionary.Add ("comment_post_modified_gmt", value.ToUniversalTime().ToString "o")
    if not (String.IsNullOrWhiteSpace this.RecheckReason) then dictionary.Add ("recheck_reason", this.RecheckReason)
    match this.Referrer with None -> () | Some value -> dictionary.Add ("referrer", value.ToString())
    if not (String.IsNullOrWhiteSpace this.Type) then dictionary.Add ("comment_type", this.Type)
    dictionary

/// Specifies the type of a comment.
module CommentType =

  /// A blog post.
  [<Literal>]
  let BlogPost = "blog-post"

  /// A blog comment.
  [<Literal>]
  let Comment = "comment"

  /// A contact form or feedback form submission.
  [<Literal>]
  let ContactForm = "contact-form"

  /// A top-level forum post.
  [<Literal>]
  let ForumPost = "forum-post"

  /// A message sent between just a few users.
  [<Literal>]
  let Message = "message"

  /// A reply to a top-level forum post.
  [<Literal>]
  let Reply = "reply"

  /// A new user account.
  [<Literal>]
  let Signup = "signup"
