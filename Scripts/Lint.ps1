using module PSScriptAnalyzer
using module ./Cmdlets.psm1

"Performing the static analysis of source code..."
Invoke-ScriptAnalyzer $PSScriptRoot -Recurse
Invoke-FSharpLint Akismet.slnx -Configuration Configuration/FSharpLint.json
Test-ModuleManifest Akismet.psd1 | Out-Null
