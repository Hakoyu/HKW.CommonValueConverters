using System.Collections;
using Avalonia;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class GetDictionaryValueConverterTests
{
    [TestMethod]
    public Task SingleAndMultiBoundaries_RejectSentinelsWithoutMutatingInputs() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var dictionary = new Hashtable
            {
                ["nil"] = null,
                ["array"] = new[] { 1, 2 },
                ["unset"] = AvaloniaProperty.UnsetValue,
            };
            var single = new GetDictionaryValueConverter { DefaultResult = marker };
            Assert.IsNull(single.Convert(dictionary, null, "nil", null));
            Assert.AreSame(dictionary["array"], single.Convert(dictionary, null, "array", null));
        });

    public static IEnumerable<object[]> SharedCases => GetDictionaryValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<GetDictionaryValueConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<GetDictionaryValueConverterSuite.Configuration> CreateShared(
        GetDictionaryValueConverterSuite.Configuration configuration
    )
    {
        var converter = new GetDictionaryValueConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(GetDictionaryValueConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:GetDictionaryValueConverter", () =>
                    new ConverterCase(
                        "GetDictionaryValueConverter",
                        typeof(GetDictionaryValueConverter),
                        () => new GetDictionaryValueConverter(),
                        [new Hashtable { ["key"] = "before" }],
                        "before",
                        [new(0, new Hashtable { ["key"] = "after" }, "after")],
                        "key",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(GetDictionaryValueConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new GetDictionaryValueConverter());
            var converter = (GetDictionaryValueConverter)c.Create();
            var pristine = new GetDictionaryValueConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                GetDictionaryValueConverter.PreferredCultureProperty,
                nameof(GetDictionaryValueConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (GetDictionaryValueConverter)configured;
        var actual = (GetDictionaryValueConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
