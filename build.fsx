// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

RestorePackages()

// Directories
let buildDir  = @".\build\"
let testDir   = @".\test\"
let deployDir = @".\deploy\"
let reportDir = @".\reports\"

// version info
let version = "0.1"  // it will be retrieved from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; deployDir; reportDir]
)

Target "SetAssemblyInfos" (fun _ ->
    CreateCSharpAssemblyInfo "./src/app/FAKENugetDemo/Properties/AssemblyInfo.cs"
        [Attribute.Title "FAKENugetDemo"
         Attribute.Description "Sample project to demonstrate FAKE build of the nuget package"
         Attribute.Guid "23C9BAC6-E44F-44C3-B30E-81B0D104B02D"
         Attribute.Product "FAKE Sample"
         Attribute.Version version
         Attribute.FileVersion version]
)

Target "BuildApp" (fun _ ->
    !! @"src\app\**\*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
    !! @"src\test\**\*.csproj"
      |> MSBuildDebug testDir "Build"
      |> Log "TestBuild-Output: "
)

open MSTest

Target "NUnitTest" (fun _ ->
    !! (testDir + @"NUnitTest.*.dll")
      |> NUnit (fun p ->
                 {p with
                   DisableShadowCopy = true;
                   OutputFile = testDir @@ @"TestResults.xml"})
)

Target "Zip" (fun _ ->
    !! (buildDir + "\**\*.*")
      -- "*.zip"
      |> Zip buildDir (deployDir + "FAKENugetDemo." + version + ".zip")
)

Target "BuildWithoutTests" DoNothing
Target "RunTests" DoNothing
Target "Build" DoNothing
Target "Test" DoNothing

"Clean"
  ==> "SetAssemblyInfos"
  ==> "BuildApp"

"BuildApp"
  ==> "BuildTest"
  ==> "NUnitTest"
  ==> "RunTests"

"BuildApp" ==> "Zip"
"RunTests" ?=> "Zip"

"Zip" ==> "Build"
"RunTests" ==> "Build"
"Zip" ==> "BuildWithoutTests"

// start build
RunTargetOrDefault "Build"
