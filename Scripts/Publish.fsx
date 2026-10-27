#load "Tools.fsx"
open System
open System.IO
open Tools

if IsRelease then startProcess "dotnet" "fsi Scripts/Default.fsx -Release"
else writeError @"The ""-Release"" switch must be set!"

writeOutput "Tagging the repository..."
startProcess "git" $"tag v{Version}"
startProcess "git" $"push origin v{Version}"

writeOutput "Publishing the package..."
startProcess "dotnet" "pack --no-build --output Temp/NuGet"

let nugetApiKey = Environment.GetEnvironmentVariable "NUGET_API_KEY"
Directory.EnumerateFiles("Temp/NuGet", "*.nupkg")
|> Seq.map (fun file -> $@"nuget push ""{file}"" --api-key {nugetApiKey} --source NuGet")
|> Seq.iter (fun arguments -> startProcess "dotnet" arguments)
