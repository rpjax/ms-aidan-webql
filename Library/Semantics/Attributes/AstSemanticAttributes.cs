﻿namespace Webql.Semantics.Attributes;

/*
 * The attributes are used to store semantic information on the syntax tree nodes. 
 * This design allows for a more flexible and extensible way to store and retrieve semantic information. 
 * Also, it allows for a simple and user firendly API. All semantic information is stored in the AST, including the symbol table.
 * Semantic questions can be asked using the semantic extensions API. 
 * E.g. `node.GetScopeType()`, `node.GetSemantics<TSemantics>()`, `node.GetSymbol<TSymbol>("id")`, etc.
 */

public static class AstSemanticAttributes
{
    public static string ScopeAttribute { get; } = "scope";
    public static string SemanticsCacheAttribute { get; } = "semantics";
}
