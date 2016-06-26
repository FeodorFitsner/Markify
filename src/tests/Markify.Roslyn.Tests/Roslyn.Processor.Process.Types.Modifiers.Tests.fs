﻿module Roslyn_Processor_Process_Types_Modifiers_Tests
    open Markify.Roslyn

    open Markify.Models.IDE
    open Markify.Models.Definitions

    open Markify.Core.Analyzers

    open Attributes

    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "public", "PublicClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "internal", "InternalClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "protected", "ProtectedClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "private", "PrivateClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "public", "IPublicInterface")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "internal", "IInternalInterface")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "protected internal", "IProtectedInternalInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "public", "PublicStruct")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "internal", "InternalStruct")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalStruct")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "public", "PublicEnum")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "internal", "InternalEnum")>]
    [<ProjectContextInlineAutoData("EnumProject.xml", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalEnum")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "public", "PublicDelegate")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "internal", "InternalDelegate")>]
    [<ProjectContextInlineAutoData("DelegateProject.xml", ProjectLanguage.CSharp, "protected internal", "ProtectedInternalDelegate")>]
    let ``Process project with types that have access modifiers`` (modifier: string, name, sut: RoslynAnalyzer, project: Project) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = 
            typeDef.AccessModifiers 
            |> Seq.filter (fun c -> Seq.contains c expectedModifiers) 

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "abstract", "AbstractClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "sealed", "SealedClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "partial", "PartialClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "static", "StaticClass")>]
    [<ProjectContextInlineAutoData("InterfaceProject.xml", ProjectLanguage.CSharp, "partial", "IPartialInterface")>]
    [<ProjectContextInlineAutoData("StructProject.xml", ProjectLanguage.CSharp, "partial", "PartialStruct")>]
    let ``Process project with types that have a single modifier`` (modifier, name, sut: RoslynAnalyzer, project: Project) =
        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)

        test <@ Seq.contains modifier typeDef.Modifiers @>

    [<Theory>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "abstract partial", "AbstractPartialClass")>]
    [<ProjectContextInlineAutoData("ClassProject.xml", ProjectLanguage.CSharp, "sealed partial", "SealedPartialClass")>]
    let ``Process project with types that have multiple modifiers`` (modifier: string, name, sut: RoslynAnalyzer, project: Project) =
        let expectedModifiers = modifier.Split [|' '|]

        let typeDef = 
            (sut :> IProjectAnalyzer)
            |> (fun c -> c.Analyze(project))
            |> (fun c -> c.Types)
            |> Seq.find (fun c -> c.Identity.Name = name)
        let possessedModifiers = Seq.filter (fun c -> Seq.contains c expectedModifiers) typeDef.Modifiers

        test <@ Seq.length possessedModifiers = Seq.length expectedModifiers @>