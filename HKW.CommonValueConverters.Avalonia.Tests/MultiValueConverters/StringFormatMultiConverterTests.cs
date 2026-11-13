using System.Globalization;
using Avalonia;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class StringFormatMultiConverterTests
{
    [TestMethod]
    public Task DateAndTimeFormats_UseConfiguredFormatAndParameterOverride() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            Assert.AreEqual(
                "1,5/2,5",
                new StringFormatMultiConverter().Convert([1.5m, 2.5m], null, "{0}/{1}", french)
            );
        });

    [TestMethod]
    public Task SingleAndMultiBoundaries_RejectSentinelsWithoutMutatingInputs() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var format = new StringFormatMultiConverter { ReplaceUnsetValue = "missing" };
            object?[] values = [AvaloniaProperty.UnsetValue, "tail"];
            Assert.AreEqual(
                "missing:tail",
                format.Convert(values, null, "{0}:{1}", CultureInfo.InvariantCulture)
            );
            Assert.AreSame(AvaloniaProperty.UnsetValue, values[0]);
            Assert.AreEqual("tail", values[1]);
        });

    public static IEnumerable<object[]> SharedCases => StringFormatMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<StringFormatMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<StringFormatMultiConverterSuite.Configuration> CreateShared(
        StringFormatMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new StringFormatMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetReplaceUnsetValue is not null)
                    converter.ReplaceUnsetValue = settings.GetReplaceUnsetValue();
                if (settings.GetDefaultResult is not null)
                    converter.DefaultResult = settings.GetDefaultResult();
            }
        );
        Assert.AreEqual(typeof(StringFormatMultiConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase(
                    "integration:StringFormatMultiConverter",
                    () =>
                        new ConverterCase(
                            "StringFormatMultiConverter",
                            typeof(StringFormatMultiConverter),
                            () => new StringFormatMultiConverter(),
                            ["a", "b"],
                            "a:b",
                            [new(0, "c", "c:b"), new(1, "d", "c:d")],
                            "{0}:{1}",
                            true
                        )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new StringFormatMultiConverter());
            var converter = (StringFormatMultiConverter)c.Create();
            var pristine = new StringFormatMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringFormatMultiConverter.ReplaceUnsetValueProperty,
                nameof(StringFormatMultiConverter.ReplaceUnsetValue),
                x => x.ReplaceUnsetValue,
                (x, value) => x.ReplaceUnsetValue = value,
                "alternate-ReplaceUnsetValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringFormatMultiConverter.PreferredCultureProperty,
                nameof(StringFormatMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (StringFormatMultiConverter)configured;
        var actual = (StringFormatMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(
            expected.ReplaceUnsetValue,
            actual.ReplaceUnsetValue
        );
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
