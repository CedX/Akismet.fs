# Submit Ham
This call is intended for the submission of false positives - items that were incorrectly classified as spam by Akismet.
It takes identical arguments as [comment check](CommentCheck.md) and [submit spam](SubmitSpam.md).

Remember that, as explained in the [submit spam](SubmitSpam.md) documentation, you should ensure
that any values you're passing here match up with the original and corresponding [comment check](CommentCheck.md) call.

See the [Akismet API documentation](https://akismet.com/developers/detailed-docs/submit-ham-false-positives) for more information.

```fsharp
Task Client.SubmitHam(Comment comment, CancellationToken cancellationToken = default)
```

## Parameters

### Comment **comment**
The user's `Comment` to be submitted, incorrectly classified as spam.

> [!NOTE]
> Ideally, it should be the same object as the one passed to the original [comment check](CommentCheck.md) API call.

### CancellationToken **cancellationToken**
The token to cancel the operation.

## Return value
A `Task` that completes when the given `Comment` has been submitted.

The task faults with a `HttpRequestException` when an issue occurs.
The exception `Message` usually includes some debug information, provided by the `X-akismet-debug-help` HTTP header,
about what exactly was invalid about the call.

It can also fault with a custom error message (provided by the `X-akismet-alert-msg` header).
See [Response Error Codes](https://akismet.com/developers/detailed-docs/errors) for more information.

## Example

```fsharp
open Belin.Akismet

async {
  let author = Author("192.168.123.456", UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:156.0) Gecko/20100101 Firefox/156.0")
  let comment = Comment(author, Content = "I'm testing out the Service API.")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.SubmitHamAsync comment with
  | Ok () -> printfn "The comment was successfully submitted as ham."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
```
