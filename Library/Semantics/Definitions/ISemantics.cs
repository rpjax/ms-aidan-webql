using System.Reflection;

namespace Webql.Semantics.Definitions;

/// <summary>
/// Represents the base interface for all semantics.
/// </summary>
public interface ISemantics
{
}

/// <summary>
/// Represents semantics that have a type associated with them.
/// </summary>
public interface ITypedSemantics : ISemantics
{
    /// <summary>
    /// Gets the type associated with the semantics.
    /// </summary>
    Type Type { get; }
}

/// <summary>
/// Represents semantics for a query.
/// </summary>
public interface IQuerySemantics : ITypedSemantics
{
}

/// <summary>
/// Represents semantics for an expression.
/// </summary>
public interface IExpressionSemantics : ITypedSemantics
{
}

/// <summary>
/// Represents semantics for member access within an expression.
/// </summary>
public interface IMemberAccessSemantics : IExpressionSemantics
{
    /// <summary>
    /// Gets the property information associated with the member access.
    /// </summary>
    PropertyInfo PropertyInfo { get; }
}

/// <summary>
/// Represents semantics for a property of an anonymous object.
/// </summary>
public interface IAnonymousObjectPropertySemantics : ITypedSemantics
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the property information associated with the anonymous object property.
    /// </summary>
    PropertyInfo PropertyInfo { get; }
}

/// <summary>
/// Represents semantics for an anonymous object.
/// </summary>
public interface IAnonymousObjectSemantics : IExpressionSemantics
{
}

/// <summary>
/// Represents the semantics for a query.
/// </summary>
public class QuerySemantics : IQuerySemantics
{
    /// <summary>
    /// Gets the type associated with the query semantics.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuerySemantics"/> class.
    /// </summary>
    /// <param name="type">The type associated with the query semantics.</param>
    public QuerySemantics(Type type)
    {
        Type = type;
    }
}

/// <summary>
/// Represents the semantics for an expression.
/// </summary>
public class ExpressionSemantics : IExpressionSemantics
{
    /// <summary>
    /// Gets the type associated with the expression semantics.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionSemantics"/> class.
    /// </summary>
    /// <param name="type">The type associated with the expression semantics.</param>
    public ExpressionSemantics(Type type)
    {
        Type = type;
    }
}

/// <summary>
/// Represents the semantics for member access within an expression.
/// </summary>
public class MemberAccessSemantics : IMemberAccessSemantics
{
    /// <summary>
    /// Gets the type associated with the member access semantics.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the property information associated with the member access.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberAccessSemantics"/> class.
    /// </summary>
    /// <param name="type">The type associated with the member access semantics.</param>
    /// <param name="propertyInfo">The property information associated with the member access.</param>
    public MemberAccessSemantics(Type type, PropertyInfo propertyInfo)
    {
        Type = type;
        PropertyInfo = propertyInfo;
    }
}

/// <summary>
/// Represents the semantics for a property of an anonymous object.
/// </summary>
public class AnonymousObjectPropertySemantics : IAnonymousObjectPropertySemantics
{
    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type associated with the anonymous object property semantics.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the property information associated with the anonymous object property.
    /// </summary>
    public PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousObjectPropertySemantics"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="type">The type associated with the anonymous object property semantics.</param>
    /// <param name="propertyInfo">The property information associated with the anonymous object property.</param>
    public AnonymousObjectPropertySemantics(
        string name, 
        Type type, 
        PropertyInfo propertyInfo)
    {
        Name = name;
        Type = type;
        PropertyInfo = propertyInfo;
    }
}

/// <summary>
/// Represents the semantics for an anonymous object.
/// </summary>
public class AnonymousObjectSemantics : IAnonymousObjectSemantics
{
    /// <summary>
    /// Gets the type associated with the anonymous object semantics.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousObjectSemantics"/> class.
    /// </summary>
    /// <param name="type">The type associated with the anonymous object semantics.</param>
    public AnonymousObjectSemantics(Type type)
    {
        Type = type;
    }
}
