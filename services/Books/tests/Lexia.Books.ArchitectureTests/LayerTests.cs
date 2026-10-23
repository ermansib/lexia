using NetArchTest.Rules;

namespace Lexia.Books.ArchitectureTests;

/// <summary>Fails the build if a Clean Architecture layer depends on something it must not.</summary>
public sealed class LayerTests
{
    private const string Application = "Lexia.Books.Application";
    private const string Infrastructure = "Lexia.Books.Infrastructure";
    private const string Api = "Lexia.Books.Api";

    [Fact]
    public void Domain_depends_on_no_other_layer_or_framework()
    {
        var result = Types.InAssembly(typeof(Domain.Books.Book).Assembly)
            .ShouldNot().HaveDependencyOnAny(Application, Infrastructure, Api, "Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore", "FluentValidation")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_api()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot().HaveDependencyOnAny(Infrastructure, Api, "Microsoft.EntityFrameworkCore", "Npgsql")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Infrastructure_does_not_depend_on_api()
    {
        var result = Types.InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .ShouldNot().HaveDependencyOn(Api)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }
}
