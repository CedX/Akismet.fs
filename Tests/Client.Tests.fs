namespace Belin.Akismet

open Belin.Should
open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Net.Http

/// Tests the features of the `Client` class.
[<TestClass>]
type ClientTests() =

  /// The client used to query the remote API.
  static let client = new Client(
    nonNull (Environment.GetEnvironmentVariable "AKISMET_API_KEY"),
    Blog "https://github.com/CedX/Akismet.fs",
    IsTest = true
  )

  /// A comment with content marked as ham.
  static let ham = Comment(
    Author(
      "192.168.0.1",
      Name = "Akismet",
      Role = AuthorRole.Administrator,
      Url = Some (Uri "https://cedric-belin.fr"),
      UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:156.0) Gecko/20100101 Firefox/156.0"),
    Content = "I'm testing out the Service API.",
    Referrer = Some (Uri "https://www.nuget.org/packages/Belin.Akismet"),
    Type = CommentType.Comment
  )

  /// A comment with content marked as spam.
  static let spam = Comment(
    Author(
      "127.0.0.1",
      Email = "akismet-guaranteed-spam@example.com",
      Name = "viagra-test-123",
      UserAgent = "Spam Bot/6.6.6"),
    Content = "Spam!",
    Date = Some DateTime.Now,
    Type = CommentType.BlogPost
  )

  /// Disposes the Akismet client.
  [<ClassCleanup>]
  static member ClassCleanup() = (client :> IDisposable).Dispose()

  [<TestMethod>]
  member _.CheckComment() =
    client.CheckComment ham |> Should.be (Ok Ham)
    client.CheckComment spam |> Should.beOneOf ([Ok Spam; Ok PervasiveSpam] :> Result<CheckResult, HttpRequestException> seq)

  [<TestMethod>]
  member _.SubmitHam() =
    client.SubmitHam ham |> Result.isOk |> Should.beTrue

  [<TestMethod>]
  member _.SubmitSpam() =
    client.SubmitSpam spam |> Result.isOk |> Should.beTrue

  [<TestMethod>]
  member _.VerifyKey() =
    client.VerifyKey() |> Should.be (Ok true)

    let newClient = new Client("0123456789AB", client.Blog, IsTest = true)
    newClient.VerifyKey() |> Should.be (Ok false)
