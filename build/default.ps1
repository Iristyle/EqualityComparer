function Get-CurrentDirectory
{
  $thisName = $MyInvocation.MyCommand.Name
  [IO.Path]::GetDirectoryName((Get-Content function:$thisName).File)
}

Properties {
  $currentDirectory = Get-CurrentDirectory
  $BuildOutDir = Join-Path $currentDirectory 'BuildArtifacts'
  $PackagesDirectory = Join-Path $currentDirectory 'Packages'
  $7ZipPath = Join-Path $PackagesDirectory '7Zip\7za.exe'
  #set by Jenkins when in his environment

  $buildTag = @($Env:BUILD_TAG, 'EqualityComparerManualBuild') |
    ? { -not [string]::IsNullOrEmpty($_) } | Select -First 1
  $ZipPath = "$BuildOutDir\$buildTag.zip"
  $ZipSymbolsPath = "$BuildOutDir\$buildTag-Symbols.zip"
}

Task default -Depends Test, Package

Task Compile -Depends Init,Clean {
  "Starting compilation process... "

  exec { msbuild build.proj }
}

filter binaries { if ('.pdb','.locked','.pssym' -contains $_.Extension) { $_ } }
filter intellisenseXml { if(((Split-Path -Leaf $_.Directory) -eq 'bin') `
    -and ($_.Extension -eq '.xml')) { $_ } }
filter toolXml { if(($_.DirectoryName.StartsWith("$BuildOutDir\tools\", 'CurrentCultureIgnoreCase')) `
  -and ($_.Extension -eq '.xml')) { $_ } }
filter symbols { if ($_.Extension -eq '.pdb') { $_ } }
filter tests { if (($_.Name -imatch '.*Tests\.dll') -and `
  ($_.Extension -eq '.dll')) { $_ } }
filter testFrameworks { if (($_.Name -imatch '.*xunit.*') -or `
  ($_.Name -imatch '.*autofixture.*') -or ($_.Name -imatch '.*fakeiteasy.*') -or `
  ($_.Name -imatch '.*semanticcomparison.*')) { $_ } }

function Get-FileLists
{
  $allFilesRecursive = Get-ChildItem $BuildOutDir -Recurse |
    ? { -not $_.PsIsContainer }

  $allExcludes =
    (($allFilesRecursive | binaries) +
      ($allFilesRecursive | toolXml) +
      ($allFilesRecursive | intellisenseXml) +
      ($allFilesRecursive | tests) +
      ($allFilesRecursive | testFrameworks)) |
    Select -ExpandProperty FullName

  $fileExcludes = $allExcludes + `
    ($serviceFiles | Select -ExpandProperty FullName)

  $files = $allFilesRecursive |
    ? { $fileExcludes -notcontains $_.FullName }

  #Symbols
  $symbols = $allFilesRecursive |
    symbols

  return $files, $symbols
}

Task RemoveTemporaryAssets {
  #after everything is packaged
}

Task Package -Depends Compile, RemoveTemporaryAssets {
  'Retrieving list of artifacts to zip'
  $files, $symbols = Get-FileLists

  #Zip it up yo!

  Set-7ZipPath $7ZipPath
  "Creating $ZipPath from files"
  New-ZipFile -Path $ZipPath -SourceFiles $files `
    -Root $BuildOutDir -Type zip

  "Creating $ZipSymbolsPath from symbols"
  New-ZipFile -Path $ZipSymbolsPath -SourceFiles $symbols `
    -Root $BuildOutDir -Type zip
}

Task Clean -Depends Init {
  "Removing old zips"

  #old cruft if it's hanging around
  if (Test-Path $BuildOutDir)
    { Remove-Item "$BuildOutDir\*.zip" }
}

Task Init {
  "init"
}

Task Test -Depends Compile {
  Invoke-Xunit -Path $BuildOutDir #-ExcludeTraits @{"Category" = "Integration"}
}
