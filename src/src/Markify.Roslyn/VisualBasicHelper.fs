﻿namespace Markify.Roslyn

open Markify.Models.Definitions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module VisualBasicSyntaxHelper =
    let (|NamespaceNode|_|) (node: SyntaxNode) =
        match node with
        | :? NamespaceBlockSyntax as c -> Some c
        | _ -> None

    let (|ClassNode|_|) (node: SyntaxNode) = 
        match node with
        | :? ClassBlockSyntax as c -> Some c
        | _ -> None

    let (|InterfaceNode|_|) (node : SyntaxNode) =
        match node with
        | :? InterfaceBlockSyntax as c -> Some c
        | _ -> None

    let (|StructNode|_|) (node : SyntaxNode) =
        match node with
        | :? StructureBlockSyntax as c -> Some c
        | _ -> None

    let (|EnumNode|_|) (node : SyntaxNode) =
        match node with
        | :? EnumBlockSyntax as c -> Some c
        | _ -> None

    let (|DelegateNode|_|) (node : SyntaxNode) =
        match node with
        | :? DelegateStatementSyntax as c -> Some c
        | _ -> None

    let (|ObjectNode|_|) (node : SyntaxNode) =
        match node with
        | :? TypeBlockSyntax as x -> Some (x :> DeclarationStatementSyntax)
        | :? EnumBlockSyntax as x -> Some (x :> DeclarationStatementSyntax)
        | _ -> None

    let (|ContainerTypeNode|_|) (node : SyntaxNode) = 
        match node with
        | :? TypeBlockSyntax as x -> Some x
        | _ -> None

    let (|TypeNode|_|) (node : SyntaxNode) = 
        match node with
        | ObjectNode x -> Some x
        | DelegateNode x -> Some (x :> DeclarationStatementSyntax)
        | _ -> None
        
open VisualBasicSyntaxHelper

type VisualBasicNodeHelper() =
    inherit NodeHelper()

    let accessModifiersList = 
        Set [
            SyntaxKind.PublicKeyword
            SyntaxKind.FriendKeyword 
            SyntaxKind.PrivateKeyword
            SyntaxKind.ProtectedKeyword ]

    let getModifiers filter (node : SyntaxNode) =
        let modifiers =
            match node with
            | ContainerTypeNode x -> x.BlockStatement.Modifiers
            | EnumNode x -> x.EnumStatement.Modifiers
            | DelegateNode x -> x.Modifiers
            | _ -> SyntaxTokenList()
        modifiers
        |> Seq.filter filter
        |> Seq.map (fun c -> c.Text)

    let getGenericParameters node =
        let parametersList =
            match node with
            | ContainerTypeNode x -> Some x.BlockStatement.TypeParameterList
            | DelegateNode x -> Some x.TypeParameterList
            | _ -> None
        let parameters =
            match parametersList with
            | None ->  SeparatedSyntaxList()
            | Some x ->
                match x with
                | null -> SeparatedSyntaxList()
                | x -> x.Parameters
        parameters

    override this.GetTypeName node =
        match node with
        | ContainerTypeNode x -> x.BlockStatement.Identifier.Text
        | EnumNode x -> x.EnumStatement.Identifier.Text
        | DelegateNode x -> x.Identifier.Text
        | _ -> ""

    override this.GetNamespaceName node =
        match node with
        | NamespaceNode x -> x.NamespaceStatement.Name.ToString()
        | _ -> ""

    override this.GetTypeKind (node : SyntaxNode) =
        match node with
        | ClassNode _ -> StructureKind.Class
        | InterfaceNode _ -> StructureKind.Interface
        | StructNode _ -> StructureKind.Struct
        | EnumNode _ -> StructureKind.Enum
        | DelegateNode _ -> StructureKind.Delegate
        | _ -> StructureKind.Unknown

    override this.GetModifiers node =
        node
        |> getModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind())
            |> not)

    override this.GetAccessModifiers node =
        node
        |> getModifiers (fun c ->
            accessModifiersList
            |> Set.contains (c.Kind()))

    override this.GetParents node =
        match node with
        | ContainerTypeNode x ->
            let interfaces = 
                x.Implements
                |> Seq.map (fun c -> c.Types)
                |> Seq.concat
            let types =
                x.Inherits
                |> Seq.map (fun c -> c.Types)
                |> Seq.concat
            let allTypes = 
                Seq.append interfaces types
                |> Seq.map (fun c -> c.ToString())
            allTypes
        | EnumNode x -> seq { yield x.EnumStatement.UnderlyingType.Type().ToString() }
        | _ -> Seq.empty

    override this.GetGenericConstraints node =
        let constraints = 
            getGenericParameters node
            |> Seq.map (fun c -> 
            let paramConstraint =
                match c.TypeParameterConstraintClause with
                | :? TypeParameterSingleConstraintClauseSyntax as x -> SeparatedSyntaxList().Add x.Constraint
                | :? TypeParameterMultipleConstraintClauseSyntax as x -> x.Constraints
                | null | _ -> SeparatedSyntaxList()
            let p = {
                TypeConstraints.Name = c.Identifier.Text
                Constraints =
                    paramConstraint
                    |> Seq.map (fun c -> c.ToString())}
            p)
        constraints

    override this.GetGenericParameters node =
        let parameters = 
            getGenericParameters node
            |> Seq.map (fun c -> 
                let modifier =
                    match c.VarianceKeyword.Value with
                    | null -> ""
                    | x -> x.ToString() 
                let p = {
                    Name = c.Identifier.Text
                    Modifier = modifier}
                p)
        parameters

module VisualBasicModule = 
    open Markify.Roslyn.LanguageHelper
    open Microsoft.CodeAnalysis.CSharp

    let nodeHelper = VisualBasicNodeHelper()
    let nodeFactory = NodeFactory(nodeHelper)
    
    let rec getNode node =
        match node with
        | TypeNode _ -> nodeFactory.buildTypeNode node getNode
        | NamespaceNode x -> nodeFactory.buildNamespaceNode x
        | null -> NoNode
        | _ -> nodeFactory.buildOtherNode node getNode

    let readSource (source : string) =
        VisualBasicSyntaxTree.ParseText source

    let inspect source =
        let tree = readSource source
        let root = tree.GetRoot()
        root.DescendantNodes()
        |> Seq.filter (fun c -> 
            match c with
            | TypeNode _ -> true
            | _ -> false)
        |> Seq.map getNode  