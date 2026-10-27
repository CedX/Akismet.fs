open System
open System.Diagnostics
open System.IO
open System.Xml.Linq

/// Contains operations for building a solution.
[<AutoOpen>]
module Tools =

  /// Value indicating whether the requested configuration is "Release".
  let IsRelease: bool =
    fsi.CommandLineArgs |> Array.tail |> Array.exists (fun arg -> arg.ToLower() = "-release")

  /// The configuration to use for project generation.
  let Configuration: string =
    if IsRelease then "Release" else "Debug"

  /// The path to the solution file.
  let Solution: FileInfo =
    Directory.EnumerateFiles(Environment.CurrentDirectory, "*.slnx") |> Seq.exactlyOne |> FileInfo

  /// The project version number.
  let Version: Version =
    let defaultValue = Version.Parse "0.0.0"
    match Solution.FullName |> XDocument.Load |> _.Root with
    | null -> defaultValue
    | element -> match element.Attribute "ProjectVersion" with null -> defaultValue | attribute -> Version.Parse attribute.Value

  /// Deletes the file system entry located at the specified path.
  let removeItem (path: string) =
    if Directory.Exists path then Directory.Delete(path, recursive = true)
    else File.Delete path

  /// Cleans the directory located at the specified path.
  let clearItem (path: string) =
    Directory.EnumerateFileSystemEntries path
    |> Seq.filter (fun entry -> Path.GetFileName entry <> ".gitkeep")
    |> Seq.iter removeItem

  /// Prints the specified message to the standard error and terminates the process.
  let writeError (message: string) =
    eprintfn "%s" message
    Environment.Exit 1

  /// Prints the specified message to the standard output.
  let writeOutput (message: string) =
    printfn "%s" message

  /// Invokes the specified command.
  let startProcess (command: string) (arguments: string) =
    try
      let startedProcess = Process.Start(command, arguments)
      startedProcess.WaitForExit()
      if startedProcess.ExitCode <> 0 then failwith ($"Command failed: {command} {arguments}".TrimEnd())
    with
      ex -> writeError ex.Message
