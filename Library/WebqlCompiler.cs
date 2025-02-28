﻿using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;
using Webql.Translation.Linq.Translators;

namespace Webql;

/// <summary>
/// Represents a compiler for Webql queries.
/// </summary>
public class WebqlCompiler
{   
    private WebqlCompilerSettings Settings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlCompiler"/> class.
    /// </summary>
    /// <param name="settings">The compiler settings.</param>
    public WebqlCompiler(WebqlCompilerSettings? settings = null)
    {
        Settings = settings ?? new WebqlCompilerSettings();
    }

    /// <summary>
    /// Compiles the specified webql query into an expression.
    /// </summary>
    /// <param name="query">The Webql query.</param>
    /// <param name="elementType">The type of the elements in the collection.</param>
    /// <returns>The compiled expression.</returns>
    public LambdaExpression Compile(string query, Type elementType)
    {
        // Represents the compilation process. Each compilation process has its own context.
        var context = new WebqlCompilationContext(Settings, elementType);

        /*
         * Analysis.
         */
            
        // Parses the raw query into an AST.
        var syntaxTree = WebqlParser.ParseToAst(query) as WebqlSyntaxNode;

        // Executes the analysis pipeline. It may include tree transformations and other analysis steps.
        SemanticAnalyzer.ExecuteSemanticalAnalysis(context: context, node: ref syntaxTree);

        /*
         * Synthesis.
         */

        // Translates the AST to a LINQ expression.
        return WebqlLinqTranslator.TranslateQuery(node: (syntaxTree as WebqlQuery)!);
    }

}
