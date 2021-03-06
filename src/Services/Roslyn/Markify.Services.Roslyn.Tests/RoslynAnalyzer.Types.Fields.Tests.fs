﻿namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesFieldsTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open TestHelper
    open DefinitionsHelper
    open Xunit
    open Swensen.Unquote

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", 8)>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", 8)>]
    let ``Analyze should return expected field count`` (name, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getFields
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "Field")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "SecondField")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "Field")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "SecondField")>]
    let ``Analyze should return expected field name`` (name, field, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c -> 
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = name)
                    |> getFields
                    |> Seq.filter (fun d -> d.Name = field)
                    |> Seq.length
                count::acc) []

        test <@ actual |> List.forall ((=) 1) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "Field", "private")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "PublicField", "public")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "Field", "Private")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "PublicField", "Public")>]
    let ``Analyze should return all access modifiers when field has some`` (name, field, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let fieldDefinition = getField library.Types name field
                let accessModifiers = Set fieldDefinition.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "ConstField", "const")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "StaticReadOnlyField", "static;readonly")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "ConstField", "Const")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "StaticReadOnlyField", "Shared;ReadOnly")>]
    let ``Analyze should return all modifiers when field has some`` (name, field, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let fieldDefinition = getField library.Types name field
                let accessModifiers = Set fieldDefinition.Modifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>


    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "Field", "int")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "SecondField", "int")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "Field", "Integer")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "SecondField", "Integer")>]
    let ``Analyze should return expected field type`` (name, field, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) = 
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let fieldDefinition = getField library.Types name field
                fieldDefinition.Type::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "Field", "")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.CSharp, "FooType", "ConstField", "1")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "Field", "")>]
    [<MultiProjectData("TypeMembers/ContainerFields", ProjectLanguage.VisualBasic, "FooType", "ConstField", "1")>]
    let ``Analyze should return default value when field has some`` (name, field, value, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = 
            match String.IsNullOrWhiteSpace value with
            | true -> None
            | false -> Some value
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let fieldDefinition = getField library.Types name field
                fieldDefinition.DefaultValue::acc) []

        test <@ actual |> List.forall ((=) expected) @>