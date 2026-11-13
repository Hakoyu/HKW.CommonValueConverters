using System.Reflection;
using System.Resources;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ConverterTestCoverageTests
{
    [TestMethod]
    public void EveryProductionConverter_HasActiveExecutableLayers()
    {
        var types = ConverterTestCatalog.DiscoverConverters(typeof(ConverterBase).Assembly, type =>
            typeof(ConverterBase).IsAssignableFrom(type)
        );
        var descriptors = ConverterTestCatalog.Discover(GetType().Assembly);
        ConverterTestCatalog.Validate(types, descriptors, Enum.GetValues<ConverterTestLayer>());
    }

    [TestMethod]
    public void PlatformData_AssociatesActualClosedTypesAndCompiledResources()
    {
        var resources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (
            var type in GetType()
                .Assembly.GetTypes()
                .Where(type => type.IsDefined(typeof(TestClassAttribute), true))
        )
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            var associations = method.GetCustomAttributes<ConverterTestAttribute>().ToArray();
            if (associations.Length == 0)
                continue;
            foreach (var source in method.GetCustomAttributes().OfType<ITestDataSource>())
            foreach (var row in source.GetData(method))
            foreach (var scenario in row.OfType<WpfPlatformCase>())
            {
                Assert.IsFalse(scenario.Type.ContainsGenericParameters);
                foreach (var association in associations)
                    Assert.AreEqual(
                        ConverterTestCatalog.Identity(association.ConverterType),
                        ConverterTestCatalog.Identity(scenario.Type)
                    );
                if (associations.Any(association => association.Layer == ConverterTestLayer.Xaml))
                    resources.Add(
                        "testviews/"
                            + scenario.Resource.Replace(
                                ".xaml",
                                ".baml",
                                StringComparison.OrdinalIgnoreCase
                            )
                    );
            }
        }
        using var stream = GetType()
            .Assembly.GetManifestResourceStream("HKW.CommonValueConverters.WPF.Tests.g.resources");
        Assert.IsNotNull(stream);
        using var reader = new ResourceReader(stream);
        var compiled = new List<string>();
        foreach (System.Collections.DictionaryEntry entry in reader)
            if (
                entry.Key is string key
                && key.StartsWith("testviews/", StringComparison.OrdinalIgnoreCase)
                && key.EndsWith(".baml", StringComparison.OrdinalIgnoreCase)
            )
                compiled.Add(key);
        CollectionAssert.AreEquivalent(
            resources.Select(value => value.ToLowerInvariant()).ToArray(),
            compiled.Select(value => value.ToLowerInvariant()).ToArray()
        );
    }
}
