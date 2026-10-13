# Submit Spam
This call is for submitting comments that weren't marked as spam but should have been.

It is very important that the values you submit with this call match those of your [comment check](CommentCheck.md) calls as closely as possible.
In order to learn from its mistakes, Akismet needs to match your missed spam and false positive reports
to the original [comment check](CommentCheck.md) API calls made when the content was first posted. While it is normal for less information
to be available for [submit spam](SubmitSpam.md) and [submit ham](SubmitHam.md) calls (most comment systems and forums will not store all metadata),
you should ensure that the values that you do send match those of the original content.

See the [Akismet API documentation](https://akismet.com/developers/detailed-docs/submit-spam-missed-spam) for more information.

```fsharp
member Client.SubmitSpam: Comment * CancellationToken opt
  -> Async<Result<unit, HttpRequestException>>
```

## Parameters

### **comment**: Comment
The user's `Comment` to be submitted, incorrectly classified as ham.

> [!NOTE]
> Ideally, it should be the same object as the one passed to the original [comment check](CommentCheck.md) API call.

### **?cancellationToken**: CancellationToken
The token to cancel the operation.

## Return value
An `Async<Result>` that completes when the given `Comment` has been submitted.

The result fails with an `Error (HttpRequestException)` when an issue occurs.
The exception `Message` usually includes some debug information, provided by the `X-akismet-debug-help` HTTP header,
about what exactly was invalid about the call.

It can also fail with a custom error message (provided by the `X-akismet-alert-msg` header).
See [Response Error Codes](https://akismet.com/developers/detailed-docs/errors) for more information.

## Example

```fsharp
open Belin.Akismet

async {
  let author = Author("192.168.123.456", UserAgent = "Spam Bot/6.6.6")
  let comment = Comment(author, Content = "Spam!")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.SubmitSpamAsync comment with
  | Ok () -> printfn "The comment was successfully submitted as spam."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
```
