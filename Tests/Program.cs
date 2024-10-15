using Webql.Core;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Tests.Models;
using Xunit;

namespace Webql.Tests;

public class Program
{

}

public class WebqlSyntaxTests
{
    private WebqlCompiler Compiler { get; }
    private IQueryable<Account> Source { get; }

    public WebqlSyntaxTests()
    {
        Compiler = new WebqlCompiler(settings: GetCompilerSettings());
        Source = GetSource();
    }

    private static WebqlCompilerSettings GetCompilerSettings()
    {
        return new WebqlCompilerSettings(
            useAsyncQueryable: true);
    }

    private static IQueryable<Account> GetSource()
    {
        // generate a big list of accounts
        return new List<Account>()
        {
            Account.Create()
                .WithNickname("jacques")
                .WithEmail(new Email("rodrigopjax@gmail.com"))
                .Build(),
            Account.Create()
                .WithNickname("alice")
                .WithEmail(new Email("alice@example.com"))
                .Build(),
            Account.Create()
                .WithNickname("bob")
                .WithEmail(new Email("bob@example.com"))
                .Build(),
            Account.Create()
                .WithNickname("carla")
                .WithEmail(new Email("carla@example.com"))
                .Build()
        }.AsQueryable();
    }

    [Fact(DisplayName = "Test the filter operator")]
    public void TestFilter()
    {
        //// Represents a compiler for Webql queries.
        //var compiler = new WebqlCompiler();

        //// Compiles the specified webql query into an expression.
        //var query = "{ $filter: { nickname: 'jacques' } }";
        //var elementType = typeof(Account);
        //var expression = compiler.Compile(query, elementType);

        //// Prints the compiled expression.
        //Console.WriteLine(expression);

        /**/

        var query = @"
        { 
            $filter: { 
                nickname: 'jacques' 
            }, 
            $select: { 
                nickname: nickname,
                email: email
            } 
        }";
        var elementType = typeof(Account);
        var expression = Compiler.Compile(query, elementType);

        // Asserts that the compiled expression is not null.
        Assert.NotNull(expression);
    }

}
