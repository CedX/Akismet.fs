namespace Belin.Akismet
// fsharplint:disable TupleCommaSpacing

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading

/// Submits comments to the Akismet service.
type Client(apiKey: string, blog: Blog) =

  /// The response returned by the `submit-ham` and `submit-spam` endpoints when the outcome is a success.
  [<Literal>]
  static let Thanks = "Thanks for making the web a better place."

  /// The assembly version.
  static let version = nonNull (typeof<Client>.Assembly.GetName().Version)

  /// Value indicating whether this object has been disposed.
  let mutable disposed = false

  /// The underlying HTTP client.
  let httpClient = new HttpClient(Timeout = TimeSpan.FromMinutes 1L)

  /// The Akismet API key.
  member val ApiKey: string = apiKey with get, set

  /// The base URL of the remote API endpoint.
  member val BaseUrl = Uri "https://rest.akismet.com/" with get, set

  /// The front page or home URL of the instance making requests.
  member val Blog: Blog = blog with get, set

  /// Value indicating whether the client operates in test mode.
  member val IsTest = false with get, set

  /// The user agent string to use when making requests.
  member val UserAgent = $".NET/{Environment.Version} | Belin.Akismet.FSharp/{version.ToString 3}" with get, set

  interface IDisposable with
    /// Releases any resources associated with this object.
    member this.Dispose() =
      this.Dispose(disposing = true)
      GC.SuppressFinalize this

  /// Checks the specified comment against the service database, and returns a value indicating whether it is spam.
  member this.CheckCommentAsync(comment: Comment, ?cancellationToken: CancellationToken): Async<Result<CheckResult, HttpRequestException>> = async {
    match! this.PostAsync("1.1/comment-check", comment.ToDictionary(), ?cancellationToken = cancellationToken) with
    | Error error -> return Error error
    | Ok message ->
      use response = message
      match! response.Content.ReadAsStringAsync() |> Async.AwaitTask with
      | "false" -> return Ok Ham
      | _ ->
        match Client.GetHttpHeader response.Headers "X-akismet-pro-tip" with
        | Some header when header = "discard" -> return Ok PervasiveSpam
        | _ -> return Ok Spam
  }

  /// Checks the specified comment against the service database, and returns a value indicating whether it is spam.
  member this.CheckComment(comment: Comment, ?cancellationToken: CancellationToken): Result<CheckResult, HttpRequestException> =
    this.CheckCommentAsync(comment, ?cancellationToken = cancellationToken) |> Async.RunSynchronously

  /// Submits the specified comment that was incorrectly marked as spam but should not have been.
  member this.SubmitHamAsync(comment: Comment, ?cancellationToken: CancellationToken): Async<Result<unit, HttpRequestException>> = async {
    match! this.PostAsync("1.1/submit-ham", comment.ToDictionary(), ?cancellationToken = cancellationToken) with
    | Error error -> return Error error
    | Ok message ->
      use response = message
      match! response.Content.ReadAsStringAsync() |> Async.AwaitTask with
      | body when body = Thanks -> return Ok ()
      | _ -> return Error (HttpRequestException "Invalid server response.")
  }

  /// Submits the specified comment that was incorrectly marked as spam but should not have been.
  member this.SubmitHam(comment: Comment, ?cancellationToken: CancellationToken): Result<unit, HttpRequestException> =
    this.SubmitHamAsync(comment, ?cancellationToken = cancellationToken) |> Async.RunSynchronously

  /// Submits the specified comment that was not marked as spam but should have been.
  member this.SubmitSpamAsync(comment: Comment, ?cancellationToken: CancellationToken): Async<Result<unit, HttpRequestException>> = async {
    match! this.PostAsync("1.1/submit-spam", comment.ToDictionary(), ?cancellationToken = cancellationToken) with
    | Error error -> return Error error
    | Ok message ->
      use response = message
      match! response.Content.ReadAsStringAsync() |> Async.AwaitTask with
      | body when body = Thanks -> return Ok ()
      | _ -> return Error (HttpRequestException "Invalid server response.")
  }

  /// Submits the specified comment that was not marked as spam but should have been.
  member this.SubmitSpam(comment: Comment, ?cancellationToken: CancellationToken): Result<unit, HttpRequestException> =
    this.SubmitSpamAsync(comment, ?cancellationToken = cancellationToken) |> Async.RunSynchronously

  /// Checks the API key against the service database, and returns a value indicating whether it is valid.
  member this.VerifyKeyAsync(?cancellationToken: CancellationToken): Async<Result<bool, HttpRequestException>> = async {
    match! this.PostAsync("1.1/verify-key", ?cancellationToken = cancellationToken) with
    | Error error -> return Error error
    | Ok message ->
      use response = message
      let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
      return Ok (body = "valid")
  }

  /// Checks the API key against the service database, and returns a value indicating whether it is valid.
  member this.VerifyKey(?cancellationToken: CancellationToken): Result<bool, HttpRequestException> =
    this.VerifyKeyAsync(?cancellationToken = cancellationToken) |> Async.RunSynchronously

  /// Releases any resources associated with this object.
  member private _.Dispose(disposing: bool) =
    if not disposed then
      if disposing then httpClient.Dispose()
      disposed <- true

  /// Queries the service by posting the specified fields to a given end point, and returns the response.
  // fsharplint:disable-next-line MaxCharactersOnLine
  member private this.PostAsync(requestUri: string, ?fields: IDictionary<string, string>, ?cancellationToken: CancellationToken): Async<Result<HttpResponseMessage, HttpRequestException>> = async {
    let body = this.Blog.ToDictionary()
    body.Add("api_key", this.ApiKey)
    if this.IsTest then body.Add("is_test", "1")
    match fields with None -> () | Some entries -> for entry in entries do body.Add(entry.Key, entry.Value)

    use request = new HttpRequestMessage(HttpMethod.Post, Uri(this.BaseUrl, requestUri), Content = new FormUrlEncodedContent(body))
    request.Headers.Add("User-Agent", this.UserAgent)

    try
      let! ambientToken = Async.CancellationToken
      let! response = httpClient.SendAsync(request, defaultArg cancellationToken ambientToken) |> Async.AwaitTask
      response.EnsureSuccessStatusCode() |> ignore<HttpResponseMessage>

      let statusCode = HttpStatusCode.BadRequest
      match Client.GetHttpHeader response.Headers "X-akismet-alert-msg" with
      | None -> ()
      | Some header -> raise (HttpRequestException(header, null, statusCode))
      match Client.GetHttpHeader response.Headers "X-akismet-debug-help" with
      | None -> ()
      | Some header -> raise (HttpRequestException(header, null, statusCode))

      return Ok response
    with
    | :? HttpRequestException as ex -> return Error ex
    | :? AggregateException as ex ->
      match ex.InnerException with
      | :? HttpRequestException as innerEx -> return Error innerEx
      | _ -> return raise (ex.Flatten())
  }

  /// Gets the value of the HTTP header with the specified name.
  static member private GetHttpHeader (headers: HttpResponseHeaders) (name: string): string option =
    match headers.TryGetValues name with
    | false, _ -> None
    | true, header ->
      match header with
      | null -> None
      | values when Seq.isEmpty values -> None
      | values -> values |> Seq.head |> Some
