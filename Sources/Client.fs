namespace Belin.Akismet

open System
open System.Collections.Generic
open System.Net
open System.Net.Http

/// Submits comments to the Akismet service.
type Client(apiKey: string, blog: Blog) =

  /// The response returned by the `submit-ham` and `submit-spam` endpoints when the outcome is a success.
  [<Literal>]
  static let Success = "Thanks for making the web a better place."

  /// The assembly version.
  static let Version = typeof<Client>.Assembly.GetName().Version |> nonNull

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
  member val UserAgent = $".NET/{Environment.Version} | Belin.Akismet.FSharp/{Version.ToString 3}" with get, set

  interface IDisposable with
    /// Releases any resources associated with this object.
    member this.Dispose() =
      this.Dispose(disposing = true)
      GC.SuppressFinalize this

  /// Checks the API key against the service database, and returns a value indicating whether it is valid.
  member this.VerifyKey(): Result<bool, HttpRequestException> =
    this.VerifyKeyAsync() |> Async.RunSynchronously

  /// Checks the API key against the service database, and returns a value indicating whether it is valid.
  member this.VerifyKeyAsync(): Async<Result<bool, HttpRequestException>> = async {
    try
      let! response = this.PostAsync("1.1/verify-key", None)
      let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
      return Ok (body = "valid")
    with :? HttpRequestException as exn ->
      return Error exn
  }

  /// Releases any resources associated with this object.
  member private _.Dispose(disposing: bool) =
    if not disposed then
      if disposing then httpClient.Dispose()
      disposed <- true

  /// Queries the service by posting the specified fields to a given end point, and returns the response.
  member private this.PostAsync(requestUri: string, fields: IDictionary<string, string> option): Async<HttpResponseMessage> = async {
    let body = this.Blog.ToDictionary()
    body.Add("api_key", this.ApiKey)
    if this.IsTest then body.Add("is_test", "1")
    match fields with None -> () | Some value -> for entry in value do body.Add(entry.Key, entry.Value)

    use request = new HttpRequestMessage(HttpMethod.Post, Uri(this.BaseUrl, requestUri))
    request.Headers.Add("User-Agent", this.UserAgent)

    let! response = httpClient.SendAsync request |> Async.AwaitTask
    response.EnsureSuccessStatusCode() |> ignore

    let statusCode = HttpStatusCode.BadRequest
    match response.Headers.TryGetValues "X-akismet-alert-msg" with
    | false, _ -> ()
    | true, value -> raise (HttpRequestException(nonNull value |> Seq.head, null, statusCode))
    match response.Headers.TryGetValues "X-akismet-debug-help" with
    | false, _ -> ()
    | true, value -> raise (HttpRequestException(nonNull value |> Seq.head, null, statusCode))

    return response
  }
