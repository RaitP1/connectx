using System.Reflection;

namespace ConnectX.UnitTests.Architecture;

public class ArchitectureTests
{
    private static readonly string[] ConnectXAssemblyNames =
        ["Domain", "Application", "Infrastructure", "ConsoleApp", "WebApp"];

    private static HashSet<string> GetConnectXReferences(Assembly assembly)
    {
        return assembly.GetReferencedAssemblies()
            .Where(a => ConnectXAssemblyNames.Contains(a.Name))
            .Select(a => a.Name!)
            .ToHashSet();
    }

    [Fact]
    public void Domain_References_No_ConnectX_Projects()
    {
        var domainAssembly = typeof(global::Domain.DomainMarker).Assembly;
        var references = GetConnectXReferences(domainAssembly);

        Assert.Empty(references);
    }

    [Fact]
    public void Application_References_Only_Domain()
    {
        var applicationAssembly = typeof(global::Application.ApplicationMarker).Assembly;
        var references = GetConnectXReferences(applicationAssembly);

        Assert.Equal(new HashSet<string> { "Domain" }, references);
    }

    [Fact]
    public void Infrastructure_Does_Not_Reference_Presentation_Layers()
    {
        var infrastructureAssembly = typeof(Infrastructure.InfrastructureMarker).Assembly;
        var references = GetConnectXReferences(infrastructureAssembly);

        Assert.DoesNotContain("ConsoleApp", references);
        Assert.DoesNotContain("WebApp", references);
    }

    [Fact]
    public void WebApp_References_Only_Domain_Application_Infrastructure()
    {
        var webAppAssembly = typeof(WebApp.DefaultConfigSeeder).Assembly;
        var references = GetConnectXReferences(webAppAssembly);

        Assert.Equal(new HashSet<string> { "Domain", "Application", "Infrastructure" }, references);
    }
}
