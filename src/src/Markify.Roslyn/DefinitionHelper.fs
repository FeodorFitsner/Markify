﻿namespace Markify.Roslyn

[<AutoOpen>]
module DefinitionHelper =
    open Markify.Models.Definitions

    let getFullname identity =
        match (identity.Namespace, identity.Parents) with
        | (Some x, Some y) -> sprintf "%s.%s.%s" x y identity.Name
        | (Some x, None) | (None, Some x) -> sprintf "%s.%s" x identity.Name
        | _ -> identity.Name