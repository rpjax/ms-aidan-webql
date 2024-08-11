namespace Webql.Tests;

public class Program
{
    public static void Main()
    {
        // Represents a compiler for Webql queries.
        var compiler = new WebqlCompiler();

        // Compiles the specified webql query into an expression.
        var query = "{ nickname: 'jacques' }";
        var elementType = typeof(object);
        var expression = compiler.Compile(query, elementType);

        // Prints the compiled expression.
        Console.WriteLine(expression);
    }
}