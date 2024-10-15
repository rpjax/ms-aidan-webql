using System.Runtime.CompilerServices;
using Webql.Core.Analysis;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;
using Webql.Semantics.Attributes;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Scope;
using Webql.Semantics.Symbols;

namespace Webql.Semantics.Extensions;

/*
 * This extension class provides the main API for semantic related operations on the syntax tree.
 */

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeExtensions
{
    /*  
     * Scope related extensions
     */

    /// <summary>
    /// Determines whether the node has a scope attribute.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node has a scope attribute; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasScopeAttribute(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.ScopeAttribute);
    }

    /// <summary>
    /// Gets the scope associated with the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns>The scope associated with the node.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the scope is not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlScope GetScope(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if (!current.HasScopeAttribute())
            {
                current = current.Parent;
                continue;
            }

            if (!current.TryGetAttribute<WebqlScope>(AstSemanticAttributes.ScopeAttribute, out var scope))
            {
                throw new InvalidOperationException();
            }

            return scope;
        }

        throw new InvalidOperationException("Scope not found");
    }

    /// <summary>
    /// Binds a scope to the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="scope">The scope to bind.</param>
    /// <param name="enableOverride">if set to <c>true</c> enables overriding the existing scope attribute.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindScope(
        this WebqlSyntaxNode node,
        WebqlScope scope,
        bool enableOverride = false)
    {
        if (enableOverride && node.HasAttribute(AstSemanticAttributes.ScopeAttribute))
        {
            node.RemoveAttribute(AstSemanticAttributes.ScopeAttribute);
        }

        node.AddAttribute(AstSemanticAttributes.ScopeAttribute, scope);
    }

    /*
     * Semantics related extensions
     */

    /// <summary>
    /// Determines whether the node has a semantics attribute.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node has a semantics attribute; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasSemanticsAttribute(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstSemanticAttributes.SemanticsCacheAttribute);
    }

    /// <summary>
    /// Gets the semantics associated with the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns>The semantics associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISemantics GetSemantics(this WebqlSyntaxNode node)
    {
        if (node.HasSemanticsAttribute())
        {
            //return node.GetAttribute<ISemantics>(AstSemanticAttributes.SemanticsCacheAttribute);
        }

        var semantics = SemanticAnalyzer.CreateSemantics(node.GetCompilationContext(), node);

        /*
         * Caches the semantics in the syntax tree node.
         */
        //node.BindSemantics(semantics);

        return semantics;
    }

    /// <summary>
    /// Gets the semantics of the specified type associated with the node.
    /// </summary>
    /// <typeparam name="TSemantics">The type of the semantics.</typeparam>
    /// <param name="node">The syntax node.</param>
    /// <returns>The semantics of the specified type associated with the node.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the semantics cannot be cast to the specified type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSemantics GetSemantics<TSemantics>(
        this WebqlSyntaxNode node)
        where TSemantics : ISemantics
    {
        var semantics = node.GetSemantics();

        if (semantics is not TSemantics cast)
        {
            throw new InvalidOperationException();
        }

        return cast;
    }

    /// <summary>
    /// Binds semantics to the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="semantics">The semantics to bind.</param>
    /// <param name="enableOverride">if set to <c>true</c> enables overriding the existing semantics attribute.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindSemantics(
        this WebqlSyntaxNode node,
        ISemantics semantics,
        bool enableOverride = false)
    {
        if (enableOverride && node.HasSemanticsAttribute())
        {
            node.RemoveAttribute(AstSemanticAttributes.SemanticsCacheAttribute);
        }

        node.AddAttribute(AstSemanticAttributes.SemanticsCacheAttribute, semantics);
    }

    /*
     * Symbol resolution related extensions
     */

    /// <summary>
    /// Tries to resolve a symbol with the specified identifier.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>The resolved symbol, or <c>null</c> if the symbol is not found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISymbol? TryResolveSymbol(
        this WebqlSyntaxNode node,
        string identifier)
    {
        var scope = node.GetScope();
        var symbol = scope.ResolveSymbol(identifier);

        return symbol;
    }

    /// <summary>
    /// Tries to resolve a symbol of the specified type with the specified identifier.
    /// </summary>
    /// <typeparam name="TSymbol">The type of the symbol.</typeparam>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>The resolved symbol, or <c>null</c> if the symbol is not found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resolved symbol cannot be cast to the specified type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSymbol? TryResolveSymbol<TSymbol>(
        this WebqlSyntaxNode node,
        string identifier)
        where TSymbol : class, ISymbol
    {
        var symbol = node.TryResolveSymbol(identifier);

        if (symbol is null)
        {
            return null;
        }

        if (symbol is not TSymbol typedSymbol)
        {
            throw new InvalidOperationException();
        }

        return typedSymbol;
    }

    /// <summary>
    /// Resolve a symbol with the specified identifier.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>The resolved symbol.</returns>
    /// <exception cref="SemanticException">Thrown when the symbol is not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISymbol ResolveSymbol(
        this WebqlSyntaxNode node,
        string identifier)
    {
        var symbol = node.TryResolveSymbol(identifier);

        if (symbol is null)
        {
            throw node.CreateSymbolNotFoundException(identifier);
        }

        return symbol;
    }

    /// <summary>
    /// Resolve a symbol of the specified type with the specified identifier.
    /// </summary>
    /// <typeparam name="TSymbol">The type of the symbol.</typeparam>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>The resolved symbol.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the resolved symbol cannot be cast to the specified type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSymbol ResolveSymbol<TSymbol>(
        this WebqlSyntaxNode node,
        string identifier)
        where TSymbol : class, ISymbol
    {
        var symbol = node.ResolveSymbol(identifier);

        if (symbol is not TSymbol typedSymbol)
        {
            throw new InvalidOperationException();
        }

        return typedSymbol;
    }

    /// <summary>
    /// Gets the source symbol associated with the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns>The source symbol associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SourceSymbol GetSourceSymbol(this WebqlSyntaxNode node)
    {
        var scope = node.GetScope();
        var symbol = scope.ResolveSymbol<SourceSymbol>(WebqlAstSymbols.SourceIdentifier);

        if (symbol is null)
        {
            throw node.CreateSymbolNotFoundException(WebqlAstSymbols.SourceIdentifier);
        }

        return symbol;
    }

    /// <summary>
    /// Declares a source symbol in the node's scope.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="type">The type of the source symbol.</param>
    /// <exception cref="SemanticException">Thrown when the source symbol is already declared.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DeclareSourceSymbol(
        this WebqlSyntaxNode node,
        Type type)
    {
        var scope = node.GetScope();

        var symbol = new SourceSymbol(
            identifier: WebqlAstSymbols.SourceIdentifier,
            type: type
        );

        if (scope.ContainsSymbol(symbol.Identifier, useParentScope: false))
        {
            node.CreateSymbolAlreadyDeclaredException(symbol.Identifier);
        }

        scope.DeclareSymbol(symbol);
    }

    /// <summary>
    /// Gets the element symbol associated with the node.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns>The element symbol associated with the node.</returns>
    /// <exception cref="SemanticException">Thrown when the element symbol is not found.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSymbol GetElementSymbol(this WebqlSyntaxNode node)
    {
        var scope = node.GetScope();
        var symbol = scope.ResolveSymbol<ParameterSymbol>(WebqlAstSymbols.ElementIdentifier);

        if (symbol is null)
        {
            throw node.CreateSymbolNotFoundException(WebqlAstSymbols.ElementIdentifier);
        }

        return symbol;
    }

    /// <summary>
    /// Declares an element symbol in the node's scope.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="type">The type of the element symbol.</param>
    /// <exception cref="SemanticException">Thrown when the element symbol is already declared.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DeclareElementSymbol(
        this WebqlSyntaxNode node,
        Type type)
    {
        var scope = node.GetScope();

        var symbol = new ParameterSymbol(
            identifier: WebqlAstSymbols.ElementIdentifier,
            type: type
        );

        if (scope.ContainsSymbol(symbol.Identifier, useParentScope: false))
        {
            node.CreateSymbolAlreadyDeclaredException(symbol.Identifier);
        }

        scope.DeclareSymbol(symbol);
    }

    /// <summary>
    /// Gets the expression semantics associated with the node.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <returns>The expression semantics associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IExpressionSemantics GetExpressionSemantics(this WebqlExpression node)
    {
        return node.GetSemantics<IExpressionSemantics>();
    }

    /// <summary>
    /// Gets the type of the expression associated with the node.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <returns>The type of the expression associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetExpressionType(this WebqlExpression node)
    {
        return node.GetExpressionSemantics().Type;
    }

    /// <summary>
    /// Gets the member access semantics associated with the node.
    /// </summary>
    /// <param name="node">The member access expression node.</param>
    /// <returns>The member access semantics associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMemberAccessSemantics GetMemberAccessSemantics(this WebqlMemberAccessExpression node)
    {
        return node.GetSemantics<IMemberAccessSemantics>();
    }

    /// <summary>
    /// Gets the anonymous object semantics associated with the node.
    /// </summary>
    /// <param name="node">The anonymous object expression node.</param>
    /// <returns>The anonymous object semantics associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAnonymousObjectSemantics GetAnonymousObjectSemantics(this WebqlAnonymousObjectExpression node)
    {
        return node.GetSemantics<IAnonymousObjectSemantics>();
    }

    /// <summary>
    /// Gets the anonymous object property semantics associated with the node.
    /// </summary>
    /// <param name="node">The anonymous object property node.</param>
    /// <returns>The anonymous object property semantics associated with the node.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAnonymousObjectPropertySemantics GetAnonymousObjectPropertySemantics(this WebqlAnonymousObjectProperty node)
    {
        return node.GetSemantics<IAnonymousObjectPropertySemantics>();
    }

    /*
     * Generic helpers
     */

    /// <summary>
    /// Determines whether the node is the root of the syntax tree.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is the root; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRoot(this WebqlSyntaxNode node)
    {
        return node.Parent is null;
    }

    /// <summary>
    /// Determines whether the node is not the root of the syntax tree.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is not the root; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotRoot(this WebqlSyntaxNode node)
    {
        return !node.IsRoot();
    }

    /// <summary>
    /// Determines whether the node represents a collection operator.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node represents a collection operator; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollectionOperator(this WebqlSyntaxNode node)
    {
        if (node is not WebqlOperationExpression operationExpression)
        {
            return false;
        }

        return WebqlOperatorAnalyzer.IsCollectionOperator(operationExpression.Operator);
    }

    
    /// <summary>
    /// Determines whether the node is a reference expression.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is a reference expression; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsReferenceExpression(this WebqlSyntaxNode node)
    {
        return node is WebqlReferenceExpression;
    }

    /// <summary>
    /// Determines whether the node is a member access expression.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is a member access expression; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMemberAccessExpression(this WebqlSyntaxNode node)
    {
        return node is WebqlMemberAccessExpression;
    }

    /// <summary>
    /// Determines whether the node is a scope source.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is a scope source; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsScopeSource(this WebqlSyntaxNode node)
    {
        return node.HasScopeAttribute();
    }

    /// <summary>
    /// Determines whether the node is in the root scope.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <returns><c>true</c> if the node is in the root scope; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRootScope(this WebqlSyntaxNode node)
    {
        var current = node;
        var isScopeSourceFound = false;

        while (current is not null)
        {
            if (current.HasScopeAttribute())
            {
                if (isScopeSourceFound)
                {
                    return false;
                }

                isScopeSourceFound = true;
            }

            current = current.Parent;
        }

        return isScopeSourceFound;
    }

    /// <summary>
    /// Ensures that the node has the expected number of operands.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    /// <param name="expectedCount">The expected number of operands.</param>
    /// <exception cref="SemanticException">Thrown when the actual number of operands does not match the expected count.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureOperandCount(this WebqlOperationExpression node, int expectedCount)
    {
        var actualCount = node.Operands.Length;

        if (actualCount != expectedCount)
        {
            throw node.CreateInvalidOperandCountException(expectedCount, actualCount);
        }
    }

    /// <summary>
    /// Ensures that the node has at least one operand.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    /// <exception cref="SemanticException">Thrown when no operands are provided.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureAtLeastOneOperand(this WebqlOperationExpression node)
    {
        var actualCount = node.Operands.Length;

        if (actualCount == 0)
        {
            throw new SemanticException($"Operator '{node.Operator}' expects at least one operand, but none were provided", node);
        }
    }

    /// <summary>
    /// Ensures that the expression is queryable.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <exception cref="SemanticException">Thrown when the expression is not queryable.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureIsQueryable(this WebqlExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var type = semantics.Type;

        if (type.IsNotQueryable())
        {
            throw node.CreateExpressionIsNotQuryableException(type);
        }
    }

    /// <summary>
    /// Ensures that the expression has the specified type.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <param name="type">The expected type of the expression.</param>
    public static void EnsureExpressionType(this WebqlExpression node, Type type)
    {
        if (node.GetExpressionType() != type)
        {
            // TODO...
        }
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that a symbol was not found.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the symbol was not found.</returns>
    public static SemanticException CreateSymbolNotFoundException(
        this WebqlSyntaxNode node,
        string identifier)
    {
        return new SemanticException($"Symbol '{identifier}' not found", node);
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that a property was not found in the specified type.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="type">The type in which the property was not found.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the property was not found.</returns>
    public static SemanticException CreatePropertyNotFoundException(
        this WebqlSyntaxNode node,
        Type type,
        string propertyName)
    {
        return new SemanticException($"Property '{propertyName}' not found in type '{type.FullName}'", node);
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that an operator is incompatible with the specified types.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="leftType">The type of the left operand.</param>
    /// <param name="rightType">The type of the right operand.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the operator is incompatible with the specified types.</returns>
    public static SemanticException CreateOperatorIncompatibleTypeException(
        this WebqlSyntaxNode node,
        Type leftType,
        Type rightType)
    {
        return new SemanticException($@"
Error: Operator Incompatible Type Exception

Description:
The operation you are attempting to perform involves incompatible types.
This error occurs when the types of the operands do not match the expected types
for the operation being executed. Specifically, the operator cannot be applied to 
operands of the given types.

For example, attempting to add a string to an integer or comparing a boolean 
with a string will result in this exception.

Details:
- Left Operand Type: {leftType.FullName}
- Right Operand Type: {rightType.FullName}

Resolution:
Ensure that the operands used in the operation are of compatible types. 
Refer to the documentation or type specifications for the expected operand types 
for the operator being used.
", node);
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that the number of operands is invalid.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    /// <param name="expectedCount">The expected number of operands.</param>
    /// <param name="actualCount">The actual number of operands.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the number of operands is invalid.</returns>
    public static SemanticException CreateInvalidOperandCountException(
        this WebqlOperationExpression node,
        int expectedCount,
        int actualCount)
    {
        return new SemanticException($"Operator '{node.Operator}' expects {expectedCount} operands, but {actualCount} were provided", node);
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that the expression is not queryable.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <param name="type">The type of the expression.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the expression is not queryable.</returns>
    public static SemanticException CreateExpressionIsNotQuryableException(
        this WebqlExpression node,
        Type type)
    {
        return new SemanticException($"Expression of type '{type.FullName}' is not queryable", node);
    }

    /// <summary>
    /// Creates a <see cref="SemanticException"/> indicating that a symbol is already declared.
    /// </summary>
    /// <param name="node">The syntax node.</param>
    /// <param name="identifier">The identifier of the symbol.</param>
    /// <returns>A <see cref="SemanticException"/> indicating that the symbol is already declared.</returns>
    public static SemanticException CreateSymbolAlreadyDeclaredException(
        this WebqlSyntaxNode node,
        string identifier)
    {
        return new SemanticException($"Symbol '{identifier}' is already declared", node);
    }


}
