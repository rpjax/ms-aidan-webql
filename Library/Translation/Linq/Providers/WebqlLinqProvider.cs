using Aidan.Core.Linq;
using Aidan.Core.Linq.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using Webql.Core;
using Webql.Core.Extensions;
using Webql.Core.Linq;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Translation.Linq.Providers;

public class WebqlLinqProvider : IWebqlLinqProvider
{
    public static Type DefaultQueryableType { get; } = typeof(IQueryable<>);
    public static Type DefaultAsyncQueryableType { get; } = typeof(IAsyncQueryable<>);

    /*
     * MethodInfo cache
     */

    // TODO: Implement a cache for MethodInfo objects

    /*
     * Type providers
     */

    public virtual Type GetQueryableType(WebqlSyntaxNode node)
    {
        var compilationContext = node.GetCompilationContext();
        var useAsyncQueryable = compilationContext.Settings.UseAsyncQueryable;

        if (useAsyncQueryable && node.IsInRootScope())
        {
            return typeof(IAsyncQueryable<>);
        }

        return typeof(IQueryable<>);
    }

    /*
     * Collection Manipulation LINQ methods
     */

    public MethodInfo GetWhereMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions).GetMethods()
                .Where(x => x.Name == "Where")
                .First()
                .MakeGenericMethod(elementType);
            ;
        }
        else
        {
            return typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Where" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .MakeGenericMethod(elementType);
            ;
        }
    }

    public MethodInfo GetSelectMethodInfo(WebqlExpression source, WebqlExpression selector)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();
        var resultType = selector.GetExpressionType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Select")
                .First()
                .MakeGenericMethod(elementType, resultType);
            ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Select" && m.IsGenericMethodDefinition)
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2
                    && x.Args.Length == 2
                    && x.Params[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                    && x.Params[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .Select(x => x.Method)
                .First(m => m != null)
                .MakeGenericMethod(elementType, resultType);
            ;
        }
    }

    public MethodInfo GetTakeMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Take")
                .First()
                .MakeGenericMethod(elementType)
                ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Take" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType == typeof(int))
                .MakeGenericMethod(elementType);
            ;
        }
    }

    public MethodInfo GetSkipMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "Skip")
                .First()
                .MakeGenericMethod(elementType)
                ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Skip" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>) &&
                    m.GetParameters()[1].ParameterType == typeof(int))
                .MakeGenericMethod(elementType);
            ;
        }
    }

    /*
     * Collection Aggregation LINQ methods
     */

    public MethodInfo GetCountMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "CountAsync")
                .First()
                .MakeGenericMethod(elementType)
                ;
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Count" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetContainsMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Enumerable)
                .GetMethods()
                .Where(m => m.Name == "Contains")
                .Where(m => m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(elementType);

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "ContainsAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Enumerable)
                .GetMethods()
                .Where(m => m.Name == "Contains")
                .Where(m => m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetIndexMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "ElementAt")
                .First()
                .MakeGenericMethod(elementType);

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "ElementAtAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "ElementAt")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetAnyMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Any")
                .First()
                .MakeGenericMethod(elementType);

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "AnyAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Any")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetAllMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "All")
                .First()
                .MakeGenericMethod(elementType);

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "AllAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "All")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetMinMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "MinAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Min")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetMaxMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "MaxAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Max")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetSumMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "SumAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Sum")
                .First()
                .MakeGenericMethod(elementType);
        }
    }

    public MethodInfo GetAverageMethodInfo(WebqlExpression source)
    {
        var sourceType = source.GetExpressionType();
        var elementType = sourceType.GetQueryableElementType();

        if (sourceType.IsAsyncQueryable())
        {
            return typeof(AsyncQueryableExtensions)
                .GetMethods()
                .Where(m => m.Name == "AverageAsync")
                .First()
                .MakeGenericMethod(elementType);
        }
        else
        {
            return typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == "Average")
                .First()
                .MakeGenericMethod(elementType);
        }
    }
}
