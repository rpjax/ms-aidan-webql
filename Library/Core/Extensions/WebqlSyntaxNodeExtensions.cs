using System.Runtime.CompilerServices;
using Webql.Core.Linq;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Core.Extensions;

/// <summary>  
/// Provides extension methods for <see cref="WebqlSyntaxNode"/> instances.  
/// </summary>  
public static class WebqlSyntaxNodeExtensions
{
    const string CompilationContextKey = "compilation_context";

    /// <summary>  
    /// Casts the specified node to the specified type.  
    /// </summary>  
    /// <typeparam name="T">The type to cast the node to.</typeparam>  
    /// <param name="node">The node to cast.</param>  
    /// <returns>The node cast to the specified type.</returns>  
    /// <exception cref="InvalidOperationException">Thrown when the node cannot be cast to the specified type.</exception>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T As<T>(this WebqlSyntaxNode node) where T : WebqlSyntaxNode
    {
        if (node is T t)
        {
            return t;
        }

        throw new InvalidOperationException("Attempted to cast a node to an incompatible type during semantic analysis.");
    }

    /*  
     * Compilation context related methods  
     */

    /// <summary>  
    /// Gets the compilation context associated with the specified node.  
    /// </summary>  
    /// <param name="node">The node to get the compilation context for.</param>  
    /// <returns>The compilation context associated with the node.</returns>  
    /// <exception cref="InvalidOperationException">Thrown when the compilation context is not found or is of an unexpected type.</exception>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlCompilationContext GetCompilationContext(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if (current.HasAttribute(CompilationContextKey))
            {
                var context = current.GetAttribute<WebqlCompilationContext>(CompilationContextKey);

                if (context is null)
                {
                    throw new InvalidOperationException("The compilation context attribute is not of the expected type.");
                }

                return context;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("The compilation context attribute was not found in the node hierarchy.");
    }

    /// <summary>  
    /// Binds the specified compilation context to the specified node.  
    /// </summary>  
    /// <param name="node">The node to bind the compilation context to.</param>  
    /// <param name="context">The compilation context to bind.</param>  
    /// <exception cref="InvalidOperationException">Thrown when the node is not the root node.</exception>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindCompilationContext(this WebqlSyntaxNode node, WebqlCompilationContext context)
    {
        if (!node.IsRoot())
        {
            throw new InvalidOperationException("The compilation context can only be set on the root node.");
        }

        node.SetAttribute(CompilationContextKey, context);
    }

    /// <summary>  
    /// Gets the LINQ provider associated with the specified node.  
    /// </summary>  
    /// <param name="node">The node to get the LINQ provider for.</param>  
    /// <returns>The LINQ provider associated with the node.</returns>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IWebqlLinqProvider GetLinqProvider(this WebqlSyntaxNode node)
    {
        return node.GetCompilationContext().LinqProvider;
    }

    /// <summary>  
    /// Gets the queryable type associated with the specified node.  
    /// </summary>  
    /// <param name="node">The node to get the queryable type for.</param>  
    /// <returns>The queryable type associated with the node.</returns>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetQueryableType(this WebqlSyntaxNode node)
    {
        return node.GetCompilationContext()
            .LinqProvider
            .GetQueryableType(node);
    }

    /// <summary>  
    /// Gets the compilation element type associated with the specified node.  
    /// </summary>  
    /// <param name="node">The node to get the compilation element type for.</param>  
    /// <returns>The compilation element type associated with the node.</returns>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetCompilationElementType(this WebqlSyntaxNode node)
    {
        return node.GetCompilationContext().ElementType;
    }
}
