// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

RestorePackages()

// package info
let title = "FAKENugetDemo"
let product = title
let guid = "23C9BAC6-E44F-44C3-B30E-81B0D104B02D"
let authors = ["Kukukuchu"]
let summary = "Sample project to demonstrate FAKE build of the nuget package"
let description = "This should be a bit longer description of the project, possibly with some markdown etc. Alas, lazyness.."

// Directories
let buildDir  = @".\#build\"
let testDir   = @".\#test\"
let deployDir = @".\#deploy\"
let reportDir = @".\#reports\"
let packagingDir = @".\#packagingArea\"
let nugetOutputDir = deployDir @@ "nuget"
let zipOutputDir = deployDir @@ "zip"

// version info
let version = "0.8"  // it will be retrieved from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; deployDir; reportDir; packagingDir; nugetOutputDir; zipOutputDir]
)

Target "SetAssemblyInfos" (fun _ ->
    CreateCSharpAssemblyInfo "./src/app/FAKENugetDemo/Properties/AssemblyInfo.cs"
        [Attribute.Title title
         Attribute.Description summary
         Attribute.Guid guid
         Attribute.Product product
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
    
    ensureDirectory zipOutputDir
    !! (buildDir + "\**\*.*")
      -- "*.zip"
      |> Zip buildDir (zipOutputDir @@ ("FAKENugetDemo." + version + ".zip"))
)



Target "CreatePackage" (fun _ ->
    !! (buildDir + "\**\*.*")
      -- "*.zip"
        // Copy all the package files into a package folder
        |>CopyFiles packagingDir

    ensureDirectory nugetOutputDir
    NuGet (fun p -> 
            {p with
                Authors = authors
                Project = title
                Description = description                               
                OutputPath = nugetOutputDir
                Summary = summary
                WorkingDir = packagingDir
                Version = version
                AccessKey = "someStrongPassword" //environVar "nugetAccessKey"
                Publish = true
                PublishUrl = "http://kukukuchu-nuget-server.azurewebsites.net/" 
                Files = [
                            ("\**\*.dll", Some "lib", None)
                            //("\**\*.txt", Some "Build", None)
                            //("\**\*.target", Some "Build", None)
                ]
                IncludeReferencedProjects = true}) 
                "NugetTemplate.nuspec"
)

Target "BuildWithoutTests" DoNothing
Target "RunTests" DoNothing
Target "Build" DoNothing
Target "Test" DoNothing
Target "Publish" DoNothing

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

"RunTests" ==> "CreatePackage" ==> "Publish"

// start build
RunTargetOrDefault "Build"
