#load "Tools.fsx"
open System
open System.IO
open Tools

writeOutput "Deleting all generated files..."
Directory.EnumerateDirectories(Environment.CurrentDirectory, "obj", SearchOption.AllDirectories) |> Seq.iter removeItem
removeItem "Binaries"
clearItem "Temp"
