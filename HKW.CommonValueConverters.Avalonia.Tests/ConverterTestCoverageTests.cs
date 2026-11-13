using System.Reflection;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ConverterTestCoverageTests
{
    [TestMethod]
    public void EveryPublicConverterHasActiveObjectOwnedCases()
    {
        var coreTypes = ConverterTestCatalog.DiscoverConverters(
            typeof(Core.CommonConverterBase).Assembly,
            type =>
                type.IsSubclassOf(typeof(Core.ValueConverterBase))
                || type.IsSubclassOf(typeof(Core.MultiValueConverterBase))
        );
        var platformTypes = ConverterTestCatalog.DiscoverConverters(
            typeof(ConverterBase).Assembly,
            type => typeof(ConverterBase).IsAssignableFrom(type)
        );
        ConverterTestCatalog.ValidatePlatformParity(
            coreTypes,
            platformTypes,
            typeof(ConverterBase).Namespace!
        );
        var production = ConverterTestCatalog.DiscoverConverters(
            typeof(ConverterBase).Assembly,
            type => typeof(Core.ICommonValueConverter).IsAssignableFrom(type)
        );
        var entries = ConverterTestCatalog.Discover(typeof(ConverterTestCoverageTests).Assembly);
        ConverterTestCatalog.Validate(
            production,
            entries,
            Enum.GetValues<ConverterTestLayer>(),
            LegacyBoolToStyleConverterTests.Exceptions
        );
    }

    [TestMethod]
    public void GenericXamlAdaptersInheritWithoutOverridingProductionBehavior()
    {
        var adapters = typeof(ConverterTestCoverageTests)
            .Assembly.GetExportedTypes()
            .Where(type =>
                type.BaseType is { IsConstructedGenericType: true } baseType
                && baseType.Assembly == typeof(ConverterBase).Assembly
            );
        Assert.IsNotEmpty(adapters.ToArray());
        foreach (var adapter in adapters)
        {
            Assert.IsEmpty(
                adapter.GetMethods(
                    BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.DeclaredOnly
                )
            );
        }
    }
}
