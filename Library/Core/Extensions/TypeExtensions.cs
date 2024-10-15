using Aidan.Core.Linq;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Webql.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified type is queryable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is queryable; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsQueryable(this Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        return false
            || typeof(IEnumerable).IsAssignableFrom(type)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    /// <summary>
    /// Determines whether the specified type is asynchronously queryable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is asynchronously queryable; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsyncQueryable(this Type type)
    {
        return false
            || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>)
            || type.GetInterfaces().Any(i =>
               i.IsGenericType &&
               i.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>));
    }

    /// <summary>
    /// Determines whether the specified type is not queryable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is not queryable; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotQueryable(this Type type)
    {
        return !IsQueryable(type);
    }

    /// <summary>
    /// Determines whether the specified type is not asynchronously queryable.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type is not asynchronously queryable; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotAsyncQueryable(this Type type)
    {
        return !IsAsyncQueryable(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type? TryGetQueryableType(this Type type)
    {
        if (IsAsyncQueryable(type))
        {
            return typeof(IAsyncQueryable<>);
        }

        if (IsQueryable(type))
        {
            return typeof(IQueryable<>);
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetQueryableType(this Type type)
    {
        return TryGetQueryableType(type) 
            ?? throw new InvalidOperationException("The type is not queryable.");
    }

    /// <summary>
    /// Tries to get the element type of the specified queryable type.
    /// </summary>
    /// <param name="type">The queryable type.</param>
    /// <returns>The element type if the type is queryable; otherwise, <c>null</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type? TryGetQueryableElementType(this Type type)
    {
        if (IsNotQueryable(type))
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var asyncQueryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAsyncQueryable<>))
            .FirstOrDefault();

        if (asyncQueryableInterface != null)
        {
            return asyncQueryableInterface.GetGenericArguments()[0];
        }

        var queryableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .FirstOrDefault();

        if (queryableInterface != null)
        {
            return queryableInterface.GetGenericArguments()[0];
        }

        var enumerableInterface = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .FirstOrDefault();

        if (enumerableInterface != null)
        {
            return enumerableInterface.GetGenericArguments()[0];
        }

        return null;
    }

    /// <summary>
    /// Gets the element type of the specified queryable type.
    /// </summary>
    /// <param name="type">The queryable type.</param>
    /// <returns>The element type of the queryable type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type does not have an element type.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type GetQueryableElementType(this Type type)
    {
        var elementType = TryGetQueryableElementType(type);

        if (elementType == null)
        {
            throw new InvalidOperationException("The type does not have an element type.");
        }

        return elementType;
    }
}
