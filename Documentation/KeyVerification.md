# Key Verification
Key verification authenticates your API key before calling the [comment check](CommentCheck.md),
[submit spam](SubmitSpam.md) or [submit ham](SubmitHam.md) methods.

This is the first call that you should make to Akismet and is especially useful
if you will have multiple users with their own Akismet subscriptions using your application.

See the [Akismet API documentation](https://akismet.com/developers/detailed-docs/key-verification) for more information.

```fsharp
member Client.VerifyKey: CancellationToken option
  -> Async<Result<bool, HttpRequestException>>
```

## Parameters

### **?cancellationToken** CancellationToken
The token to cancel the operation.

## Return value
An `Async<Result>` that completes with a boolean value indicating whether the client's API key is valid.

The result fails with an `Error (HttpRequestException)` when an issue occurs.
The exception `Message` usually includes some debug information, provided by the `X-akismet-debug-help` HTTP header,
about what exactly was invalid about the call.

It can also fail with a custom error message (provided by the `X-akismet-alert-msg` header).
See [Response Error Codes](https://akismet.com/developers/detailed-docs/errors) for more information.

## Example

```fsharp
open Belin.Akismet

async {
  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  match! client.VerifyKeyAsync() with
  | Ok true -> printfn "The API key is valid."
  | Ok false -> printfn "The API key is invalid."
  | Error error -> printfn $"An error occurred: {error.Message}"
}
```

See the [source code](https://github.com/CedX/Akismet.fs/tree/main/Sources) for detailed information
about the `Client` and `Blog` classes, and their properties and methods.
