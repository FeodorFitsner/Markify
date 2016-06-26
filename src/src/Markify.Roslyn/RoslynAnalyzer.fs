﻿namespace Markify.Roslyn

open Markify.Models
open Markify.Models.IDE
open Markify.Models.Definitions

open Markify.Core.Analyzers

open Inspector
open SourceProvider

type RoslynAnalyzer() =
    let inspectFile path =
        let tree = getSyntaxTree path
        match tree with
        | Some s ->
            s.GetRoot()
            |> searchTypes
            |> Some
        | None -> None

    let inspectProject (files : FilesList) =
        (Seq.empty, files)
        ||> Seq.fold (fun acc c ->
            let types = inspectFile c.AbsolutePath
            match types with
            | Some s -> Seq.append acc s
            | None -> acc )
        |> Seq.filter (fun c ->
            match c.Identity.Name with
            | "" -> false
            | _ -> true )
        |> Seq.distinctBy (fun c -> c.Identity.Fullname)

    interface IProjectAnalyzer with
        member this.Analyze (project : Project) : LibraryDefinition = {
                Project = project.Name
                Types = inspectProject project.Files }