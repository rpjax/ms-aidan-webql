﻿using Aidan.Core;
using Webql.Core.Linq;

namespace Webql.Core;

/// <summary>
/// Represents the compilation context for a query.
/// </summary>
public class WebqlCompilationContext
{
    /// <summary>
    /// Gets the Webql compiler settings.
    /// </summary>
    public WebqlCompilerSettings Settings { get; }

    /// <summary>
    /// Gets the type of the element.
    /// </summary>
    public Type ElementType { get; }

    private List<Error> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlCompilationContext"/> class.
    /// </summary>
    /// <param name="settings">The Webql compiler settings.</param>
    /// <param name="elementType">The type of the element.</param>
    public WebqlCompilationContext(
        WebqlCompilerSettings settings,
        Type elementType)
    {
        Settings = settings;
        ElementType = elementType;
        Errors = new List<Error>();
    }

    /// <summary>
    /// Gets the Webql LINQ provider.
    /// </summary>
    public IWebqlLinqProvider LinqProvider => Settings.LinqProvider;

}
