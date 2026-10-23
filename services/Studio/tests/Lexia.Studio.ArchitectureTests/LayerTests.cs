using NetArchTest.Rules;

namespace Lexia.Studio.ArchitectureTests;

/// <summary>Fails the build if a Clean Architecture layer depends on something it must not.</summary>
public sealed class LayerTests
{
    private const string Application = "Lexia.Studio.Application";
    private const string Infrastructure = "Lexia.Studio.Infrastructure";
    private const string Api = "Lexia.Studio.Api";

    [Fact]
    public void Domain_depends_on_no_other_layer_or_framework()
    {
        var result = Types.InAssembly(typeof(Domain.Kdp.TrimSize).Assembly)
            .ShouldNot().HaveDependencyOnAny(Application, Infrastructure, Api, "Microsoft.AspNetCore", "PdfSharp", "FluentValidation")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_api()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .ShouldNot().HaveDependencyOnAny(Infrastructure, Api, "PdfSharp")
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
