#r "tools/FAKE/tools/FakeLib.dll"
open Fake

let projName="DomainBus"
let projDir= "..\src" @@ projName
let testDir="..\src" @@ "Tests"

let testOnCore=false
let additionalPack=[]

let localNugetRepo="E:/Libs/nuget"
let nugetExeDir="tools"



