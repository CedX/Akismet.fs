# Testing
When you will integrate this library with your own application, you will of course need to test it.
Often we see developers get ahead of themselves, making a few trivial API calls with minimal values
and drawing the wrong conclusions about Akismet's accuracy.

## Simulate a positive result (spam)
Make a [comment check](CommentCheck.md) API call with the `Author.Name` set to `"viagra-test-123"`
or `Author.Email` set to `"akismet-guaranteed-spam@example.com"`. Populate all other required fields with typical values.

The Akismet API will always return a `CheckResult.Spam` response to a valid request with one of those values.
If you receive anything else, something is wrong in your client, data, or communications.

```fsharp
open Belin.Akismet

async {
  let author = Author("127.0.0.1", Name = "viagra-test-123", UserAgent = "Mozilla/5.0")
  let comment = Comment(author, Content = "A user comment.")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  let! result = client.CheckCommentAsync comment
  printfn $"It should be `Ok Spam`: {result}"
}
```

## Simulate a negative result (ham)
Make a [comment check](CommentCheck.md) API call with the `Author.Role` set to `"administrator"`
and all other required fields populated with typical values.

The Akismet API will always return a `CheckResult.Ham` response. Any other response indicates a data or communication problem.

```fsharp
open Belin.Akismet

async {
  let author = Author("127.0.0.1", Role = AuthorRole.Administrator, UserAgent = "Mozilla/5.0")
  let comment = Comment(author, Content = "A user comment.")

  use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com")
  let! result = client.CheckCommentAsync comment
  printfn $"It should be `Ok Ham`: {result}"
}
```

## Automated testing
Enable the `Client.IsTest` option in your tests.

That will tell Akismet not to change its behaviour based on those API calls: they will have no training effect.
That means your tests will be somewhat repeatable, in the sense that one test won't influence subsequent calls.

```fsharp
open Belin.Akismet

let author = Author("127.0.0.1", UserAgent = "Mozilla/5.0")
let comment = Comment(author, Content = "A user comment.")

// It should not influence subsequent calls.
use client = new Client("123YourAPIKey", Blog "https://www.yourblog.com", IsTest = true)
client.CheckCommentAsync comment |> Async.Ignore |> Async.RunSynchronously
```
