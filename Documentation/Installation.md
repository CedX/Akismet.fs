# Installation

## Requirements
Before installing **Akismet for F#**, you need to make sure you have the [.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/sdk)
and the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools) tool up and running.
		
You can verify if you're already good to go with the following command:

```powershell
dotnet --version
# 10.0.201
```

## Installing with NuGet package manager

### 1. Install it
From a command prompt, run:

```powershell
dotnet package add Belin.Akismet.FSharp
```

### 2. Import it
Now in your [F#](https://learn.microsoft.com/en-us/dotnet/fsharp) code, you can use:

```fsharp
open Belin.Akismet
```
